using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstBlock : HotAst
{
    public List<HotAst> Statements { get; set; } = new List<HotAst>();

    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in Statements)
        {
            sb.Append(item.Explain(indent + 4));
        }
        return sb.ToString();
    }
}
