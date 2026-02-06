using Turbo.Language.Parsing;

namespace Turbo.Tests;

[TestClass]
public sealed class SourceFileTests
{
    [TestMethod]
    public void TestGetLineSpan()
    {
        var sourceFile = new SourceFile("""
            Line 1
            Line 2
            """);
    
        Assert.AreEqual("Line 1", sourceFile.GetLineSpan(1).ToString());
        Assert.AreEqual("Line 2", sourceFile.GetLineSpan(2).ToString());
    }
    
    [TestMethod]
    public void TestGetSpan()
    {
        var sourceFile = new SourceFile("""
            This is a span
            """);
    
        Assert.AreEqual("This", sourceFile.GetSpan(0, 3).ToString());
    }
    
    [TestMethod]
    public void TestGetStartAndEndOfLine()
    {
        var sourceFile = new SourceFile("""
            This
            is
            a
            span
            """);
    
        var result = sourceFile.GetLineStartAndEnd(1);
        
        Assert.AreEqual(0, result.start);
        Assert.AreEqual(3, result.end);
    }
    
    [TestMethod]
    public void TestMoveToNextLine()
    {
        var sourceFile = new SourceFile("""
            This
            is
            a
            span
            """);
        sourceFile.MoveToNextLine();
        Assert.AreEqual(2, sourceFile.CurrentLine);
    }
}