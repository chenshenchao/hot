using Hot.Ast;
using System.Runtime.Loader;

namespace Hot;

public class HotInterpreter
{
    public HotInterpreter()
    {
        
    }

    public void Interpret(Stream stream)
    {
        using var lexer = new HotLexer(stream);
        using var parser = new HotParser(lexer);
        HotAstModuleDefine tree = parser.Parse();
        Console.Write(tree.Explain());

        //var assembly = new HotAssembly(tree);
        
    }
}