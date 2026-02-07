using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Boolean;

public class GreaterThanOrEqualTo : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode
        {
            Text = "left",
            Location = Location.None
        },
        new IdentifierNode
        {
            Text = "right",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;

    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count != 2)
        {
            throw Report.Error("Requires exactly two arguments", function.Location);
        }

        var left = Runner.EvaluateNode(arguments[0], scope).Cast<LispNumberValue>(arguments[0].Location);
        var right = Runner.EvaluateNode(arguments[1], scope).Cast<LispNumberValue>(arguments[1].Location);
        
        return new LispBooleanValue(left.Value >= right.Value);
    }
}