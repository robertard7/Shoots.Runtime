using Shoots.Runtime.Abstractions;
using Shoots.Runtime.Core;

var host = new RuntimeEngine();

var ctx = new RuntimeContext(
    SessionId: Guid.NewGuid().ToString("N"),
    CorrelationId: Guid.NewGuid().ToString("N"),
    Env: new Dictionary<string, string>
    {
        ["machine"] = Environment.MachineName
    }
);

var req = new RuntimeRequest(
    CommandId: "core.ping",
    Args: new Dictionary<string, object?> { ["msg"] = "hello" },
    Context: ctx
);

var res = host.Execute(req);

Console.WriteLine($"Runtime v{host.Version} ok={res.Ok}");
if (!res.Ok)
{
    Console.WriteLine($"{res.Error?.Code}: {res.Error?.Message}");
}
else
{
    Console.WriteLine(res.Output);
}
