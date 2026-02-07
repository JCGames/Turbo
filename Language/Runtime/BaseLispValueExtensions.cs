using Turbo.Language.Diagnostics;
using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime;

public static class BaseLispValueExtensions
{
    public static T Cast<T>(this BaseLispValue value, Location location)
    {
        if (value is not T returnValue)
        {
            throw Report.Error($"Expected a {typeof(T).Name} but got {value}.", location);
        }

        return returnValue;
    } 
}