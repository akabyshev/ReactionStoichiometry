namespace ReactionStoichiometry;

using System.Diagnostics;
using TDD;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        #if DEBUG
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var pass = TestParsing.Run() &&
                   TestBatchDetailedPlain.Run() &&
                   TestInstantiation.Run() &&
                   TestBatchVectors.Run();
        if (!pass) throw new InvalidOperationException("Tests failed");
        stopwatch.Stop();
        Debug.WriteLine($"Tests look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
        #endif

        Application.Run(new MainForm());
    }
}