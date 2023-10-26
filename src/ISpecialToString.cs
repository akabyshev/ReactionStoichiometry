namespace ReactionStoichiometry;

internal interface ISpecialToString
{
    internal enum OutputFormat
    {
        Plain,
        Html
    }

    internal string ToString(OutputFormat format);
}