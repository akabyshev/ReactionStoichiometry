namespace ReactionStoichiometry;

internal interface IChemicalEntityCollection
{
    Int32 EntitiesCount { get; }
    String Entity(Int32 i);
}