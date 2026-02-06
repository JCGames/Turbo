using Turbo.Language.Diagnostics;

namespace Turbo.Language.Parsing.Nodes.Classifications;

public interface INode
{
    public Location Location { get; set; }
}