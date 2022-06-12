using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstOperation : HotAst
{
    public HotLexeme? Operation { get; set; }
    public HotAst? Left { get; set; }
    public HotAst? Rigth { get; set; }

    public override string Explain()
    {
        throw new NotImplementedException();
    }
}
