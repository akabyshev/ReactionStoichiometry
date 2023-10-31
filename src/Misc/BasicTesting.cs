namespace ReactionStoichiometry;

using System.Numerics;

internal static class BasicTesting
{
    private static void AssertStringsAreEqual(String lhs, String rhs)
    {
        if (lhs != rhs) throw new Exception($"{lhs} is not equal to {rhs}");
    }

    public static void PerformBasicParserTests()
    {
        const String inputFilePath = @"..\..\..\data\BasicParserTests.txt";
        if (!File.Exists(inputFilePath)) return;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0) continue;
            var parts = line.Split("\t");
            AssertStringsAreEqual(Parsing.Unfold(parts[0]), parts[1]);
        }
    }

    public static void PerformInstantiationTests()
    {
        const String inputFilePath = @"..\..\..\data\InstantiationTests.csv";
        if (!File.Exists(inputFilePath)) return;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0) continue;
            var parts = line.Split("\t");
            var eq = parts[0];

            var bRisteski = new BalancerRisteskiRational(eq);
            var bThorne = new BalancerThorne(eq);
            var hhSimple = bThorne.ToString(ISpecialToStringProvider.OutputFormat.OutcomeCommaSeparated);

            if (String.IsNullOrEmpty(parts[1])) continue;
            var instances = parts[1]
                            .Split(';')
                            .Select(static s => s.Trim('(', ')').Split(',').Select(BigInteger.Parse).ToArray())
                            .Select(parametersSet => bRisteski.Instantiate(parametersSet));

            AssertStringsAreEqual(hhSimple, String.Join(",", instances));
        }
    }

    public static void PerformOnLaunchBatchTests()
    {
        const String inputFilePath = @"..\..\..\data\OnLaunchBatch.txt";
        if (!File.Exists(inputFilePath)) return;

        var balancers = new[] { typeof(BalancerThorne), typeof(BalancerRisteskiDouble), typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            using StreamReader reader = new(inputFilePath);
            using StreamWriter writerFull = new($@"..\..\..\data\output-{type.Name}.txt");

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith("#") || line.Length == 0) continue;
                var balancer = (ISpecialToStringProvider)Activator.CreateInstance(type, line)!;

                writerFull.WriteLine(balancer.ToString(ISpecialToStringProvider.OutputFormat.Plain));
                writerFull.WriteLine("====================================");
            }
        }
    }
}