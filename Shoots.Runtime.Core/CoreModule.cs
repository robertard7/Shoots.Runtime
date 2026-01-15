using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class CoreModule : IRuntimeModule
{
    public string ModuleId => "core";
    public RuntimeVersion ModuleVersion => new(0, 1, 0);

    public IReadOnlyList<RuntimeCommandSpec> Describe() =>
        new[]
        {
            new RuntimeCommandSpec(
                "core.ping",
                "Health check. Returns pong=true and echoes msg if provided.",
                new[]
                {
                    new RuntimeArgSpec("msg", "string", false, "Optional message")
                }
            )
        };

    public RuntimeResult Execute(RuntimeRequest request, CancellationToken ct = default)
    {
        if (request.CommandId.Equals("core.ping", StringComparison.OrdinalIgnoreCase))
        {
            return RuntimeResult.Success(new
            {
                pong = true,
                echo = request.Args.TryGetValue("msg", out var v) ? v?.ToString() ?? "" : ""
            });
        }

        return RuntimeResult.Fail("unknown_command", request.CommandId);
    }
}
