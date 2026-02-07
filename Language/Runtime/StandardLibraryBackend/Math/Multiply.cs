using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Math;

public class Multiply : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "items",
            Location = Location.None
        },
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count < 2)
        {
            throw Report.Error("Requires at least two arguments.", function.Location);
        }

        var accum = arguments
            .Select(parameter => Runner.EvaluateNode(parameter, scope)
                .Cast<LispNumberValue>(parameter.Location))
            .Aggregate(1m, (current, value) => current * value.Value);

        return new LispNumberValue(accum);
    }
}