using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing;
using Turbo.Language.Runtime;

Report.PreferThrownErrors = true;

var standardLibrary = new SourceFile(new FileInfo("Runtime/StandardLibrary/prelude.lisp"));
var sourceFile = new SourceFile(new FileInfo("main.txt"));

var parserStandardLibrary = new Parser(standardLibrary);
var parser = new Parser(sourceFile);

var standardLibraryNodes = parserStandardLibrary.ParseFile();
var nodes = parser.ParseFile();

foreach (var node in nodes)
{
    node.Print("", Console.Out);
}

Runner.Run([..standardLibraryNodes, ..nodes]);