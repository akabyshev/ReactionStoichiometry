namespace ReactionStoichiometry;

internal interface IImplementsSpecialToString
{
    enum OutputFormat
    {
        Plain,
        OutcomeCommaSeparated,
        OutcomeNewLineSeparated,
        Html
    }

    String ToString(OutputFormat format);
}