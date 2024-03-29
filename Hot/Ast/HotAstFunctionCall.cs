﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstFunctionCall : HotAst
{
    public HotAstAccess Access { get; set; } = null!;
    public List<HotAst> Arguments { get; set; } = null!;

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.Append("[call ");
        sb.Append(Access.Explain(0));
        sb.AppendLine("]");
        foreach (var arg in Arguments)
        {
            sb.AppendLine(arg.Explain(indent + 4));
        }
        return sb.ToString();
    }
}
