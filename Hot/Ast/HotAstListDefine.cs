using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstListDefine : HotAst
{
    public List<HotAst> Items = null!;

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine("[list]");
        foreach (var item in Items)
        {
            sb.AppendLine(item.Explain(indent + 4));
        }
        return sb.ToString();
    }
}
