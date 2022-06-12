using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstReturn : HotAst
{
    public HotAst? Expression { get; set; }

    public override string Explain()
    {
        throw new NotImplementedException();
    }
}
