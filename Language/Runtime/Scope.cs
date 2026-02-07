using System.Collections.Immutable;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime;

public class Scope
{
    private ImmutableDictionary<string, LispValue> _scope = ImmutableDictionary<string, LispValue>.Empty;
    private Scope _global;
    private readonly Scope? _parent;

    public Scope()
    {
        _global = CreateGlobal();
    }

    private Scope(Scope? global, Scope? parent)
    {
        _global = global!;
        _parent = parent;
    }

    private static Scope CreateGlobal()
    {
        var global = new Scope(null, null);
        global._global = global;
        return global;
    }

    public Scope PushScope()
    {
        return new(_global, this);
    }

    public Scope PopScope()
    {
        return _parent ?? throw new InvalidOperationException("Cannot pop scope.");
    }

    public bool HasOwnValue(string identifier)
    {
        return _scope.ContainsKey(identifier);
    }
    
    public BaseLispValue? Read(string identifier)
    {
        var scope = this;
        LispValue? value;
        while (!scope._scope.TryGetValue(identifier, out value))
        {
            scope = scope._parent;
            if (scope == null) break;
        }

        if (value is null)
        {
            _global._scope.TryGetValue(identifier, out value);
        }
        
        return value;
    }

    public void UpdateScope(string identifier, LispValue value)
    {
        _scope = _scope.SetItem(identifier, value);
    }

    public void UpdateGlobalScope(string identifier, LispValue value)
    {
        _global.UpdateScope(identifier, value);
    }
}