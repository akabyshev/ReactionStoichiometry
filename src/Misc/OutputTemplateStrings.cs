namespace ReactionStoichiometry;

internal static class OutputTemplateStrings
{
    public const String PLAIN_OUTPUT = "Skeletal:\r\n%Skeletal%\r\n\r\n" +
                                       "Details:\r\n%Details%\r\n\r\n" +
                                       "Outcome:\r\n%Outcome%\r\n\r\n" +
                                       "Diagnostics:\r\n%Diagnostics%";

    public const String HTML_OUTPUT = @"<!DOCTYPE html>
        <html>
        <head>
        <title>Report</title>
        </head>
        <body>
        <h2>Skeletal</h2>
        <p>%Skeletal%</p>
        <h2>Details</h2>
        <pre>%Details%</pre>
        <h2>Outcome</h2>
        <p>%Outcome%</p>
        <h2>Diagnostics</h2>
        <p>%Diagnostics%</p>
        </body>
        </html>
        ";
}