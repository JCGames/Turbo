using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Math;

public class SubtractOrNegate : ITurboFunction
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
        if (arguments.Count != 1)
        {
            throw Report.Error("Requires exactly one argument.", function.Location);
        }

        var accum = GetValue(arguments[0], scope);
        
        if (arguments.Count == 1)
        {
            return new LispNumberValue(-accum);
        }

        accum = arguments[1..]
            .Select(parameter => GetValue(parameter, scope))
            .Aggregate(accum, (current, value) => current - value);

        return new LispNumberValue(accum);
    }

    private decimal GetValue(Node node, Runtime.Scope scope)
    {
        var value = Runner.EvaluateNode(node, scope)
            .Cast<LispNumberValue>(node.Location);
        
        return value.Value;
    }
}