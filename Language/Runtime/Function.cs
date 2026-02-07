using Turbo.Language.Parsing.Nodes;

namespace Turbo.Language.Runtime;

public class Function
{
    public required ListNode Definition;
    public required List<ListNode> Body;
}