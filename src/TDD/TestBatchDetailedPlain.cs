namespace ReactionStoichiometry.TDD;

internal static class TestBatchDetailedPlain
{
    private const String INPUT_FILE_PATH = @"..\..\..\data\TestBatchDetailedPlain.txt";

    internal static Boolean Run()
    {
        if (!File.Exists(INPUT_FILE_PATH)) return false;

        var balancers = new[] { typeof(BalancerThorne), typeof(BalancerRisteskiDouble), typeof(BalancerRisteskiRational) };

        foreach (var type in balancers)
        {
            using StreamReader reader = new(INPUT_FILE_PATH);
            using StreamWriter writer = new($@"..\..\..\data\{nameof(TestBatchDetailedPlain)}-{type.Name}.txt");

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith("#") || line.Length == 0) continue;
                var balancer = (Balancer)Activator.CreateInstance(type, line.Replace(" ", String.Empty))!;
                balancer.Balance();

                writer.WriteLine(balancer.ToString(Balancer.OutputFormat.DetailedPlain));
                writer.WriteLine("====================================");
            }
        }

        return true;
    }
}