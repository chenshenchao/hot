using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstIf : HotAst
{
    public HotAst? Condition { get; set; }
    public HotAstBlock? Body { get; set; }
    public HotAst? ElseBlock{ get; set; }

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        string head = string.Join("", new char[indent].Select(_ => ' '));
        sb.Append(head);
        sb.AppendLine("[if]");
        sb.AppendLine(Condition!.Explain(indent + 4));
        sb.Append(head);
        sb.AppendLine("[then]");
        sb.AppendLine(Body!.Explain(indent + 4));
        if (ElseBlock != null)
        {
            sb.Append(head);
            sb.AppendLine("[else]");
            sb.AppendLine(ElseBlock!.Explain(indent + 4));
        }
        return sb.ToString();
    }
}
