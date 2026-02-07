using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing;

namespace Turbo.Tests;

[TestClass]
public class ParserTests
{
    [TestInitialize]
    public void InitMethod()
    {
        Report.PreferThrownErrors = true;
    }
    
    [TestMethod]
    public void TestStringLiteralParse()
    {
        var parser = new Parser(new SourceFile(
            """
            ("Hello, world")
            """
        ));
        
        var result = parser.ParseFile();

        result.First().Print("", Console.Out);
    }
}