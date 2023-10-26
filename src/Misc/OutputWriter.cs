namespace ReactionStoichiometry;

internal class OutputWriter
{
    private readonly List<string> _strings = new();

    internal void SaveTo(string path)
    {
        File.WriteAllText(path, string.Join(Environment.NewLine, _strings));
    }

    internal void WritePlainText(IBalancer balancer)
    {
        WriteLine(balancer.SimpleStackedOutput());
    }

    internal void WriteLine(string line)
    {
        _strings.Add(line);
    }
}

internal static class OutputWriterExtensions
{
    public static string SimpleStackedOutput(this IBalancer b)
    {
        return string.Join(
            Environment.NewLine,
            new List<string>
            {
                "Skeletal:",
                b.Skeletal,
                string.Empty,
                "Details:",
                b.Details,
                string.Empty,
                "Outcome:",
                b.Outcome,
                string.Empty,
                "Diagnostics:",
                b.Diagnostics
            });
    }
}