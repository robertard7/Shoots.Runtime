using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class RuntimeEngine : IRuntimeHost
{
    private readonly Dictionary<string, IRuntimeModule> _commands = new();

    public RuntimeVersion Version { get; } = new(0, 1, 0);

    public RuntimeEngine(IEnumerable<IRuntimeModule>? modules = null)
    {
        if (modules == null) return;

        foreach (var module in modules)
        {
            foreach (var spec in module.Describe())
            {
                _commands[spec.CommandId] = module;
            }
        }
    }

    public RuntimeResult Execute(RuntimeRequest request, CancellationToken ct = default)
    {
        if (!_commands.TryGetValue(request.CommandId, out var module))
            return RuntimeResult.Fail("unknown_command", request.CommandId);

        return module.Execute(request, ct);
    }
}
