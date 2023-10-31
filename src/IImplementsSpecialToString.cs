namespace ReactionStoichiometry;

internal interface IImplementsSpecialToString
{
    enum OutputFormat
    {
        Plain,
        Html
    }

    String ToString(OutputFormat format);
}