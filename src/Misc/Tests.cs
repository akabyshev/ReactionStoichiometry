namespace ReactionStoichiometry;

internal static class Tests
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
            AssertStringsAreEqual(Helpers.UnfoldFragment(parts[0]), parts[1]);
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
            using StreamReader reader = new(inputFilePath);

            var path = @"..\..\..\data\output-" + type.Name + ".txt";
            var writer = new OutputWriter();

            while (reader.ReadLine() is { } line)
            {
                if (!line.StartsWith("EQ: "))
                    continue;
                var arguments = new object[] { line.Replace("EQ:", string.Empty) };
                var balancer = (IBalancer)Activator.CreateInstance(type, arguments)!;
                writer.WritePlainText(balancer);
                writer.WriteLine("====================================");
            }

            writer.SaveTo(path);
        }
    }
}