using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstAssign : HotAst
{
    public string? Identifier { get; set; }
    public HotAst? Expression { get; set; }

    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("",new char[indent].Select(_ => ' ')));
        sb.AppendLine($"[{Identifier} = ]");
        sb.Append(Expression!.Explain(indent + 4));
        return sb.ToString();
    }
}
