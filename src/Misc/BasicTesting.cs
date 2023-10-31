namespace ReactionStoichiometry;

internal static class BasicTesting
{
    public static void PerformParsingTests()
    {
        const String inputFilePath = @"..\..\..\data\parser_tests.txt";

        if (!File.Exists(inputFilePath)) return;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split("\t");
            AssertStringsAreEqual(Parsing.Unfold(parts[0]), parts[1]);
        }

        return;

        static void AssertStringsAreEqual(String lhs, String rhs)
        {
            if (lhs != rhs) throw new Exception($"{lhs} is not equal to {rhs}");
        }
    }

    public static void PerformOnLaunchBatchTests()
    {
        const String inputFilePath = @"..\..\..\data\OnLaunchBatch.txt";
        if (!File.Exists(inputFilePath)) return;

        var balancers = new[] { typeof(BalancerThorne), typeof(BalancerRisteskiDouble), typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            var path = @"..\..\..\data\output-" + type.Name + ".txt";
            using StreamWriter writer = new(path);
            using StreamReader reader = new(inputFilePath);

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith("#") || line.Length == 0) continue;
                var eq = new Object[] { line };
                var balancer = (IImplementsSpecialToString)Activator.CreateInstance(type, eq)!;
                writer.WriteLine(balancer.ToString(IImplementsSpecialToString.OutputFormat.Plain));
                writer.WriteLine("====================================");
            }
        }
    }
}