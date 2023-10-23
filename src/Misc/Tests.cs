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
        const string inputFilePath = @"data\parser_tests.txt";

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
        const string inputFilePath = @"data\eqs-input.txt";
        const string outputFilePath = @"data\eqs-output.txt";

        if (!File.Exists(inputFilePath))
            return;

        using StreamReader reader = new(inputFilePath);
        using StreamWriter writer = new(outputFilePath);

        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("@"))
                writer.WriteLine(line);

            if (!line.StartsWith("EQ: "))
                continue;

            var eq = line.Replace("EQ:", string.Empty);
            writer.WriteLine(Helpers.SimpleStackedOutput(new BalancerThorne(eq)));
            writer.WriteLine("----");
            writer.WriteLine(Helpers.SimpleStackedOutput(new BalancerRisteskiRational(eq)));
            writer.WriteLine("====================================");
            writer.WriteLine();
        }
    }
}