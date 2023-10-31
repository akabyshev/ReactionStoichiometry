namespace ReactionStoichiometry;

internal interface IChemicalEntitiesList
{
    Int32 EntitiesCount { get; }
    String Entity(Int32 i);
}