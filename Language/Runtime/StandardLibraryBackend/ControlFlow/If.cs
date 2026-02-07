using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.ControlFlow;

public class If : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "condition",
            Location = Location.None
        },
        new IdentifierNode()
        {
            Text = "true-branch",
            Location = Location.None
        },
        new IdentifierNode()
        {
            Text = "false-branch",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count < 2)
        {
            throw Report.Error("Requires at least two arguments.", function.Location);
        }

        var condition = Runner.EvaluateNode(arguments[0], scope)
            .Cast<LispBooleanValue>(arguments[0].Location);

        if (condition.Value)
        {
            return Runner.EvaluateNode(arguments[1], scope);
        }
        
        if (arguments.Count >= 3)
        {
            return Runner.EvaluateNode(arguments[2], scope);
        }

        return LispVoidValue.Instance;
    }
}