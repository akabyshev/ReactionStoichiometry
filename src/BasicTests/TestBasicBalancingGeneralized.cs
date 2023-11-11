namespace ReactionStoichiometry;

internal class TestBasicBalancingGeneralized: BasicTest
{
    internal static void Run()
    {
        using StreamReader reader = new(ConstructPath(nameof(TestBasicBalancingGeneralized)));
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(CHAR_TAB);
            var balancer = new BalancerGeneralized(parts[0]);
            balancer.Run();
            AssertStringsAreEqual(balancer.ToString(Balancer.OutputFormat.Vectors), parts[1]);
        }
    }
}
