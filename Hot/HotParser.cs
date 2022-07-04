using Hot.Ast;

namespace Hot;

/// <summary>
/// 语法分析器
/// </summary>
public class HotParser : IDisposable
{
    private HotLexer? lexer;
    private List<HotLexeme> lexemes;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lexer"></param>
    public HotParser(HotLexer lexer)
    {
        this.lexer = lexer;
        lexemes = new List<HotLexeme>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (lexer != null)
        {
            lexer.Dispose();
            lexer = null;
        }
    }

    public HotAstModuleDefine Parse()
    {
        return MatchModuleDefine();
    }

    /// <summary>
    /// moduleDefine ::= 'mod' identifier ';' statement*
    /// </summary>
    /// <returns></returns>
    private HotAstModuleDefine MatchModuleDefine()
    {
        Match(HotToken.KeywordMod);
        var id = Match(HotToken.Identifier);
        Match(HotToken.SignSemicolon);

        var body = new List<HotAst>();
        while (true)
        {
            HotLexeme lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.EOF)
            {
                break;
            }
            body.Add(MatchStatement());
        }

        return new HotAstModuleDefine
        {
            Name = (id.Content as string)!,
            Body = body,
        };
    }

    /// <summary>
    /// functionParameters ::=
    ///     ε
    ///     identifier (',' identifier)*
    /// </summary>
    /// <returns></returns>
    private List<string> MatchFunctionParameters()
    {
        var result = new List<string>();
        var lexeme = PeekLexeme();
        while (lexeme.Token == HotToken.Identifier)
        {
            PopLexeme();
            result.Add((lexeme.Content as string)!);
            lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.SignComma)
            {
                PopLexeme();
                lexeme = PeekLexeme();
            }
        }
        return result;
    }


    /// <summary>
    /// functionDefine ::=
    ///     'fn' '(' functionParameters ')' '->' '{' block '}'
    ///     'fn' '(' functionParameters ')' '->' expression
    /// </summary>
    /// <returns></returns>
    private HotAstFunctionDefine MatchFunctionDefine()
    {
        Match(HotToken.KeywordFn);

        var parameters = MatchFunctionParameters();

        Match(HotToken.SignArrowRight);

        var lexeme = PeekLexeme();
        if (lexeme.Token == HotToken.SignBraceLeft)
        {
            var block = MatchBlock();
            return new HotAstFunctionDefine()
            {
                Parameters = parameters,
                Body = block,
            };
        }

        var expression = MatchExpression();

        return new HotAstFunctionDefine()
        {
            Parameters = parameters,
            Body = expression,
        };
    }

    /// <summary>
    /// block ::= '{' statement* '}'
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    private HotAstBlock MatchBlock()
    {
        Match(HotToken.SignBraceLeft);
        var result = new List<HotAst>();
        while (true)
        {
            HotLexeme lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.SignBraceRight)
            {
                break;
            }
            result.Add(MatchStatement());
        }
        Match(HotToken.SignBraceRight);
        return new HotAstBlock
        {
            Statements = result,
        };
    }

    /// <summary>
    /// listDefine ::= '[' expression* ']'
    /// </summary>
    /// <returns></returns>
    private HotAstListDefine MatchListDefine()
    {
        Match(HotToken.SignBracketLeft);
        var items = new List<HotAst>();
        var lexeme = PeekLexeme();
        while (lexeme.Token != HotToken.SignBracketRight)
        {
            items.Add(MatchExpression());
            lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.SignComma)
            {
                PopLexeme();
                lexeme = PeekLexeme();
            }
            else
            {
                break;
            }
        }
        Match(HotToken.SignBracketRight);
        return new HotAstListDefine
        {
            Items = items,
        };
    }

    /// <summary>
    /// dictionaryDefine ::= '{' ((expression ':' expression) (',' expression ':' expression)*)? '}'
    /// </summary>
    /// <returns></returns>
    private HotAstDictionaryDefine MatchDictionaryDefine()
    {
        Match(HotToken.SignBraceLeft);
        var items = new List<KeyValuePair<HotAst, HotAst>>();
        var lexeme = PeekLexeme();
        while (lexeme.Token != HotToken.SignBraceRight)
        {
            var key = MatchExpression();
            Match(HotToken.SignColon);
            var value = MatchExpression();
            items.Add(new KeyValuePair<HotAst, HotAst>(key, value));
            lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.SignComma)
            {
                PopLexeme();
                lexeme = PeekLexeme();
            }
            else
            {
                break;
            }
        }
        Match(HotToken.SignBraceRight);
        return new HotAstDictionaryDefine
        {
            Items = items,
        };
    }

    /// <summary>
    /// variableDefine ::= 'let' identifier '=' expression ';'
    /// </summary>
    /// <returns></returns>
    private HotAstVariableDefine MatchVariableDefine()
    {
        Match(HotToken.KeywordLet);
        var id = Match(HotToken.Identifier);
        Match(HotToken.SignEqual);
        var expression = MatchExpression();
        Match(HotToken.SignSemicolon);
        return new HotAstVariableDefine
        {
            Identifier = id.Content as string,
            Expression = expression,
        };
    }

    /// <summary>
    /// return ::= 'ret' expression ';'
    /// </summary>
    /// <returns></returns>
    private HotAstReturn MatchReturn()
    {
        Match(HotToken.KeywordRet);
        var expression = MatchExpression();
        Match(HotToken.SignSemicolon);
        return new HotAstReturn
        {
            Expression = expression
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private HotAstAccess MatchAccess()
    {
        var name = Match(HotToken.Identifier);
        var lexeme = PeekLexeme();
        var result = new HotAstAccess
        {
            Name = (name.Content as string)!,
        };
        if (lexeme.Token == HotToken.SignDot)
        {
            Match(HotToken.SignDot);
            result.Access = MatchAccess();
        }
        return result;
    }

    /// <summary>
    /// operand ::=
    ///     true
    ///     false
    ///     number
    ///     string
    ///     identifier
    ///     ('-'|'+') (number | identifier)
    ///     '(' expression ')'
    /// </summary>
    /// <returns></returns>
    private HotAst MatchOperand()
    {
        var lexeme = PeekLexeme();
        switch (lexeme.Token)
        {
            case HotToken.KeywordFalse:
            case HotToken.KeywordTrue:
            case HotToken.Number:
            case HotToken.String:
                lexeme = PopLexeme();
                return new HotAstOperand
                {
                    Operand = lexeme,
                };
            case HotToken.Identifier:
                var access = MatchAccess();
                var p = PeekLexeme();
                if (p.Token == HotToken.SignParentheseLeft)
                {
                    return MatchFunctionCall(access);
                }
                return access;
            case HotToken.SignPlus:
            case HotToken.SignMinus:
                lexeme = PopLexeme();
                var operand = PopLexeme();
                return new HotAstOperand
                {
                    Sign = lexeme,
                    Operand = operand,
                };
            case HotToken.SignParentheseLeft:
                lexeme = PopLexeme();
                var expression = MatchExpression();
                Match(HotToken.SignParentheseRight);
                return new HotAstOperand
                {
                    Expression= expression,
                };
            default:
                throw new HotException($"语法错误，不是有效的操作数 {lexeme}");
        }
    }

    /// <summary>
    /// operation ::= operand (('+'|'-'|'*'|'/') operand)*
    /// </summary>
    /// <returns></returns>
    private HotAst MatchOperation()
    {
        var operand = MatchOperand();
        var root = new HotAstOperation();
        root.Left = operand;
        var current = root;
    loop:
        var lexeme = PeekLexeme();
        switch (lexeme.Token)
        {
            case HotToken.SignPlus:
            case HotToken.SignMinus:
            case HotToken.SignStar:
            case HotToken.SignSlash:
            case HotToken.SignEqual:
            case HotToken.SignGreater:
            case HotToken.SignGreaterEqual:
            case HotToken.SignLess:
            case HotToken.SignLessEqual:
            case HotToken.SignAnd:
            case HotToken.SignOr:
                var operation = PopLexeme();
                var right = MatchOperand();
                if (root.Operation is null)
                {
                    root.Operation = operation;
                    root.Right = right;
                }
                else
                {
                    var one = new HotAstOperation
                    {
                        Top = current,
                        Operation = operation,
                        Left = current.Right,
                        Right = right,
                    };
                    current.Right = one;
                    root = HotAstOperation.Adjust(one, root);
                    current = one;
                }
                goto loop;
            default:
                break;
        }

        if (root.Operation is null)
        {
            return operand;
        }

        // TODO 优先级 和 结合性调整
        return root;
    }

    /// <summary>
    /// expression ::= 
    ///     functionDefine
    ///     operation
    /// </summary>
    /// <returns></returns>
    private HotAst MatchExpression()
    {
        var p1 = PeekLexeme();
        if (p1.Token == HotToken.KeywordFn)
        {
            return MatchFunctionDefine();
        }
        if (p1.Token == HotToken.SignBracketLeft)
        {
            return MatchListDefine();
        }
        if (p1.Token == HotToken.SignBraceLeft)
        {
            return MatchDictionaryDefine();
        }

        return MatchOperation();
    }

    /// <summary>
    /// functionParameters ::=
    ///     ε
    ///     expression (',' expression)*
    /// </summary>
    /// <returns></returns>
    private List<HotAst> MatchFunctionArguments()
    {
        var result = new List<HotAst>();
        var lexeme = PeekLexeme();
        while (lexeme.Token != HotToken.SignParentheseRight)
        {
            result.Add(MatchExpression());
            lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.SignComma)
            {
                PopLexeme();
                lexeme = PeekLexeme();
            }
        }
        return result;
    }

    /// <summary>
    /// functionCall ::= access '(' functionArguments ')'
    /// </summary>
    /// <returns></returns>
    private HotAst MatchFunctionCall(HotAstAccess access)
    {
        Match(HotToken.SignParentheseLeft);
        var arguments = MatchFunctionArguments();
        Match(HotToken.SignParentheseRight);
        return new HotAstFunctionCall
        {
            Access = access,
            Arguments = arguments,
        };
    }

    /// <summary>
    /// assign ::= identifier '=' expression ';'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotAstAssign MatchAssign(HotAstAccess access)
    {
        Match(HotToken.SignEqual);
        var expression = MatchExpression();
        Match(HotToken.SignSemicolon);
        return new HotAstAssign
        {
            Access = access,
            Expression = expression,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private HotAstIf MatchIf()
    {
        Match(HotToken.KeywordIf);
        var condition = MatchExpression();
        var body = MatchBlock();
        var lexeme = PeekLexeme();
        HotAst? elseBlock = null;
        if (lexeme.Token == HotToken.KeywordElse)
        {
            Match(HotToken.KeywordElse);
            lexeme = PeekLexeme();
            if (lexeme.Token == HotToken.KeywordIf)
            {
                elseBlock = MatchIf();
            }
            else
            {
                elseBlock = MatchBlock();
            }
        }
        return new HotAstIf
        {
            Condition = condition,
            Body = body,
            ElseBlock = elseBlock,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private HotAstLoop MatchLoop()
    {
        Match(HotToken.KeywordLoop);
        return new HotAstLoop
        {

        };
    }

    /// <summary>
    /// statement ::=
    ///     variableDefine
    ///     return
    ///     assign
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotAst MatchStatement()
    {
        var lexeme = PeekLexeme();
        switch (lexeme.Token)
        {
            case HotToken.KeywordLet:
                return MatchVariableDefine();
            case HotToken.KeywordRet:
                return MatchReturn();
            case HotToken.KeywordIf:
                return MatchIf();
            case HotToken.Identifier:
                var access = MatchAccess();
                var p = PeekLexeme();
                if (p.Token == HotToken.SignEqual)
                {
                    return MatchAssign(access);
                }
                var call = MatchFunctionCall(access);
                Match(HotToken.SignSemicolon);
                return call;
        }
        throw new HotException($"语法错误,不是预期的语句 {lexeme}");
    }

    /// <summary>
    /// 匹配词素
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotLexeme Match(HotToken token)
    {
        var lexeme = PopLexeme();
        if (lexeme.Token != token)
        {
            throw new HotException($"语法错误，{lexeme} 不匹配预期的 {token}");
        }
        return lexeme;
    }

    /// <summary>
    /// 弹出词素
    /// </summary>
    /// <returns></returns>
    private HotLexeme PopLexeme()
    {
        if (lexemes.Count > 0)
        {
            var r = lexemes[0];
            lexemes.RemoveAt(0);
            return r;
        }
        return lexer!.PopLexeme();
    }

    /// <summary>
    /// 向前看词素
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private HotLexeme PeekLexeme(int i = 1)
    {
        for(int j = lexemes.Count; j < i; ++j)
        {
            var lexeme = lexer!.PopLexeme();
            lexemes.Add(lexeme);
        }
        return lexemes[i - 1];
    }
}
