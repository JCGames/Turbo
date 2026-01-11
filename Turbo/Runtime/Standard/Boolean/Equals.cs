using Turbo.Language.Diagnostics;
using Turbo.Language.Diagnostics.Reports;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.Standard.Boolean;

public class Equals : ITurboFunction
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
    
    public BaseLispValue Execute(Node function, List<Node> arguments, LispScope scope)
    {
        if (arguments.Count < 2) Report.Error(new WrongArgumentCountReportMessage(Parameters, arguments.Count, 2), function.Location);
        
        var left = Runner.EvaluateNode(arguments[0], scope);
        var right = Runner.EvaluateNode(arguments[1], scope);
        
        return new LispBooleanValue(left.Equals(right));
    }
}