using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstOperation : HotAst
{
    public static readonly Dictionary<HotToken, int> Priorities = new Dictionary<HotToken, int>()
    {
        { HotToken.SignAnd, 1 },
        { HotToken.SignOr, 1 },
        { HotToken.SignEqual, 2 },
        { HotToken.SignGreater, 2 },
        { HotToken.SignGreaterEqual, 2 },
        { HotToken.SignLess, 2 },
        { HotToken.SignLessEqual, 2 },
        { HotToken.SignPlus, 3 },
        { HotToken.SignMinus, 3 },
        { HotToken.SignStar, 4 },
        { HotToken.SignSlash, 4 },
    };

    public static readonly HashSet<HotToken> Rightists = new HashSet<HotToken>();

    private HotLexeme? operation = null;

    public HotLexeme? Operation {
        get => operation;
        set
        {
            operation = value;
            Level = Priorities[value!.Token];
            IsRightist = Rightists.Contains(value!.Token);
        }
    }
    
    public HotAstOperation? Top { get; set; }
    public HotAst? Left { get; set; }
    public HotAst? Right { get; set; }

    public int Level { get; private set; }
    public bool IsRightist { get; private set; }

    /// <summary>
    /// 根据优先级和结合性调整
    /// </summary>
    public static HotAstOperation Adjust(HotAstOperation one, HotAstOperation root)
    {

        while (true)
        {
            if (one.Top == null)
            {
                return one;
            }
            if (one.Level < one.Top.Level || (!one.IsRightist && one.Level == one.Top.Level))
            {
                var top = one.Top;
                one.Top = top.Top;
                top.Top = one;
                top.Right = one.Left;
                one.Left = top;
            }
            else
            {
                break;
            }
        }
        return root;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="indent"></param>
    /// <returns></returns>
    public override string Explain(int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Join("", new char[indent].Select(_ => ' ')));
        sb.AppendLine(Operation!.ToString());
        sb.AppendLine(Left!.Explain(indent + 4));
        sb.Append(Right!.Explain(indent + 4));
        return sb.ToString();
    }
}
