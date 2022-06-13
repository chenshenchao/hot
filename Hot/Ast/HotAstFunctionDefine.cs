using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstFunctionDefine : HotAst
{
    public List<string>? Parameters { get; set; }
    public HotAstBlock? Body { get; set; }

    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.Append("[fn](");
        sb.Append(string.Join(",", Parameters!));
        sb.AppendLine(") ->");
        sb.Append(Body!.Explain(indent + 4));
        return sb.ToString();
    }
}
