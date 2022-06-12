using Hot;

using var stream = File.OpenRead("Examples/a.hot");
using var lexer = new HotLexer(stream);
HotLexeme? lexeme = null;
do
{
    lexeme = lexer.PopLexeme();
    Console.WriteLine(lexeme);
} while (lexeme.Token != HotToken.EOF);

//var interperter = new HotInterpreter();
//interperter.Interpret();

Console.ReadKey();
