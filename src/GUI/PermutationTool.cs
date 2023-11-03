namespace ReactionStoichiometry;

internal sealed partial class PermutationTool : Form
{
    public PermutationTool()
    {
        InitializeComponent();
    }

    public void In(String s)
    {
        ChemicalReactionEquation e = new(s);

        listEntities.Groups.Add(new ListViewGroup("LHS", "Reactants"));
        listEntities.Groups.Add(new ListViewGroup("RHS", "Products"));
        for (var i = 0; i < e.EntitiesCount; i++)
        {
            var item = new ListViewItem(e.GetEntity(i)) { Group = (i < e.ReactantsCount ? listEntities.Groups["LHS"] : listEntities.Groups["RHS"]) };
            listEntities.Items.Add(item);
        }
        listEntities.Refresh();
    }

    public void Out()
    {

    }
}