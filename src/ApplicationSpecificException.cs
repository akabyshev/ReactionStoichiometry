namespace ReactionStoichiometry;

internal class ApplicationSpecificException : InvalidOperationException
{
    public ApplicationSpecificException(string message) : base(message) { }
}