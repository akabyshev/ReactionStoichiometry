namespace ReactionStoichiometry;

internal static class BasicTesting
{
    public static void PerformParsingTests()
    {
        const string inputFilePath = @"..\..\..\data\parser_tests.txt";

        if (!File.Exists(inputFilePath))
            return;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split("\t");
            AssertStringsAreEqual(Parsing.UnfoldFragment(parts[0]), parts[1]);
        }

        return;

        static void AssertStringsAreEqual(string lhs, string rhs)
        {
            if (lhs != rhs)
                throw new Exception($"{lhs} is not equal to {rhs}");
        }
    }

    public static void PerformOnLaunchBatchTests()
    {
        const string inputFilePath = @"..\..\..\data\OnLaunchBatch.txt";
        if (!File.Exists(inputFilePath))
            return;

        var balancers = new[]
            { typeof(BalancerThorne), typeof(BalancerRisteskiDouble), typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            var path = @"..\..\..\data\output-" + type.Name + ".txt";
            using StreamWriter writer = new(path);
            using StreamReader reader = new(inputFilePath);

            while (reader.ReadLine() is { } line)
            {
                if (!line.StartsWith("EQ: "))
                    continue;
                var arguments = new object[] { line.Replace("EQ:", string.Empty) };
                var balancer = (ISpecialToString)Activator.CreateInstance(type, arguments)!;
                writer.WriteLine(balancer.ToString(ISpecialToString.OutputFormat.Plain));
                writer.WriteLine("====================================");
            }
        }
    }
}