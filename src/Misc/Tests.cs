namespace ReactionStoichiometry
{
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
            var inputFilePath = @"data\parser_tests.txt";

            if (!File.Exists(inputFilePath))
                return;

            using StreamReader reader = new(inputFilePath);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split("\t");
                Assert_StringsAreEqual(Helpers.UnfoldFragment(parts[0]), parts[1]);
            }
        }

        public static void BalanceEquationsFromFile()
        {
            var inputFilePath = @"data\eqs-input.txt";
            var outputFilePath = @"data\eqs-output.txt";

            if (!File.Exists(inputFilePath))
                return;

            using StreamReader reader = new(inputFilePath);
            using StreamWriter writer = new(outputFilePath);
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("@"))
                {
                    writer.WriteLine(line);
                }
                if (line.StartsWith("EQ: "))
                {
                    var eq = line.Replace("EQ:", string.Empty);
                    writer.WriteLine(Helpers.SimpleStackedOutput(new BalancerThorne(eq)));
                    writer.WriteLine("----");
                    writer.WriteLine(Helpers.SimpleStackedOutput(new BalancerRisteskiRational(eq)));
                    writer.WriteLine("====================================");
                    writer.WriteLine();
                }
            }
        }
    }
}
