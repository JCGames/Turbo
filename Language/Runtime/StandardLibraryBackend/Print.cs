using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend;

public class Print : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new RestIdentifierNode()
        {
            Text = "items",
            Location = Location.None
        },
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        foreach (var parameter in arguments)
        {
            var value = Runner.EvaluateNode(parameter, scope);
            Runner.StdOut.Write(value);
        }
        
        Runner.StdOut.WriteLine();
        
        return LispVoidValue.Instance;
    }
}