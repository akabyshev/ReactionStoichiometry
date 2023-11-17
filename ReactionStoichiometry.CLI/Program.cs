#define RUN_BATCH_PROCESSING

using System.Diagnostics;
using ReactionStoichiometry;
using ReactionStoichiometry.CLI;

#if RUN_BATCH_PROCESSING
var stopwatch = new Stopwatch();
stopwatch.Start();
// ReSharper disable once LoopCanBePartlyConvertedToQuery
foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
{
    BatchProcessor.Run(format);
}
stopwatch.Stop();
Console.WriteLine($"Batch processing look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

const String fileJson = @"D:\Solutions\ReactionStoichiometry\batchdata\Json-BalancerGeneralized.txt";
const String fileWrapped = @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.JsonViewer\wrapped_json.js";
var jsonData = "const jsonData =" + Environment.NewLine + File.ReadAllText(fileJson);
File.WriteAllText(fileWrapped, jsonData);
Console.WriteLine(value: "Wrapped Json-BalancerGeneralized.txt into wrapped_json.js");

Console.WriteLine(value: "---------------------------");
#endif

Console.WriteLine(value: "Equation?");
var equation = Console.ReadLine();
if (equation != null && ChemicalReactionEquation.IsValidString(equation))
{
    var b = new BalancerGeneralized(equation);
    b.Balance();
    Console.WriteLine(b.ToString(OutputFormat.DetailedMultiline));
}
