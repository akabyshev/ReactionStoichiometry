namespace ReactionStoichiometry;

internal interface IFragmentStore
{
    Int32 FragmentsCount { get; }
    String Fragment(Int32 i);
}