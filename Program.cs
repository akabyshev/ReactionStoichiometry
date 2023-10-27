namespace ReactionStoichiometry;

internal static class Program
{
    public const Double DOUBLE_PSEUDOZERO = 1e-10;
    public const Int32 LETTER_LABEL_THRESHOLD = 7;
    public const String MULTIPLICATION_SYMBOL = "·";

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        BasicTesting.PerformParsingTests();
        BasicTesting.PerformOnLaunchBatchTests();
        Application.Run(new ReactionStoichiometry.GUI.MainForm());
    }
}