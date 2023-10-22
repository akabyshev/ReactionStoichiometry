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

        public static void RunParsingTests()
        {
            string inputFilePath = @"..\..\..\parser_tests.txt";

            if (!File.Exists(inputFilePath))
                return;

            using StreamReader reader = new(inputFilePath);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split("\t");
                Assert_StringsAreEqual(Helpers.UnfoldFragment(parts[0]), parts[1]);
            }
        }

        public static void RunTestsFromFile()
        {
            string inputFilePath = @"..\..\..\data\eqs-input.txt";
            string outputFilePath = @"..\..\..\data\eqs-output.txt";

            if (!File.Exists(inputFilePath))
                return;

            if (!File.Exists(outputFilePath))
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
                    string eq = line.Replace("EQ:", string.Empty);
                    writer.WriteLine(Helpers.SimpleStackedOutput(new ThorneBalancer(eq)));
                    writer.WriteLine("----");
                    writer.WriteLine(Helpers.SimpleStackedOutput(new RisteskiBalancer_Rational(eq)));
                    writer.WriteLine("====================================");
                    writer.WriteLine();
                }
            }
        }
    }
}
