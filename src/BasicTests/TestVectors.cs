namespace ReactionStoichiometry;

internal class TestVectors : BasicTest
{
    internal static void Run()
    {
        var balancers = new[] { typeof(BalancerInverseBased), typeof(BalancerGeneralized) };

        foreach (var type in balancers)
        {
            using StreamReader reader = new(ConstructPath(nameof(TestVectors)));
            using StreamWriter writer = new(ConstructPath(nameof(TestVectors), type.Name));

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0) continue;
                var balancer = (Balancer)Activator.CreateInstance(type, line.Replace(oldValue: " ", String.Empty))!;
                balancer.Run();

                writer.WriteLine(line);
                writer.WriteLine(CHAR_TAB + balancer.ToString(Balancer.OutputFormat.Vectors));
            }
        }
    }
}
