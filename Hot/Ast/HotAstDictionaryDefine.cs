using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstDictionaryDefine : HotAst
{
    public List<KeyValuePair<HotAst, HotAst>> Items { get; set; } = null!;

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine("[dictionary]");
        foreach (var item in Items)
        {
            sb.Append(string.Join("", new char[indent + 4].Select(_ => ' ')));
            sb.AppendLine("[key]");
            sb.AppendLine(item.Key.Explain(indent + 4));
            sb.Append(string.Join("", new char[indent + 4].Select(_ => ' ')));
            sb.AppendLine("[value]");
            sb.AppendLine(item.Value.Explain(indent + 4));
        }
        return sb.ToString();
    }
}
