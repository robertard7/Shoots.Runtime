using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public static class ModuleRegistryBuilder
{
    public static RuntimeResult? TryBuild(
        IReadOnlyList<IRuntimeModule> modules,
        out IReadOnlyList<IRuntimeCommand> commands)
    {
        commands = Array.Empty<IRuntimeCommand>();

        if (modules is null)
        {
            return RuntimeResult.Fail(new RuntimeError(
                "modules_null",
                "Module list was null."));
        }

        var list = new List<IRuntimeCommand>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var module in modules)
        {
            if (module is null)
            {
                return RuntimeResult.Fail(new RuntimeError(
                    "module_null",
                    "A null module instance was provided."));
            }

            // Assumes IRuntimeModule exposes Commands.
            foreach (var cmd in module.Commands)
            {
                if (cmd is null)
                {
                    return RuntimeResult.Fail(new RuntimeError(
                        "command_null",
                        "A module provided a null command."));
                }

                if (string.IsNullOrWhiteSpace(cmd.Id))
                {
                    return RuntimeResult.Fail(new RuntimeError(
                        "command_id_empty",
                        "A command with an empty id was provided."));
                }

                if (!seen.Add(cmd.Id))
                {
                    return RuntimeResult.Fail(new RuntimeError(
                        "duplicate_command_id",
                        $"Duplicate command id detected: '{cmd.Id}'."));
                }

                list.Add(cmd);
            }
        }

        commands = list;
        return null;
    }
}
