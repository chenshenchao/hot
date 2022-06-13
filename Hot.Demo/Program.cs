using Hot;
using Hot.Ast;

using var stream = File.OpenRead("Examples/a.hot");
using var lexer = new HotLexer(stream);
//HotLexeme? lexeme = null;
//do
//{
//    lexeme = lexer.PopLexeme();
//    Console.WriteLine(lexeme);
//} while (lexeme.Token != HotToken.EOF);

using var parser = new HotParser(lexer);
HotAst tree = parser.Parse();
var plain = tree.Explain();
Console.Write(plain);

//var interperter = new HotInterpreter();
//interperter.Interpret();

Console.ReadKey();
