namespace Shoots.Runtime.Abstractions;

public sealed record RuntimeError(
    string Code,
    string Message,
    string? Details = null
);
