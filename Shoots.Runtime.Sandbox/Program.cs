using Shoots.Runtime.Abstractions;
using Shoots.Runtime.Core;
using Shoots.Runtime.Loader;

var loader = new DefaultRuntimeLoader();

var modules = new IRuntimeModule[]
{
    new CoreModule()
};

var host = new RuntimeEngine(modules);

var ctx = new RuntimeContext(
    SessionId: Guid.NewGuid().ToString("N"),
    CorrelationId: Guid.NewGuid().ToString("N"),
    Env: new Dictionary<string, string>()
);

var req = new RuntimeRequest(
    CommandId: "core.ping",
    Args: new Dictionary<string, object?> { ["msg"] = "sealed" },
    Context: ctx
);

var res = host.Execute(req);

Console.WriteLine(res.Ok);
Console.WriteLine(res.Output);
