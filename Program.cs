namespace ReactionStoichiometry;

using System.Diagnostics;

internal static class Program
{
    internal const Double GOOD_ENOUGH_FLOAT_PRECISION = 1e-10;
    internal const Int32 LETTER_LABEL_THRESHOLD = 7;
    internal const String MULTIPLICATION_SYMBOL = "·";
    internal const String FAILED_BALANCING_OUTCOME = "<FAIL>";

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
        if (!BasicTesting.PerformBasicParserTests() || !BasicTesting.PerformInstantiationTests() || !BasicTesting.PerformOnLaunchBatchTests())
            throw new InvalidOperationException("Tests failed");
        stopwatch.Stop();
        Debug.WriteLine($"Tests look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
        #endif

        Application.Run(new MainForm());
    }
}