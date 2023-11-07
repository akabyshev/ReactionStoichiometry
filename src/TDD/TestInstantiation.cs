using System.Numerics;

namespace ReactionStoichiometry.TDD;

internal static class TestInstantiation
{
    private const String INPUT_FILE_PATH = @"..\..\..\data\InstantiationTests.csv";

    internal static Boolean Run()
    {
        if (!File.Exists(INPUT_FILE_PATH)) return false;

        using StreamReader reader = new(INPUT_FILE_PATH);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0) continue;
            var parts = line.Split("\t");
            var eq = parts[0].Replace(" ", String.Empty);

            var bRisteski = new BalancerRisteskiRational(eq);
            var bThorne = new BalancerThorne(eq);

            bRisteski.Balance();
            bThorne.Balance();

            var hhSimple = bThorne.ToString(Balancer.OutputFormat.OutcomeOnlyCommas);

            if (String.IsNullOrEmpty(parts[1])) continue;
            var instances = parts[1]
                            .Split(';')
                            .Select(static s => s.Trim('(', ')').Split(',').Select(BigInteger.Parse))
                            .Select(parametersSet => bRisteski.Instantiate(parametersSet.ToArray()).readable);

            Utils.AssertStringsAreEqual(hhSimple, String.Join(",", instances));
        }

        return true;
    }
}