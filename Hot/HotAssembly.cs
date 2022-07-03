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
    public Dictionary<string, TypeBuilder> FunctionDefines { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="root"></param>
    public HotAssembly(HotAstModuleDefine root)
    {
        Name = new AssemblyName(root.Name);
        Builder = AssemblyBuilder.DefineDynamicAssembly(Name, AssemblyBuilderAccess.RunAndCollect);
        Module = Builder.DefineDynamicModule(root.Name);
        Main = Module.DefineType($"{root.Name}.Main");
        FunctionDefines = new Dictionary<string, TypeBuilder>();

        foreach (var s in root.Body)
        {
            if (s is HotAstVariableDefine)
            {
                var vd = s as HotAstVariableDefine;
                if (vd!.Expression is HotAstFunctionDefine)
                {
                    var b = EmitFunctionDefine((vd!.Expression as HotAstFunctionDefine)!);
                    b.CreateType();
                }
                else
                {

                }
            }
        }
        Main.CreateType();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fd"></param>
    /// <returns></returns>
    private TypeBuilder EmitFunctionDefine(HotAstFunctionDefine fd)
    {
        // 内部匿名闭包类
        var name = $"HotClosable__{Main.Name}__{FunctionDefines.Count}";
        var type = Main.DefineNestedType(
            name,
            TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit
        );
        FunctionDefines.Add(name, type);

        // 内部匿名闭包类方法
        var pts = fd!.Parameters!.Select(_ => typeof(object)).ToArray();
        var mname = $"{name}_Method";
        var method = type.DefineMethod(
            mname,
            MethodAttributes.Assembly | MethodAttributes.HideBySig,
            typeof(object),
            pts
        );

        var generator = method.GetILGenerator();
        if (fd.Body is HotAstBlock)
        {
            var body = fd.Body as HotAstBlock;
            foreach (var s in body!.Statements)
            {
                if (s is HotAstVariableDefine)
                {

                }
                else if (s is HotAstReturn)
                {

                }
                else if (s is HotAstAssign)
                {

                }
            }
        }
        else
        {

        }
        generator.Emit(OpCodes.Ret);

        return type;
    }
}
