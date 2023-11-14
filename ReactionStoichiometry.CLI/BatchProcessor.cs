namespace ReactionStoichiometry.CLI
{
    internal abstract class BatchProcessor
    {
        private const String IGNORED_LINE_MARK = "#";

        internal static void Run(Balancer.OutputFormat format)
        {
            var balancers = new[] { typeof(BalancerInverseBased), typeof(BalancerGeneralized) };

            foreach (var type in balancers)
            {
                using StreamReader reader = new(ConstructPath(filename: @"input\MyBatch"));
                using StreamWriter writer = new(ConstructPath(format.ToString(), type.Name));

                if (format == Balancer.OutputFormat.Json)
                {
                    writer.WriteLine(value: "{");
                    writer.WriteLine(value: "\"serialized\": [");
                }
                while (reader.ReadLine() is { } line)
                {
                    if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                    {
                        continue;
                    }
                    var balancer = (Balancer)Activator.CreateInstance(type, line)!;
                    balancer.Balance();

                    switch (format)
                    {
                        case Balancer.OutputFormat.Simple:
                            writer.WriteLine(line);
                            break;
                        case Balancer.OutputFormat.Multiline:
                            writer.WriteLine(line);
                            break;
                        case Balancer.OutputFormat.DetailedMultiline: break;
                        case Balancer.OutputFormat.Json: break;
                        default: throw new ArgumentOutOfRangeException(nameof(format));
                    }

                    writer.Write(balancer.ToString(format));
                    if (format != Balancer.OutputFormat.Json)
                        writer.WriteLine(Environment.NewLine);
                    else
                        writer.WriteLine(',');
                }
                if (format == Balancer.OutputFormat.Json)
                {
                    writer.WriteLine(value: ']');
                    writer.WriteLine(value: '}');
                }
            }
        }

        private static String ConstructPath(String filename, String? str2 = null)
        {
            return @$"D:\Solutions\ReactionStoichiometry\batchdata\{filename + (str2 == null ? String.Empty : '-' + str2)}.txt";
        }
    }
}
