namespace ReactionStoichiometry.CLI
{
    internal abstract class BatchProcessor
    {
        private const String IGNORED_LINE_MARK = "#";

        internal static void Run()
        {
            foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
            {
                File.WriteAllText(ConstructPath(format.ToString(), str2: "RowBased"), String.Empty);
                File.WriteAllText(ConstructPath(format.ToString(), str2: "ColumnsBased"), String.Empty);
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

                var equation = new ChemicalReactionEquation(line);

                foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
                {
                    using StreamWriter writerRowBasedSolution = new(ConstructPath(format.ToString(), str2: "RowBased"), append: true);
                    using StreamWriter writerColumnsBasedSolutions = new(ConstructPath(format.ToString(), str2: "ColumnsBased"), append: true);

                    if (format != OutputFormat.DetailedMultiline)
                    {
                        writerRowBasedSolution.WriteLine(line);
                        writerColumnsBasedSolutions.WriteLine(line);
                    }

                    writerRowBasedSolution.Write(equation.RowsBasedSolution.ToString(format));
                    writerRowBasedSolution.WriteLine(Environment.NewLine);
                    writerColumnsBasedSolutions.Write(equation.ColumnsBasedSolution.ToString(format));
                    writerColumnsBasedSolutions.WriteLine(Environment.NewLine);
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
