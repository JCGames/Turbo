using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;

namespace Turbo.Language.Runtime.Types;

public interface IExecutableLispValue
{
    public IEnumerable<IParameterNode> Parameters { get; }
    
    BaseLispValue Execute(Node function, List<Node> arguments, LispScope scope);
}