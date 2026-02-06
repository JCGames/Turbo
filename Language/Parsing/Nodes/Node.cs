using Turbo.Language.Diagnostics;

namespace Turbo.Language.Parsing.Nodes;

public abstract class Node
{
    public required Location Location { get; set; }
    
    public abstract void Print(string indent, TextWriter? writer = null);
}