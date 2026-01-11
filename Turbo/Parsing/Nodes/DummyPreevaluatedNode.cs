using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Parsing.Nodes;

public class DummyPreEvaluatedNode : TokenNode
{
    public required LispValue Value { get; set; }
}