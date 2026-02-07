using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Boolean;

public class Not : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode
        {
            Text = "item",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;

    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count != 1)
        {
            throw Report.Error("Requires exactly one arguments", function.Location);
        }

        var value = Runner.EvaluateNode(arguments[0], scope).Cast<LispBooleanValue>(arguments[0].Location);
        
        return new LispBooleanValue(!value.Value);
    }
}