namespace ReactionStoichiometry;

using System.Diagnostics;

internal static class Program
{
    public const Double GOOD_ENOUGH_DOUBLE_ZERO = 1e-10;
    public const Int32 LETTER_LABEL_THRESHOLD = 7;
    public const String MULTIPLICATION_SYMBOL = "·";
    public const String FAILED_BALANCING_OUTCOME = "<FAIL>";

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();

        #if DEBUG
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        BasicTesting.PerformBasicParserTests();
        BasicTesting.PerformInstantiationTests();
        BasicTesting.PerformOnLaunchBatchTests();
        stopwatch.Stop();
        var elapsed = stopwatch.Elapsed;
        Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} milliseconds");
#endif

        Application.Run(new MainForm());
    }
}