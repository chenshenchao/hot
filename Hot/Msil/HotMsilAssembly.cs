using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hot.Msil;

public class HotMsilAssembly
{
    public AssemblyName Name { get; private set; }
    public AssemblyBuilder Builder { get; private set; }

    public HotMsilAssembly(string name)
    {
        Name = new AssemblyName(name);
        Builder = AssemblyBuilder.DefineDynamicAssembly(Name, AssemblyBuilderAccess.RunAndCollect);
    }
}
