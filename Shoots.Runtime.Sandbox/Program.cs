using Shoots.Runtime.Abstractions;
using Shoots.Runtime.Core;
using Shoots.Runtime.Language;

// Create runtime host with sealed modules
var host = new RuntimeEngine(new IRuntimeModule[]
{
    new CoreModule()
});

// Create execution context (stable for session)
var ctx = new RuntimeContext(
    SessionId: Guid.NewGuid().ToString("N"),
    CorrelationId: Guid.NewGuid().ToString("N"),
    Env: new Dictionary<string, string>()
);

Console.WriteLine("Shoots Runtime Sandbox");
Console.WriteLine("Type commands (empty line to exit)");
Console.WriteLine();

// Interactive loop
while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(line))
        break;

    try
    {
        // Parse Shoots command language → RuntimeRequest
        var req = ShootsParser.Parse(line, ctx);

        // Execute via runtime
        var res = host.Execute(req);

        // Print result
        if (res.Ok)
            Console.WriteLine(res.Output);
        else
            Console.WriteLine($"error: {res.Error}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"fatal: {ex.Message}");
    }
}

