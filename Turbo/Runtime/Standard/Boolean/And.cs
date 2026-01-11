using Turbo.Language.Diagnostics;
using Turbo.Language.Diagnostics.Reports;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.Standard.Boolean;

public class And : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode
        {
            Text = "items",
            Location = Location.None
        },
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;

    public BaseLispValue Execute(Node function, List<Node> arguments, LispScope scope)
    {
        if (arguments.Count < 2) Report.Error(new WrongArgumentCountReportMessage(Parameters, arguments.Count, 2), function.Location);

        foreach (var parameter in arguments)
        {
            var value = Runner.EvaluateNode(parameter, scope);
            
            if (value is not LispBooleanValue boolValue) throw Report.Error(new WrongArgumentTypeReportMessage("And expects it's arguments to be booleans."), parameter.Location);
            
            if (!boolValue.Value) return new LispBooleanValue(false);
        }

        return new LispBooleanValue(true);
    }
}