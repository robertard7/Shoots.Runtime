namespace Shoots.Runtime.Abstractions;

public interface IRuntimeHost
{
    RuntimeResult Execute(RuntimeInput input);
}
