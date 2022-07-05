using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstBreak : HotAst
{
    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine("[break]");
        return sb.ToString();
    }
}
