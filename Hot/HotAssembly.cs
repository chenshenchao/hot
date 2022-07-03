using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Hot.Ast;

namespace Hot;

/// <summary>
/// 
/// </summary>
public class HotAssembly
{
    public AssemblyName Name { get; private set; }
    public AssemblyBuilder Builder { get; private set; }
    public ModuleBuilder Module { get; private set; }
    public TypeBuilder Main { get; private set; }
    public Dictionary<string, TypeBuilder> FunctionDefine { get; private set; }

    public HotAssembly(HotAstModuleDefine root)
    {
        Name = new AssemblyName(root.Name);
        Builder = AssemblyBuilder.DefineDynamicAssembly(Name, AssemblyBuilderAccess.RunAndCollect);
        Module = Builder.DefineDynamicModule(root.Name);
        Main = Module.DefineType($"{root.Name}.Main");
        FunctionDefine = new Dictionary<string, TypeBuilder>();

        foreach (var s in root.Body)
        {
            if (s is HotAstVariableDefine)
            {
                var vd = s as HotAstVariableDefine;
                if (vd!.Expression is HotAstFunctionDefine)
                {
                    EmitFunctionDefine((vd!.Expression as HotAstFunctionDefine)!);
                }
                else
                {

                }
            }
        }
    }

    private TypeBuilder EmitFunctionDefine(HotAstFunctionDefine fd)
    {
        var name = $"{Main.FullName}_FC_{FunctionDefine.Count}";
        var type = Main.DefineNestedType(
            name,
            TypeAttributes.NestedPrivate | TypeAttributes.Sealed
        );

        var pts = fd!.Parameters!.Select(_ => typeof(object)).ToArray();
        var mname = $"{name}_Method";
        var method = type.DefineMethod(
            name,
            MethodAttributes.Public,
            typeof(object),
            pts
        );

        var generator = method.GetILGenerator();
        //foreach (var s in fd.Body!.Statements)
        //{

        //}
        return type;
    }
}
