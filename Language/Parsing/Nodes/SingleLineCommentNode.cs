namespace Turbo.Language.Parsing.Nodes;

public class SingleLineCommentNode : TokenNode
{
    public override void Print(string indent, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        writer.WriteLine($"{indent}[{nameof(SingleLineCommentNode)}, |'{Text}|, {Location.Line}]");
    }
}