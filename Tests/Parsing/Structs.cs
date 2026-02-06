using Turbo.Language.Parsing;

namespace Turbo.Tests.Parsing;

[TestClass]
public class Structs
{
    [TestMethod]
    public void NestedStructParsesSuccessfully()
    {
        var parser = new Parser(new SourceFile(
            """
            (def struct {
                a: "a"
                b: "b"
                test: (lambda (data) (print data))
                another-test: {
                    just-one-more-test: (lambda () (print "ok, this is crazy"))}})
            """));
        
        var result = parser.ParseFile();
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count != 0);
    }
}