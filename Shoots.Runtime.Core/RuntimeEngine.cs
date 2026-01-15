using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class RuntimeEngine : IRuntimeHost
{
    public RuntimeVersion Version { get; } = new(0, 1, 0);

    public RuntimeResult Execute(RuntimeRequest request, CancellationToken ct = default)
    {
        if (request is null) return RuntimeResult.Fail("null_request", "Request cannot be null.");
        if (string.IsNullOrWhiteSpace(request.CommandId))
            return RuntimeResult.Fail("invalid_command", "CommandId cannot be empty.");

        // Placeholder behavior until modules are wired in
        return RuntimeResult.Success(new
        {
            request.CommandId,
            Args = request.Args,
            request.Context.SessionId,
            request.Context.CorrelationId
        });
    }
}
