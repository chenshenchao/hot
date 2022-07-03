using Hot;
using Lokad.ILPack;

using var stream = File.OpenRead("Examples/a.hot");
using var lexer = new HotLexer(stream);
//HotLexeme? lexeme = null;
//do
//{
//    lexeme = lexer.PopLexeme();
//    Console.WriteLine(lexeme);
//} while (lexeme.Token != HotToken.EOF);

using var parser = new HotParser(lexer);
var tree = parser.Parse();
var plain = tree.Explain();
Console.Write(plain);

var assembly = new HotAssembly(tree);
var generator = new AssemblyGenerator();
generator.GenerateAssembly(assembly.Builder, "hot.demo.1.dll");


//var interperter = new HotInterpreter();
//interperter.Interpret(stream);

Console.ReadKey();
