namespace ReactionStoichiometry;

internal sealed partial class FormPermutation : Form
{
    internal FormPermutation() => InitializeComponent();

    internal void Init(String s)
    {
        listLHS.Items.Clear();
        listRHS.Items.Clear();

        ChemicalReactionEquation e = new(s);
        for (var i = 0; i < e.Substances.Count; i++)
            (i < e.OriginalReactantsCount ? listLHS : listRHS).Items.Add(e.Substances[i]);
    }

    private void OnListMouseDoubleClick(Object sender, MouseEventArgs e)
    {
        var list = (ListBox)sender;
        if (list.SelectedItems.Count != 1)
            return;
        var item = list.SelectedItems[index: 0] ?? throw new InvalidOperationException();
        var indexNew = list.Items.Count - 1;

        list.Items.Remove(item);
        list.Items.Insert(indexNew, item);

        list.SelectedItems.Clear();
        PassTheStringBack();
    }

    private void PassTheStringBack()
    {
        var s = String.Join(separator: "+", listLHS.Items.OfType<String>()) + "=" + String.Join(separator: "+", listRHS.Items.OfType<String>());
        (Owner as FormMain)!.textBoxInput.Text = s;
        (Owner as FormMain)!.Balance();
    }
}
