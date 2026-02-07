using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;

namespace Turbo.Language.Runtime.Types;

public class LispListValue : LispValue, ICollectionLispValue<LispNumberValue>
{
    public List<LispValue> Value { get; set; } = new();

    public LispListValue()
    { }
    public LispListValue(ListNode listNode, Scope scope)
    {
        foreach (var item in listNode.Nodes)
        {
            var value = Runner.EvaluateNode(item, scope);
            if (value is not LispValue lispValue) throw Report.Error($"{value} cannot be stored in a struct", item.Location);
            
            Value.Add(lispValue);
        }
    }
    public Type KeyType => typeof(LispNumberValue);
    public LispValue First() => Value[0];

    public ICollectionLispValue<LispNumberValue> Rest() => new LispListValue()
    {
        Value = Value[1..]
    };

    public LispNumberValue Count() => new(Value.Count);

    public LispValue? GetValue(LispNumberValue key) => Value[(int)key.Value];
    
    public override string ToString() => "'(" + string.Join(", ", Value) + ")";

    protected override bool Equals(BaseLispValue other) => 
        other is LispListValue listValue 
        && listValue.Value.SequenceEqual(Value);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in Value)
        {
            hash.Add(value);
        }
        
        return hash.ToHashCode();
    }
}