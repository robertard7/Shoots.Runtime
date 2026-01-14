namespace Shoots.Runtime.Abstractions;

public sealed record RuntimeResult(
    bool Success,
    object? Output,
    RuntimeError? Error
);
