namespace ReactionStoichiometry;

internal sealed partial class PermutationTool : Form
{
    public PermutationTool() => InitializeComponent();

    public void Init(String s)
    {
        listLHS.Items.Clear();
        listRHS.Items.Clear();

        ChemicalReactionEquation e = new(s);
        for (var i = 0; i < e.EntitiesCount; i++)
        {
            (i < e.ReactantsCount ? listLHS : listRHS).Items.Add(e.GetEntity(i));
        }
    }

    private void OnListMouseDoubleClick(Object sender, MouseEventArgs e)
    {
        if (((ListBox)sender).SelectedItems.Count != 1) return;
        var item = ((ListBox)sender).SelectedItems[0] ?? throw new InvalidOperationException();
        var indexNew = ((ListBox)sender).Items.Count - 1;

        ((ListBox)sender).Items.Remove(item);
        ((ListBox)sender).Items.Insert(indexNew, item);

        ((ListBox)sender).SelectedItems.Clear();
        PassTheStringBack();
    }

    private void PassTheStringBack()
    {
        var s = String.Join("+", listLHS.Items.OfType<String>()) + "=" + String.Join("+", listRHS.Items.OfType<String>());
        (Owner as MainForm)!.textBoxInput.Text = s;
        (Owner as MainForm)!.Balance();
    }
}