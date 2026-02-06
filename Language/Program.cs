using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing;
using Turbo.Language.Runtime;

Report.PreferThrownErrors = true;

// var standardLibrary = new SourceFile(new FileInfo("Runtime/StandardLibrary/prelude.lisp"));
var sourceFile = new SourceFile(new FileInfo("main.txt"));

// var parserStandardLibrary = new Parser(standardLibrary);
var parser = new Parser(sourceFile);

// var standardLibraryList = parserStandardLibrary.ParseFile();
var list = parser.ParseFile();

foreach (var listNode in list)
{
    listNode.Print("", Console.Out);
}

// Runner.Run([..standardLibraryList, ..list]);

Runner.Run(list);