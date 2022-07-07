using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstAccessLocator : HotAst
{
    public HotAst Expression { get; set; } = null!;

    public HotAstAccessLocator? Locator { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append('[');
        sb.Append(Expression.ToString());
        sb.Append(']');

        if (Locator != null)
        {
            sb.Append(Locator.ToString());
        }

        return sb.ToString();
    }

    public override string Explain(int indent = 0)
    {
        StringBuilder sb = new StringBuilder();

        var head = string.Join("", new char[indent].Select(_ => ' '));
        sb.Append(head);
        sb.AppendLine("[location begin");
        sb.Append(Expression.Explain(indent + 4));
        sb.Append(head);
        sb.AppendLine("]");

        if (Locator != null)
        {
            sb.Append(Locator.Explain(indent));
        }

        return sb.ToString();
    }
}
