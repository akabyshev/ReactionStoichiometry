namespace ReactionStoichiometry.CLI
{
    internal abstract class BatchProcessorSingleLine
    {
        internal static void Run()
        {
            var balancers = new[] { typeof(BalancerInverseBased), typeof(BalancerGeneralized) };

            foreach (var type in balancers)
            {
                using StreamReader reader = new(Common.ConstructPath(filename: "MyBatch"));
                using StreamWriter writer = new(Common.ConstructPath(nameof(BatchProcessorSingleLine), type.Name));

                while (reader.ReadLine() is { } line)
                {
                    if (line.StartsWith(Common.IGNORED_LINE_MARK) || line.Length == 0)
                    {
                        continue;
                    }
                    var balancer = (Balancer)Activator.CreateInstance(type, line)!;
                    balancer.Run();

                    writer.WriteLine(line);
                    writer.WriteLine(Common.CHAR_TAB + balancer.ToString(Balancer.OutputFormat.SingleLine));
                }
            }
        }
    }
}
