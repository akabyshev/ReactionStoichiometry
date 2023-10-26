using ReactionStoichiometry.GUI;

namespace ReactionStoichiometry;

internal static class Program
{
    public const double DOUBLE_PSEUDOZERO = 1e-10;
    public const int LETTER_LABEL_THRESHOLD = 7;
    public const string MULTIPLICATION_SYMBOL = "·";

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Tests.PerformParsingTests();
        Tests.PerformOnLaunchBatchTests();
        Application.Run(new MainForm());
    }
}