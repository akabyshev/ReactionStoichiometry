namespace ReactionStoichiometry.CLI
{
    internal abstract class BatchProcessor
    {
        private const String IGNORED_LINE_MARK = "#";

        internal static void Run()
        {
            foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
            {
                File.WriteAllText(ConstructPath(format.ToString(), str2: "BalancerGeneralized"), String.Empty);
                File.WriteAllText(ConstructPath(format.ToString(), str2: "BalancerInverseBased"), String.Empty);
            }

            using StreamWriter writerJson = new(path: "batch.json");
            writerJson.WriteLine(value: "{");
            writerJson.WriteLine(value: "\"serialized\": [");

            using StreamReader reader = new(ConstructPath(filename: @"input\MyBatch"));
            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                {
                    continue;
                }

                var equation = new ChemicalReactionEquation(line
                                                          , ChemicalReactionEquation.SolutionTypes.Generalized
                                                          | ChemicalReactionEquation.SolutionTypes.InverseBased);

                foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
                {
                    var filePathGeneralized = ConstructPath(format.ToString(), str2: "BalancerGeneralized");
                    var filePathInverseBased = ConstructPath(format.ToString(), str2: "BalancerInverseBased");

                    using StreamWriter writerGeneralized = new(filePathGeneralized, append: true);
                    using StreamWriter writerInverseBased = new(filePathInverseBased, append: true);

                    if (format != OutputFormat.DetailedMultiline)
                    {
                        writerGeneralized.WriteLine(line);
                        writerInverseBased.WriteLine(line);
                    }

                    writerGeneralized.Write(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).ToString(format));
                    writerGeneralized.WriteLine(Environment.NewLine);
                    writerInverseBased.Write(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.InverseBased).ToString(format));
                    writerInverseBased.WriteLine(Environment.NewLine);
                }

                writerJson.Write(equation.ToJson());
                writerJson.WriteLine(value: ',');
            }

            writerJson.WriteLine(value: ']');
            writerJson.WriteLine(value: '}');
        }

        private static String ConstructPath(String filename, String? str2 = null)
        {
            return @$"D:\Solutions\ReactionStoichiometry\batchdata\{filename + (str2 == null ? String.Empty : '-' + str2)}.txt";
        }
    }
}
