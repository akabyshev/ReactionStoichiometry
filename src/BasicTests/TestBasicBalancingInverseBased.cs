namespace ReactionStoichiometry;

internal class TestBasicBalancingInverseBased : BasicTest
{
    internal static void Run()
    {
        using StreamReader reader = new(ConstructPath(nameof(TestBasicBalancingInverseBased)));
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                continue;
            var parts = line.Split(CHAR_TAB);
            var balancer = new BalancerInverseBased(parts[0]);
            balancer.Run();
            AssertStringsAreEqual(balancer.ToString(Balancer.OutputFormat.Vectors), parts[1]);
        }
    }
}
