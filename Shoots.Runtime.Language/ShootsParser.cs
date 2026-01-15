using Shoots.Runtime.Abstractions;
using System.Text;

namespace Shoots.Runtime.Language;

public static class ShootsParser
{
    public static RuntimeRequest Parse(
        string input,
        RuntimeContext context)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Empty command");

        var tokens = Tokenize(input);
        var commandId = tokens[0];

        var args = new Dictionary<string, object?>();

        for (int i = 1; i < tokens.Count; i++)
        {
            var part = tokens[i];
            var idx = part.IndexOf('=');
            if (idx <= 0) continue;

            var key = part[..idx];
            var value = part[(idx + 1)..];

            args[key] = value;
        }

        return new RuntimeRequest(commandId, args, context);
    }

    private static List<string> Tokenize(string input)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool quoted = false;

        foreach (var c in input)
        {
            if (c == '"')
            {
                quoted = !quoted;
                continue;
            }

            if (char.IsWhiteSpace(c) && !quoted)
            {
                if (sb.Length > 0)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length > 0)
            result.Add(sb.ToString());

        return result;
    }
}
