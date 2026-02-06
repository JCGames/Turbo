using Turbo.Language.Runtime.Types;

namespace Turbo.Language.Runtime.Standard;

/// <summary>
/// A Turbo Function is any natively implemented function.
/// Used for performance, special forms, etc.
/// </summary>
public interface ITurboFunction : IExecutableLispValue
{ }