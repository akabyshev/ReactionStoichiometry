namespace ReactionStoichiometry;

internal interface ISpecialToStringProvider
{
    #region OutputFormat enum
    enum OutputFormat
    {
        Plain,
        OutcomeCommaSeparated,
        OutcomeNewLineSeparated,
        Html
    }
    #endregion

    String ToString(OutputFormat format);
}