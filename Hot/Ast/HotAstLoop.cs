using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstLoop : HotAst
{
    public string? Iterator { get; set; }
    public HotAst? Collection { get; set; }

    public HotAstBlock? Body { get; set; }

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine("[for]");
        if (Iterator != null && Collection != null)
        {
            sb.AppendLine(Iterator);
            sb.AppendLine(Collection!.Explain(indent + 4));
        }
        sb.AppendLine(Body!.Explain(indent + 4));
        return sb.ToString();
    }
}

