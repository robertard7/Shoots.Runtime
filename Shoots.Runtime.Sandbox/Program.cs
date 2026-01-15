using System.Text.Json;
using Shoots.Runtime.Abstractions;
using Shoots.Runtime.Core;
using Shoots.Runtime.Language;

var host = new RuntimeEngine(new IRuntimeModule[]
{
    new CoreModule()
});

var ctx = new RuntimeContext(
    SessionId: Guid.NewGuid().ToString("N"),
    CorrelationId: Guid.NewGuid().ToString("N"),
    Env: new Dictionary<string, string>()
);

var json = new JsonSerializerOptions { WriteIndented = false };

Console.WriteLine("Shoots Runtime Sandbox");
Console.WriteLine("Commands: core.help | core.commands | core.command id=<cmd> | core.ping [msg=...]");
Console.WriteLine("Empty line exits.\n");

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
        break;

    try
    {
        var req = ShootsParser.Parse(line, ctx);
        var res = host.Execute(req);

        if (!res.Ok)
        {
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                ok = false,
                error = res.Error?.Code,
                message = res.Error?.Message,
                details = res.Error?.Details
            }, json));
            continue;
        }

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            ok = true,
            output = res.Output
        }, json));
    }
    catch (Exception ex)
    {
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            ok = false,
            error = "parse_error",
            message = ex.Message
        }, json));
    }
}
