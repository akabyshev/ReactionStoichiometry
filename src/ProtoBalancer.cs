namespace ReactionStoichiometry;

internal abstract class ProtoBalancer
{
    protected readonly String OriginalSkeletal;
    private protected virtual String Outcome => String.Empty;

    protected ProtoBalancer(String s) => OriginalSkeletal = s;

    internal abstract String ToString(OutputFormat format);

    internal enum OutputFormat
    {
        Plain,
        Html
    }
}