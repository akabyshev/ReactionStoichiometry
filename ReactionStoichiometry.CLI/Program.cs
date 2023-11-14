#define RUN_BATCH_PROCESSING

using System.Diagnostics;

using ReactionStoichiometry;
using ReactionStoichiometry.CLI;

#if RUN_BATCH_PROCESSING
var stopwatch = new Stopwatch();
stopwatch.Start();
// ReSharper disable once LoopCanBePartlyConvertedToQuery
foreach (Balancer.OutputFormat format in Enum.GetValues(typeof(Balancer.OutputFormat)))
{
    BatchProcessor.Run(format);
}
stopwatch.Stop();
Console.WriteLine($"Batch processing look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
Console.WriteLine(value: "---------------------------");
#endif

Console.WriteLine(value: "Equation?");
var equation = Console.ReadLine();
if (equation != null && equation.LooksLikeChemicalReactionEquation())
{
    var b = new BalancerGeneralized(equation);
    b.Run();
    Console.WriteLine(b.ToString(Balancer.OutputFormat.DetailedMultiline));
}
