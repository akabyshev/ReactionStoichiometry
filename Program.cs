namespace ReactionStoichiometry;

using Rationals;

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

        //var a = Utils.ScaleRationals(new Rational[] {new Rational(1, 3), new Rational(2, 5), new Rational(7, 10)});
        //foreach (var bigInteger in a)
        //{
        //    Console.WriteLine(bigInteger.ToString());
        //}

        Application.Run(new FormMain());
    }
}
