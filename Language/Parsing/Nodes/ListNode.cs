namespace Turbo.Language.Parsing.Nodes;

public class ListNode : Node
{
    public bool IsQuoted { get; set; }
    public List<Node> Nodes { get; set; } = [];

    public override void Print(string indent, TextWriter? writer = null)
    {
        Console.WriteLine($"{indent}(LIST, Is Quoted: {IsQuoted})");
        Console.WriteLine($"{indent}[");
        foreach (var node in Nodes)
        {
            node.Print(indent + '\t', writer);
        }
        Console.WriteLine($"{indent}]");
    }
}