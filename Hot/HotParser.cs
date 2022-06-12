using Hot.Ast;

namespace Hot;

/// <summary>
/// 语法分析器
/// </summary>
public class HotParser
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

    public HotAst Parse()
    {
        return MatchBlock(HotToken.EOF);
    }


    /// <summary>
    /// functionDefine ::=
    ///     fn identifier '(' functionParameters ')' '{' '}'
    /// </summary>
    /// <returns></returns>
    private HotAstFunctionDefine MatchFunctionDefine()
    {
        HotAstFunctionDefine result = new HotAstFunctionDefine();
        Match(HotToken.KeywordFn);
        var lexeme = Match(HotToken.Identifier);
        result.Name = lexeme.Content as string;
        Match(HotToken.SignParentheseLeft);

        MatchFunctionParameters();

        Match(HotToken.SignParentheseRight);
        Match(HotToken.SignBraceLeft);

        Match(HotToken.SignBraceRight);
        return result;
    }

    private void MatchFunctionParameters()
    {

    }

    private HotAst MatchBlock(HotToken limit)
    {
        var result = new List<HotAst>();
        while (true)
        {
            HotLexeme lexeme = PeekLexeme();
            if (lexeme.Token == limit)
            {
                break;
            }
            result.Add(MatchStatement());
        }
        return new HotAstBlock
        {
            Statements = result,
        };
    }

    /// <summary>
    /// 
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

    private HotAst MatchExpression()
    {
        return null;
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotAst MatchStatement()
    {
        var lexeme = PeekLexeme();
        switch (lexeme.Token)
        {
            case HotToken.KeywordFn:
                return MatchFunctionDefine();
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
    /// 
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
            int i = lexemes.Count;
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
