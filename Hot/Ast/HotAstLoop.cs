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

    public HotAst? Condition { get; set; }

    public HotAstBlock? Body { get; set; }

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        var head = string.Join("", new char[indent].Select(_ => ' '));
        sb.Append(head);
        sb.Append("[loop] ");
        if (Iterator != null && Collection != null)
        {
            sb.Append(Iterator);
            sb.AppendLine(" [in] ");
            sb.AppendLine(Collection!.Explain(indent + 4));
        }
        else if (Condition != null)
        {
            sb.AppendLine("[while]");
            sb.AppendLine(Condition.Explain(indent + 4));
        }
        else
        {
            sb.AppendLine();
        }
        sb.Append(head);
        sb.AppendLine("[loop begin]");
        sb.AppendLine(Body!.Explain(indent + 4));
        sb.Append(head);
        sb.AppendLine("[loop end]");
        return sb.ToString();
    }
}

