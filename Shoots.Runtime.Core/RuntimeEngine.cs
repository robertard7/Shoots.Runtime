using Shoots.Runtime.Abstractions;

namespace Shoots.Runtime.Core;

public sealed class RuntimeEngine : IRuntimeHost
{
    public RuntimeResult Execute(RuntimeInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Command))
        {
            return new RuntimeResult(
                false,
                null,
                new RuntimeError("invalid_command", "Command cannot be empty")
            );
        }

        return new RuntimeResult(
            true,
            $"Executed: {input.Command}",
            null
        );
    }
}
