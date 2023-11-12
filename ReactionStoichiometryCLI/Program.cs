#define RUN_BASIC_TESTS

using System.Diagnostics;
using ReactionStoichiometry;
using ReactionStoichiometryCLI;

#if RUN_BASIC_TESTS
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    TestBasicParsing.Run();
    TestInstantiation.Run();
    BatchProcessorVectors.Run();
    BatchProcessorDetailedPlain.Run();
    stopwatch.Stop();
    Console.WriteLine($"Batch processing look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
}
#endif

Console.WriteLine(value: "---------------------------");
Console.WriteLine(value: "Equation?");
var equation = Console.ReadLine();
if (equation != null && StringOperations.SeemsFine(equation))
{
    var b = new BalancerGeneralized(equation);
    b.Run();
    Console.WriteLine(b.ToString(Balancer.OutputFormat.DetailedPlain));
}
