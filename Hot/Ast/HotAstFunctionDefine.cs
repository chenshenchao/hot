using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public class HotAstFunctionDefine : HotAst
{
    public string? Name { get; set; }
    public List<string>? Parameters { get; set; }
    public List<HotAst>? Statements { get; set; }

    public override string Explain()
    {
        throw new NotImplementedException();
    }
}
