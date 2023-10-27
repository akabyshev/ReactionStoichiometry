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
        <title>ReactionStoichiometry</title>
        </head>
        <body>
        <h2>Input</h2>
        <pre>%Skeletal%</pre>
        <h3>Details</h3>
        <pre>%Details%</pre>
        <h2>Output</h2>
        <pre>%Outcome%</pre>
        <h3>Diagnostics</h3>
        <p>%Diagnostics%</p>
        </body>
        </html>
        ";
}