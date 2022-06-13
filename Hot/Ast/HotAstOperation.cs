using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstOperation : HotAst
{
    public static readonly Dictionary<HotToken, int> Priorities = new Dictionary<HotToken, int>()
    {
        { HotToken.SignPlus, 1 },
        { HotToken.SignMinus, 1 },
        { HotToken.SignStar, 2 },
        { HotToken.SignSlash, 2 },
    };

    public static readonly HashSet<HotToken> Rightists = new HashSet<HotToken>();

    public HotLexeme? Operation { get; set; }
    
    public HotAst? Left { get; set; }
    public HotAst? Right { get; set; }

    /// <summary>
    /// 根据优先级和结合性调整
    /// </summary>
    public static HotAstOperation Adjust(HotAstOperation root)
    {

        return root;
    }

    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine(Operation!.ToString());
        sb.AppendLine(Left!.Explain(indent + 4));
        sb.AppendLine(Right!.Explain(indent + 4));
        return sb.ToString();
    }
}
