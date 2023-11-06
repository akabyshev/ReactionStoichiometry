namespace ReactionStoichiometry;

using System.Numerics;

internal static class BasicTesting
{
    internal static Boolean PerformBasicParserTests()
    {
        const String inputFilePath = @"..\..\..\data\BasicParserTests.txt";
        if (!File.Exists(inputFilePath)) return false;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0) continue;
            var parts = line.Split("\t");
            AssertStringsAreEqual(ChemicalReactionEquation.UnfoldSubstance(parts[0]), parts[1]);
        }

        return true;
    }

    internal static Boolean PerformInstantiationTests()
    {
        const String inputFilePath = @"..\..\..\data\InstantiationTests.csv";
        if (!File.Exists(inputFilePath)) return false;

        using StreamReader reader = new(inputFilePath);
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

            AssertStringsAreEqual(hhSimple, String.Join(",", instances));
        }

        return true;
    }

    internal static Boolean PerformOnLaunchTests()
    {
        const String inputFilePath = @"..\..\..\data\OnLaunch.txt";
        if (!File.Exists(inputFilePath)) return false;

        var balancers = new[] { typeof(BalancerThorne), typeof(BalancerRisteskiDouble), typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            using StreamReader reader = new(inputFilePath);
            using StreamWriter writerFull = new($@"..\..\..\data\output-{type.Name}.txt");

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith("#") || line.Length == 0) continue;
                var balancer = (Balancer)Activator.CreateInstance(type, line.Replace(" ", String.Empty))!;
                balancer.Balance();

                writerFull.WriteLine(balancer.ToString(Balancer.OutputFormat.FullPlain));
                writerFull.WriteLine("====================================");
            }
        }

        return true;
    }

    private static void AssertStringsAreEqual(String lhs, String rhs)
    {
        if (lhs != rhs) throw new Exception($"{lhs} is not equal to {rhs}");
    }
}