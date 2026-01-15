namespace Shoots.Runtime.Abstractions;

/// <summary>
/// Execution context provided by the host. Keep this minimal and stable.
/// </summary>
public sealed record RuntimeContext(
    string SessionId,
    string CorrelationId,
    IReadOnlyDictionary<string, string> Env
);
