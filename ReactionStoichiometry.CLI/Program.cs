#define RUN_BATCH_PROCESSING

using System.Diagnostics;
using ReactionStoichiometry;
using ReactionStoichiometry.CLI;

#if RUN_BATCH_PROCESSING
var stopwatch = new Stopwatch();
stopwatch.Start();
BatchProcessor.Run();
stopwatch.Stop();
Console.WriteLine($"Batch processing look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

const String fileWrapped = @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.JsonViewer\wrapped_json.js";
File.WriteAllText(fileWrapped, "const jsonData =" + Environment.NewLine + File.ReadAllText(path: "batch.json"));
Console.WriteLine(value: "Updated wrapped_json.js");

Console.WriteLine(value: "---------------------------");
#endif

Console.WriteLine(value: "Equation?");
var equationString = Console.ReadLine();
if (equationString != null && ChemicalReactionEquation.IsValidString(equationString))
{
    var equation = new ChemicalReactionEquation(equationString, ChemicalReactionEquation.SolutionTypes.Generalized);
    var solution = equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized);
    Console.WriteLine(solution.ToString(OutputFormat.DetailedMultiline));
    Console.WriteLine(equation.ToJson());
}
