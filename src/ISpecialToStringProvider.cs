namespace ReactionStoichiometry;

internal interface ISpecialToStringProvider
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