//#define RUN_BASIC_TESTS


namespace ReactionStoichiometryLegacyGUI;

file static class Program
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

        #if DEBUG && RUN_BASIC_TESTS
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            TestBasicParsing.Run();
            TestInstantiation.Run();
            TestVectors.Run();
            TestDetailedPlain.Run();
            stopwatch.Stop();
            Debug.WriteLine($"Tests look {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
        }
        #endif

        Application.Run(new FormMain());
    }
}
