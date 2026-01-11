using Turbo.Language.Diagnostics;
using Turbo.Language.Diagnostics.Reports;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.Standard.Collections;

public class Rest : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "collection",
            Location = Location.None
        },
    ];

    public IEnumerable<IParameterNode> Parameters =>  ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, LispScope scope)
    {
        if (arguments.Count != 1) throw Report.Error(new WrongArgumentCountReportMessage(Parameters, arguments.Count), function.Location);
        
        var value = Runner.EvaluateNode(arguments[0], scope);
        if (value is not ICollectionLispValue collection) throw Report.Error(new WrongArgumentTypeReportMessage("Rest expects it's argument to be a collection type"), function.Location);
        
        return (BaseLispValue)collection.Rest();
    }
}