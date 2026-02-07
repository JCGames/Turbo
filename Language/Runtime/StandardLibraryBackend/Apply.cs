using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;
using Turbo.Language.Parsing.Nodes.Classifications;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.StandardLibraryBackend;

/// <summary>
/// Apply treats it's first argument as a function.
/// The remaining arguments will be treated as arguments to that function.
/// If any argument to apply is a list, its items will be used as arguments to the give function instead. 
/// </summary>
public class Apply : ITurboFunction
{
    private static readonly List<IParameterNode> ArgumentDeclaration =
    [
        new IdentifierNode()
        {
            Text = "function",
            Location = Location.None
        },
        new RestIdentifierNode()
        {
            Text = "arguments",
            Location = Location.None
        }
    ];

    public IEnumerable<IParameterNode> Parameters => ArgumentDeclaration;
    
    public BaseLispValue Execute(Node function, List<Node> arguments, Runtime.Scope scope)
    {
        if (arguments.Count < 2)
        {
            throw Report.Error("Requires at least two arguments.", function.Location);
        }
        
        var firstArg = Runner.EvaluateNode(arguments[0], scope);

        if (firstArg is not IExecutableLispValue executable)
        {
            throw Report.Error("Import expects its first argument to be a function.", arguments[0].Location);
        }

        var args = arguments.Skip(1).SelectMany(arg =>
        {
            var value = Runner.EvaluateNode(arg, scope);
            if (value is not LispValue lispValue) throw Report.Error($"{value} is not a value");
            if (value is LispListValue list)
            {
                return list.Value.Select(v => new DummyPreEvaluatedNode()
                {
                    Value = v,
                    Text = arg.ToString() ?? "pre-evaluated",
                    Location = arg.Location,
                });
            }

            return [new()
            {
                Value = lispValue,
                Text = arg.ToString() ?? "pre-evaluated",
                Location = arg.Location,
            }];
        })
            .OfType<Node>()
            .ToList();

        return executable.Execute(function, args, scope);
    }
}