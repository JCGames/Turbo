using Turbo.Language.Diagnostics;
using Turbo.Language.Diagnostics.Reports;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend.Boolean;

public class Not : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "item",
            Location = Location.None
        },
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, LispScope scope)
    {
        if (arguments.Count != 1) Report.Error(new WrongArgumentCountReportMessage(Parameters, arguments.Count), function.Location);
        
        var value = Runner.EvaluateNode(arguments[0], scope);
        if (value is not LispBooleanValue boolValue) throw Report.Error(new WrongArgumentTypeReportMessage("Not only works with boolean arguments"), arguments[0].Location);
        
        return new LispBooleanValue(!boolValue.Value);
    }
}