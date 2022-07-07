using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

/// <summary>
/// 
/// </summary>
public class HotAstOperand : HotAst
{
    public HotLexeme? Sign { get; set; }
    public HotLexeme? Operand { get; set; }
    public HotAst? Expression { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (Sign != null)
        {
            sb.Append(' ');
            sb.Append(Sign.Content);
            sb.Append(' ');
        }

        if (Operand != null)
        {
            sb.Append(' ');
            sb.Append(Operand.Content);
            sb.Append(' ');
        }

        if (Expression != null)
        {
            sb.Append(' ');
            sb.Append(Expression.ToString());
            sb.Append(' ');
        }

        return sb.ToString();
    }

    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.Append("[");
        if (Sign != null)
        {
            sb.Append($" {Sign} ");
        }
        if (Operand != null)
        {
            sb.Append($" {Operand} ");
        }
        if (Expression != null)
        {
            sb.Append(Expression!.Explain(indent + 4));
        }
        sb.Append(']');
        return sb.ToString();
    }
}
