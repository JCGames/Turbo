using Turbo.Language.Diagnostics;
using Turbo.Language.Diagnostics.Reports;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Runtime.Standard;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime;

public static class Runner
{
    public static TextWriter StdOut = Console.Out;
    public static TextReader StdIn = Console.In;
    public static void Run(List<Node> nodes)
    {
        var scope = new LispScope();
        InitializeGlobalScope(scope);
        
        foreach (var node in nodes)
        {
            if (node is ListNode list)
            {
                ExecuteList(list, scope);
            }
        }
    }

    public static BaseLispValue EvaluateNode(Node node, LispScope scope) => node switch
    {
        DummyPreEvaluatedNode preEvaluated => preEvaluated.Value, 
        ListNode list => list.IsQuoted ? new LispListValue(list, scope) : ExecuteList(list, scope),
        IdentifierNode identifier => scope.Read(identifier.Text) ?? throw Report.Error($"{identifier.Text} is undefined.", node.Location),
        NumberLiteralNode number => new LispNumberValue(number.Value),
        StringLiteralNode stringLiteral => new LispStringValue(stringLiteral.Text),
        StructNode structNode => new LispStructValue(structNode.Struct, scope),
        SymbolNode symbol => LispSymbolValue.New(symbol.Text),
        SingleLineCommentNode => LispVoidValue.Instance,
        _ => throw Report.Error("Incorrect or invalid syntax.", node.Location)
    };

    private static BaseLispValue ExecuteList(ListNode listNode, LispScope scope)
    {
        if (listNode.Nodes.Count == 0) throw new InvalidOperationException("Cannot execute an empty list.");

        var function = EvaluateNode(listNode.Nodes[0], scope);

        if (function is not IExecutableLispValue executable) throw Report.Error(new NotAFunctionReportMessage(), listNode.Location);
        
        return executable.Execute(listNode.Nodes[0], listNode.Nodes[1..], scope);
    }
    
    private static void InitializeGlobalScope(LispScope scope)
    {
        var turboFunctions = typeof(ITurboFunction).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ITurboFunction)))
            .Where(t => t != typeof(ITurboFunction));

        foreach (var turboFunction in turboFunctions)
        {
            var name = $"Turbo:{turboFunction.Name}";
            var value = new LispTurboFunctionValue((ITurboFunction)Activator.CreateInstance(turboFunction)!);
            
            scope.UpdateGlobalScope(name, value);
        }
        
        scope.UpdateGlobalScope("true", new LispBooleanValue(true));
        scope.UpdateGlobalScope("false", new LispBooleanValue(false));
    }
}