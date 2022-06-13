using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Ast;

public abstract class HotAst
{
    public abstract string Explain(int indent=0);
}
