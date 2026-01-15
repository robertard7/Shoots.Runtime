using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class RuntimeEngine : IRuntimeServices
{
    private readonly Dictionary<string, (IRuntimeModule Module, RuntimeCommandSpec Spec)> _index;

    public RuntimeEngine(IEnumerable<IRuntimeModule> modules)
    {
        _index = new Dictionary<string, (IRuntimeModule, RuntimeCommandSpec)>(StringComparer.OrdinalIgnoreCase);

        foreach (var m in modules)
        {
            foreach (var spec in m.Describe())
            {
                if (_index.ContainsKey(spec.CommandId))
                    throw new InvalidOperationException($"Command '{spec.CommandId}' already registered");

                _index[spec.CommandId] = (m, spec);
            }
        }
    }

    public RuntimeResult Execute(RuntimeRequest request, CancellationToken ct = default)
    {
        if (!_index.TryGetValue(request.CommandId, out var hit))
            return RuntimeResult.Fail(RuntimeError.UnknownCommand(request.CommandId));

        try
        {
            return hit.Module.Execute(request, ct);
        }
        catch (OperationCanceledException)
        {
            return RuntimeResult.Fail(RuntimeError.Internal("Execution cancelled"));
        }
        catch (Exception ex)
        {
            return RuntimeResult.Fail(RuntimeError.Internal("Unhandled exception", ex.ToString()));
        }
    }

    public IReadOnlyList<RuntimeCommandSpec> GetAllCommands() =>
        _index.Values.Select(v => v.Spec)
             .OrderBy(s => s.CommandId, StringComparer.OrdinalIgnoreCase)
             .ToArray();

    public RuntimeCommandSpec? GetCommand(string commandId)
    {
        if (string.IsNullOrWhiteSpace(commandId)) return null;
        return _index.TryGetValue(commandId, out var hit) ? hit.Spec : null;
    }
}
