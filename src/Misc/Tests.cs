using System;

namespace ReactionStoichiometry;

internal static class Tests
{
    private static void Assert_StringsAreEqual(string lhs, string rhs)
    {
        if (lhs != rhs)
        {
            throw new Exception($"{lhs} is not equal to {rhs}");
        }
    }

    public static void PerformParsingTests()
    {
        const string inputFilePath = @"..\..\..\data\parser_tests.txt";

        if (!File.Exists(inputFilePath))
            return;

        using StreamReader reader = new(inputFilePath);

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split("\t");
            Assert_StringsAreEqual(Helpers.UnfoldFragment(parts[0]), parts[1]);
        }
    }

    public static void BalanceEquationsFromFile()
    {
        const string inputFilePath = @"..\..\..\data\OnLaunch.txt";
        if (!File.Exists(inputFilePath))
            return;

        var balancers = new Type[] { typeof(BalancerThorne), typeof(BalancerRisteskiDouble), typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            using StreamReader reader = new(inputFilePath);

            var path = @"..\..\..\data\output-" + type.Name + ".txt";
            var writer = new OutputWriter(path);

            while (reader.ReadLine() is { } line)
            {
                if (!line.StartsWith("EQ: "))
                    continue;
                var arguments = new object[] { line.Replace("EQ:", string.Empty) };
                var balancer = (IBalancer) Activator.CreateInstance(type, arguments)!;
                writer.WritePlainText(balancer);
                writer.WriteLine("====================================");
            }
            writer.Save();
        }
    }
}