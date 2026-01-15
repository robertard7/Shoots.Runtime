namespace Shoots.Runtime.Abstractions;

public sealed record RuntimeResult(
    bool Ok,
    object? Output,
    RuntimeError? Error
)
{
    public static RuntimeResult Success(object? output = null) => new(true, output, null);

    public static RuntimeResult Fail(string code, string message) =>
        new(false, null, new RuntimeError(code, message));
}
