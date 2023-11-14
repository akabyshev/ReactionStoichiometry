namespace ReactionStoichiometry.CLI
{
    internal abstract class BatchProcessor
    {
        private const String IGNORED_LINE_MARK = "#";

        private static String ConstructPath(String filename, String? str2 = null) =>
            @$"D:\Solutions\ReactionStoichiometry\batchdata\{filename + (str2 == null ? String.Empty : '-' + str2)}.txt";

        internal static void Run(Balancer.OutputFormat format)
        {
            var balancers = new[] { typeof(BalancerInverseBased), typeof(BalancerGeneralized) };

            foreach (var type in balancers)
            {
                using StreamReader reader = new(ConstructPath(filename: @"input\MyBatch"));
                using StreamWriter writer = new(ConstructPath(format.ToString(), type.Name));

                while (reader.ReadLine() is { } line)
                {
                    if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                    {
                        continue;
                    }
                    var balancer = (Balancer)Activator.CreateInstance(type, line)!;
                    balancer.Run();

                    writer.WriteLine(line);
                    writer.WriteLine(balancer.ToString(format));
                    writer.WriteLine(value: "----------------------------------------");
                }
            }
        }
    }
}
