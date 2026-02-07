using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Boolean;

public class Or : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode
        {
            Text = "items",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;

    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count < 2)
        {
            throw Report.Error("Requires at least two arguments", function.Location);
        }

        var result = arguments
            .Select(argument => Runner.EvaluateNode(argument, scope).Cast<LispBooleanValue>(argument.Location))
            .Any(value => value.Value);
        
        return new LispBooleanValue(result);
    }
}