using System.Text;

namespace Hot;

/// <summary>
/// 
/// </summary>
public class HotLexer : IDisposable
{
    private static readonly string signs = "+-*/=,:;()[]{}<>";

    private static readonly Dictionary<string, HotToken> keywords = new Dictionary<string, HotToken>()
    {
        { "mod", HotToken.KeywordMod },
        { "use", HotToken.KeywordUse },
        { "let", HotToken.KeywordLet },
        { "fn", HotToken.KeywordFn },
        { "ret", HotToken.KeywordRet },
        { "if", HotToken.KeywordIf },
        { "else", HotToken.KeywordElse },
        { "true", HotToken.KeywordTrue },
        { "false", HotToken.KeywordFalse },
    };

    private StreamReader? reader;

    public int Row { get; private set; }
    public int Column { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public HotLexer(Stream stream)
    {
        reader = new StreamReader(stream);
        Row = 1;
        Column = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (reader != null)
        {
            reader.Dispose();
            reader = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    public HotLexeme PopLexeme()
    {
        int c = Skip(PopChar());

        // 结束符
        if (c == -1)
        {
            return NewLexeme(HotToken.EOF, c);
        }

        // 字符串
        if (c == '"')
        {
            return PopString(c);
        }

        // 符号
        if (signs.Contains((char)c))
        {
            return PopSign(c);
        }

        // 数字
        if (char.IsDigit((char)c))
        {
            return PopNumber(c);
        }


        // 标识符
        if (char.IsLetter((char)c))
        {
            return PopIdentifier(c);
        }

        throw new HotException($"词法错误[{Row},{Column}]，未知的字符 {(char)c}({c})");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotLexeme PopString(int c)
    {
        StringBuilder sb = new StringBuilder();
        int n = reader!.Peek();
        while(n != '"')
        {
            // 转义处理
            if (c == '\\')
            {
                c = PopChar();
                switch(c)
                {
                    case '0':
                        c = '\0';
                        break;
                    case 't':
                        c = '\t';
                        break;
                    case 'n':
                        c = '\n';
                        break;
                    case 'r':
                        c = '\r';
                        break;
                    //case '\\':
                    //    c = '\\';
                    //    break;
                    //case '"':
                    //    c = '"';
                    //    break;
                    default:
                        throw new HotException($"词法错误[{Row},{Column}]，未知的转义 {(char)c}({c})");
                }
            }

            sb.Append((char)c);
            c = PopChar();
            n = reader.Peek();
        }
        sb.Append((char)n);
        return NewLexeme(HotToken.String, sb.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    /// <exception cref="HotException"></exception>
    private HotLexeme PopSign(int c)
    {
        switch ((char)c)
        {
            case '+':
                return NewLexeme(HotToken.SignPlus, "+");
            case '-':
                c = reader!.Peek();
                if (c == '>')
                {
                    PopChar();
                    return NewLexeme(HotToken.SignArrowRight, "->");
                }
                return NewLexeme(HotToken.SignMinus, "-");
            case '*':
                return NewLexeme(HotToken.SignStar, "*");
            case '/':
                return NewLexeme(HotToken.SignSlash, "/");
            case '=':
                return NewLexeme(HotToken.SignEqual, "=");
            case ',':
                return NewLexeme(HotToken.SignComma, ",");
            case ':':
                return NewLexeme(HotToken.SignColon, ":");
            case ';':
                return NewLexeme(HotToken.SignSemicolon, ";");
            case '(':
                return NewLexeme(HotToken.SignParentheseLeft, "(");
            case ')':
                return NewLexeme(HotToken.SignParentheseRight, ")");
            case '{':
                return NewLexeme(HotToken.SignBraceLeft, "{");
            case '}':
                return NewLexeme(HotToken.SignBraceRight, "}");
            case '[':
                return NewLexeme(HotToken.SignBracketLeft, "[");
            case ']':
                return NewLexeme(HotToken.SignBracketRight, "]");
            case '<':
                c = reader!.Peek();
                switch (c)
                {
                    case '-':
                        PopChar();
                        return NewLexeme(HotToken.SignArrowLeft, "<-");
                    case '=':
                        PopChar();
                        return NewLexeme(HotToken.SignLessEqual, "<=");
                    default:
                        return NewLexeme(HotToken.SignLess, "<");
                }
            case '>':
                c = reader!.Peek();
                if (c == '=')
                {
                    PopChar();
                    return NewLexeme(HotToken.SignGreaterEqual, ">=");
                }
                return NewLexeme(HotToken.SignGreater, ">");
        }
        throw new HotException($"词法错误[{Row},{Column}]，未知的符号 {(char)c}({c})");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private HotLexeme PopNumber(int c)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append((char)c);
        while (true)
        {
            c = reader!.Peek();
            if (char.IsDigit((char)c) || c == '.')
            {
                c = PopChar();
                sb.Append((char)c);
            }
            else
            {
                break;
            }
        }

        return NewLexeme(HotToken.Number, decimal.Parse(sb.ToString()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private HotLexeme PopIdentifier(int c)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append((char)c);
        
        while (true)
        {
            c = reader!.Peek();
            if (char.IsLetterOrDigit((char)c))
            {
                c = PopChar();
                sb.Append((char)c);
            }
            else
            {
                break;
            }
        }

        string result = sb.ToString();

        if (keywords.ContainsKey(result))
        {
            return NewLexeme(keywords[result], result);
        }

        return NewLexeme(HotToken.Identifier, result);
    }

    /// <summary>
    /// 跳过
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private int Skip(int c)
    {
        // 跳过空白
        while (char.IsWhiteSpace((char)c))
        {
            c = PopChar();
        }

        // 跳过注释
        if (c == '#')
        {
            while (c != '\n')
            {
                c = PopChar();
            }
            return Skip(c);
        }
        return c;
    }

    private HotLexeme NewLexeme(HotToken token, object content)
    {
        return new HotLexeme(token, content, Row, Column);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int PopChar()
    {
        int c = reader!.Read();

        // 变 \r 和 \r\n 为 \n
        if (c == '\r')
        {
            int n = reader.Peek();
            if (n == '\n')
            {
                c = reader.Read();
            }
        }

        // 换行处理
        if (c == '\n')
        {
            ++Row;
            Column = 1;
        }
        else
        {
            ++Column;
        }

        return c;
    }
}
