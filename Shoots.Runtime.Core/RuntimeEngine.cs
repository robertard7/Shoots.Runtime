using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class RuntimeEngine : IRuntimeHost
{
    private readonly Dictionary<string, (IRuntimeModule Module, RuntimeCommandSpec Spec)> _index =
        new(StringComparer.OrdinalIgnoreCase);

    public RuntimeVersion Version { get; } = new(0, 1, 0);

    public RuntimeEngine(IEnumerable<IRuntimeModule>? modules = null)
    {
        if (modules is null) return;

        foreach (var module in modules)
        {
            var specs = module.Describe();
            foreach (var spec in specs)
            {
                if (string.IsNullOrWhiteSpace(spec.CommandId))
                    continue;

                // Last write wins; keep it simple and deterministic
                _index[spec.CommandId] = (module, spec);
            }
        }
    }

    public RuntimeResult Execute(RuntimeRequest request, CancellationToken ct = default)
    {
        if (request is null)
            return RuntimeResult.Fail("null_request", "Request cannot be null.");

        if (string.IsNullOrWhiteSpace(request.CommandId))
            return RuntimeResult.Fail("invalid_command", "CommandId cannot be empty.");

        // Built-in introspection commands (engine-owned, always available)
        if (request.CommandId.Equals("core.help", StringComparison.OrdinalIgnoreCase))
            return RuntimeResult.Success(new
            {
                runtime = Version.ToString(),
                usage = "command.id arg=value arg=\"value with spaces\"",
                builtins = new[] { "core.help", "core.commands", "core.command" },
                hint = "Try: core.commands"
            });

        if (request.CommandId.Equals("core.commands", StringComparison.OrdinalIgnoreCase))
            return RuntimeResult.Success(new
            {
                commands = _index.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray()
            });

        if (request.CommandId.Equals("core.command", StringComparison.OrdinalIgnoreCase))
        {
            var target = TryGetStringArg(request, "id") ?? TryGetStringArg(request, "command");
            if (string.IsNullOrWhiteSpace(target))
                return RuntimeResult.Fail("missing_arg", "core.command requires id=<commandId>");

            if (!_index.TryGetValue(target, out var entry))
                return RuntimeResult.Fail("unknown_command", target);

            return RuntimeResult.Success(new
            {
                entry.Spec.CommandId,
                entry.Spec.Description,
                Args = entry.Spec.Args.Select(a => new { a.Name, a.Type, a.Required, a.Description }).ToArray()
            });
        }

        // Normal routing
        if (!_index.TryGetValue(request.CommandId, out var hit))
            return RuntimeResult.Fail("unknown_command", request.CommandId);

        try
        {
            return hit.Module.Execute(request, ct);
        }
        catch (Exception ex)
        {
            // Hard rule: never throw across the runtime boundary
            return new RuntimeResult(
                false,
                null,
                new RuntimeError("module_exception", ex.Message, ex.ToString())
            );
        }
    }

    private static string? TryGetStringArg(RuntimeRequest request, string key)
    {
        if (request.Args is null) return null;
        if (!request.Args.TryGetValue(key, out var v) || v is null) return null;
        return v.ToString();
    }
}
