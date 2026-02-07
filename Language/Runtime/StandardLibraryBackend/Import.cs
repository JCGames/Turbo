using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend;

public class Import : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "path",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count != 1)
        {
            throw Report.Error("Requires exactly one argument.", function.Location);
        }
        
        var result = Runner.EvaluateNode(arguments[0], scope)
            .Cast<LispStringValue>(arguments[0].Location);

        if (arguments[0] is not TokenNode token)
        {
            throw Report.Error("Import only accepts a token as its argument.", arguments[0].Location);
        }
        
        var path = Path.Join(token.Location.SourceFile?.FileInfo?.DirectoryName ?? Directory.GetCurrentDirectory(), result.Value);
        
        var sourceFile = new SourceFile(new FileInfo(path));
        var parser = new Parser(sourceFile);
        var lispListList = parser.ParseFile();

        foreach (var lispList in lispListList)
        {
            Runner.EvaluateNode(lispList, scope);
        }
        
        return LispVoidValue.Instance;
    }
}