using System;

namespace Hot;

public class HotLexeme
{
    public HotToken Token;

    public object Content = null!;
    public int Row;
    public int Column;

    public HotLexeme(HotToken token, object content, int row, int column)
    {
        Token = token;
        Content = content;
        Row = row;
        Column = column;
    }

    public override string ToString()
    {
        return $"[{Row},{Column}] {Token} {Content}";
    }
}
