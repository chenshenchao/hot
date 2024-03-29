﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstAccess : HotAst
{
    public string Name { get; set; } = null!;

    public HotAstAccessLocator? Locator { get; set; }
    public HotAstAccess? Access { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Name);

        if (Locator != null)
        {
            sb.Append(Locator.ToString());
        }

        if (Access != null)
        {
            sb.Append('.');
            sb.Append(Access.ToString());
        }

        return sb.ToString();
    }

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.Append("[access ");
        sb.Append(ToString());
        sb.Append("]");

        return sb.ToString();
    }
}
