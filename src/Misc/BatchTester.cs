namespace ReactionStoichiometry;

internal class BatchTester
{
    internal static void Run()
    {
        //return;
        const String pathR = @"D:\Solutions\ReactionStoichiometry\REFs\McMullen - 200 reactions\3-formatted.txt";
        const String pathW = @"..\..\..\data\BatchTester.txt";

        using StreamReader reader = new(pathR);
        using StreamWriter writer = new(pathW);

        while (reader.ReadLine() is { } line)
        {
            var balancer = new BalancerThorne(line);
            balancer.Balance();
            writer.WriteLine(line);
            writer.WriteLine('\t' + balancer.ToString(Balancer.OutputFormat.OutcomeVectorNotation));
        }
    }
}