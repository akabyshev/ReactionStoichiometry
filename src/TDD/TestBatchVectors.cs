namespace ReactionStoichiometry.TDD;

internal static class TestBatchVectors
{
    private const String INPUT_FILE_PATH = @"D:\Solutions\ReactionStoichiometry\REFs\Batch\McMullen - 200 reactions\3-formatted.txt";

    internal static Boolean Run()
    {
        if (!File.Exists(INPUT_FILE_PATH)) return false;

        var balancers = new[] { typeof(BalancerThorne), /*typeof(BalancerRisteskiDouble), */typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            using StreamReader reader = new(INPUT_FILE_PATH);
            using StreamWriter writer = new($@"..\..\..\data\{nameof(TestBatchVectors)}-{type.Name}.txt");

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith("#") || line.Length == 0) continue;
                var balancer = (Balancer) Activator.CreateInstance(type, line.Replace(" ", String.Empty))!;
                balancer.Balance();

                writer.WriteLine(line);
                writer.WriteLine('\t' + balancer.ToString(Balancer.OutputFormat.VectorsNotation));
            }
        }

        return true;
    }
}