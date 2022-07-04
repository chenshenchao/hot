using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstAssign : HotAst
{
    public HotAstAccess Access { get; set; } = null!;
    public HotAst? Expression { get; set; }

    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("",new char[indent].Select(_ => ' ')));
        sb.AppendLine($"[{Access.Explain(0)} = ]");
        sb.AppendLine(Expression!.Explain(indent + 4));
        return sb.ToString();
    }
}
