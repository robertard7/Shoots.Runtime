using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class RuntimeEngine
{
    private readonly Dictionary<string, IRuntimeModule> _commandIndex;

    public RuntimeEngine(IEnumerable<IRuntimeModule> modules)
    {
        _commandIndex = new Dictionary<string, IRuntimeModule>(StringComparer.OrdinalIgnoreCase);

        foreach (var m in modules)
        {
            foreach (var cmd in m.Describe())
            {
                if (_commandIndex.ContainsKey(cmd.CommandId))
                {
                    throw new InvalidOperationException(
                        $"Command '{cmd.CommandId}' already registered");
                }

                _commandIndex[cmd.CommandId] = m;
            }
        }
    }

    public RuntimeResult Execute(RuntimeRequest request, CancellationToken ct = default)
    {
        if (!_commandIndex.TryGetValue(request.CommandId, out var module))
        {
            return RuntimeResult.Fail(
                RuntimeError.UnknownCommand(request.CommandId)
            );
        }

        try
        {
            return module.Execute(request, ct);
        }
        catch (OperationCanceledException)
        {
            return RuntimeResult.Fail(
                RuntimeError.Internal("Execution cancelled")
            );
        }
        catch (Exception ex)
        {
            return RuntimeResult.Fail(
                RuntimeError.Internal("Unhandled exception", ex.ToString())
            );
        }
    }
}
