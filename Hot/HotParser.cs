using Hot.Ast;

namespace Hot;

/// <summary>
/// 语法分析器
/// </summary>
public class HotParser : IDisposable
{
    private HotLexer lexer;
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

    public HotAst Parse()
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
    /// functionDefine ::= '(' functionParameters ')' '->' '{' block '}'
    /// </summary>
    /// <returns></returns>
    private HotAstFunctionDefine MatchFunctionDefine()
    {
        Match(HotToken.KeywordFn);
        // var id = Match(HotToken.Identifier);

        Match(HotToken.SignParentheseLeft);
        var parameters = MatchFunctionParameters();
        Match(HotToken.SignParentheseRight);

        Match(HotToken.SignArrowRight);

        var body = MatchBlock();

        return new HotAstFunctionDefine()
        {
            Parameters = parameters,
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
    /// expression ::= 
    ///     functionDefine
    ///     operand (('+'|'-'|'*'|'/') operand)*
    /// </summary>
    /// <returns></returns>
    private HotAst MatchExpression()
    {
        var fn = PeekLexeme();
        if (fn.Token == HotToken.KeywordFn)
        {
            return MatchFunctionDefine();
        }

        var operand = MatchOperand();
        var root = new HotAstOperation();
        root.Left = operand;
    loop:
        var lexeme = PeekLexeme();
        switch (lexeme.Token)
        {
            case HotToken.SignPlus:
            case HotToken.SignMinus:
            case HotToken.SignStar:
            case HotToken.SignSlash:
                var operation = PopLexeme();
                var right = MatchOperand();
                if (root.Operation is null)
                {
                    root.Operation = operation;
                    root.Right = right;
                }
                else
                {
                    var left = root.Right;
                    root.Right = new HotAstOperation
                    {
                        Left = left,
                        Operation = operation,
                        Right = right,
                    };
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
        return HotAstOperation.Adjust(root);
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
            int i = lexemes.Count - 1;
            var r = lexemes[i];
            lexemes.RemoveAt(i);
            return r;
        }
        return lexer.PopLexeme();
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
            var lexeme = lexer.PopLexeme();
            lexemes.Add(lexeme);
        }
        return lexemes[lexemes.Count - i];
    }
}
