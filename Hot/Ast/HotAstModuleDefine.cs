using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstModuleDefine : HotAst
{
    public string Name { get; set; } = null!;
    public List<HotAst> Body { get; set; } = null!;

    public override string Explain(int indent=0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine($"[mod {Name}]");
        foreach (var item in Body)
        {
            sb.AppendLine(item.Explain(indent + 4));
        }
        return sb.ToString();
    }
}
