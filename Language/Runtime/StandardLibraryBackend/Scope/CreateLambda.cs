using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Scope;


public class CreateLambda : ITurboFunction
{
    public bool IsVariadic { get; set; }
    public int RequiredArguments { get; set; }
    public int MaximumAllowedArguments { get; set; }

    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "arguments",
            Location = Location.None
        },
        new IdentifierNode()
        {
            Text = "body",
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

        if (arguments[0] is not ListNode argNodeList)
        {
            throw Report.Error("Expected the first argument to be a list.", arguments[0].Location);
        }
        
        var argList = argNodeList.Nodes
            .OfType<IParameterNode>()
            .ToList();

        if (argList.Count != argNodeList.Nodes.Count)
        {
            Report.Error("Expected all parameters to be identifiers.", arguments[0].Location);
        }

        var rawBody = arguments[1..];
        var body = rawBody.OfType<ListNode>().ToList();
        if (body.Count != rawBody.Count)
        {
            Report.Error("Each item in the body must be a list.", arguments[1].Location);
        }

        return new LispFunctionValue(function, argList, body);
    }
}