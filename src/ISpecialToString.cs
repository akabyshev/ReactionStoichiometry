namespace ReactionStoichiometry;

internal interface ISpecialToString
{
    internal String ToString(OutputFormat format);

    internal enum OutputFormat
    {
        Plain,
        Html
    }
}