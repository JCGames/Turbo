using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Scope;

public class DefineLocal : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "name",
            Location = Location.None
        },
        new IdentifierNode()
        {
            Text = "value",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters { get; } = ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count != 2)
        {
            throw Report.Error("Requires exactly two arguments.", function.Location);
        }

        if (arguments[0] is not IdentifierNode identifier)
        {
            throw Report.Error($"Expected {Parameters.First().PublicParameterName} to be an {nameof(IdentifierNode)}.", arguments[0].Location);
        }
        
        var identifierName = identifier.Text;
        
        var value = Runner.EvaluateNode(arguments[1], scope)
            .Cast<LispValue>(arguments[1].Location);
        
        scope.UpdateScope(identifierName, value);
        
        return LispVoidValue.Instance;
    }
}