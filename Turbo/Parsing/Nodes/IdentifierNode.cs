using Turbo.Language.Parsing.Nodes.Classifications;

namespace Turbo.Language.Parsing.Nodes;

public class IdentifierNode : TokenNode, IParameterNode
{
    string IParameterNode.PublicParameterName => Text;
}