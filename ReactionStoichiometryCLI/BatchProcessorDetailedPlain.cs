using ReactionStoichiometry;


namespace ReactionStoichiometryCLI
{
    internal abstract class BatchProcessorDetailedPlain : BatchProcessor
    {
        internal static void Run()
        {
            var balancers = new[] { typeof(BalancerInverseBased), typeof(BalancerGeneralized) };

            foreach (var type in balancers)
            {
                using StreamReader reader = new(ConstructPath(filename: "MyBatch"));
                using StreamWriter writer = new(ConstructPath(nameof(BatchProcessorDetailedPlain), type.Name));

                while (reader.ReadLine() is { } line)
                {
                    if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                    {
                        continue;
                    }
                    var balancer = (Balancer)Activator.CreateInstance(type, line.Replace(oldValue: " ", String.Empty))!;
                    balancer.Run();

                    writer.WriteLine(balancer.ToString(Balancer.OutputFormat.DetailedPlain));
                    writer.WriteLine(value: "====================================");
                }
            }
        }
    }
}
