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
    /// functionDefine ::= 'fn' '(' functionParameters ')' '->' '{' block '}'
    /// </summary>
    /// <returns></returns>
    private HotAstFunctionDefine MatchFunctionDefine()
    {
        Match(HotToken.KeywordFn);

        var parameters = MatchFunctionParameters();

        Match(HotToken.SignArrowRight);

        var body = MatchBlock();

        return new HotAstFunctionDefine()
        {
            Parameters = parameters,
            Body = body,
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
        var lexeme = PopLexeme();
        switch (lexeme.Token)
        {
            case HotToken.KeywordFalse:
            case HotToken.KeywordTrue:
            case HotToken.Number:
            case HotToken.String:
            case HotToken.Identifier:
                return new HotAstOperand
                {
                    Operand = lexeme,
                };
            case HotToken.SignPlus:
            case HotToken.SignMinus:
                var operand = PopLexeme();
                return new HotAstOperand
                {
                    Sign = lexeme,
                    Operand = operand,
                };
            case HotToken.SignParentheseLeft:
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

        var p2 = PeekLexeme(2);
        if (p1.Token == HotToken.Identifier && p2.Token == HotToken.SignParentheseLeft)
        {
            return MatchFunctionCall();
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
    /// functionCall ::= identifier '(' functionArguments ')'
    /// </summary>
    /// <returns></returns>
    private HotAst MatchFunctionCall()
    {
        var name = Match(HotToken.Identifier);
        Match(HotToken.SignParentheseLeft);
        var arguments = MatchFunctionArguments();
        Match(HotToken.SignParentheseRight);
        return new HotAstFunctionCall
        {
            Name = (name.Content as string)!,
            Arguments = arguments,
        };
    }

    /// <summary>
    /// assign ::= identifier '=' expression ';'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotAstAssign MatchAssign()
    {
        var id = Match(HotToken.Identifier);
        Match(HotToken.SignEqual);
        var expression = MatchExpression();
        Match(HotToken.SignSemicolon);
        return new HotAstAssign
        {
            Identifier = id.Content as string,
            Expression = expression,
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
            case HotToken.Identifier:
                return MatchAssign();
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
