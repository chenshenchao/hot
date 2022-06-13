using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstModuleDefine : HotAst
{
    public string Name { get; set; } = null!;
    public List<HotAst> Body { get; set; } = null!;

    public override string Explain()
    {
        throw new NotImplementedException();
    }
}
