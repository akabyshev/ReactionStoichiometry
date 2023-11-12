#define RUN_BASIC_TESTS

using System.Diagnostics;
using ReactionStoichiometryCLI;

#if RUN_BASIC_TESTS
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    TestBasicParsing.Run();
    TestInstantiation.Run();
    TestVectors.Run();
    TestDetailedPlain.Run();
    stopwatch.Stop();
    Console.WriteLine($"Batch processing look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
}
#endif