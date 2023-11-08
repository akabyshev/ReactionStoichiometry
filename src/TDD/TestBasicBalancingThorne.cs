namespace ReactionStoichiometry.TDD;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal static class TestBasicBalancingThorne
{
    internal static Boolean Run()
    {
        const String inputFilePath = @"..\..\..\data\TestBasicBalancingThorne.txt";
        if (!File.Exists(inputFilePath)) return false;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split("\t");
            var balancer = new BalancerThorne(parts[0]);
            balancer.Balance();
            Utils.AssertStringsAreEqual(balancer.ToString(OutputFormat.VectorsNotation), parts[1]);
        }

        return true;
    }
}
