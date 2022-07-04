using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstAccess : HotAst
{
    public string Name { get; set; } = null!;
    public HotAstAccess? Access { get; set; }

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.Append("[access ");
        sb.Append(Name);
        sb.Append("]");
        if (Access != null)
        {
            sb.Append(Access.Explain(0));
        }
        return sb.ToString();
    }
}
