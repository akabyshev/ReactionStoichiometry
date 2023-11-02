namespace ReactionStoichiometry;

internal interface IChemicalEntityList
{
    Int32 EntitiesCount { get; }
    String GetEntity(Int32 i);
}