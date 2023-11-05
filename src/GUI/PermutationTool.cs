﻿namespace ReactionStoichiometry;

internal sealed partial class PermutationTool : Form
{
    internal PermutationTool() => InitializeComponent();

    internal void Init(String s)
    {
        listLHS.Items.Clear();
        listRHS.Items.Clear();

        ChemicalReactionEquation e = new(s);
        for (var i = 0; i < e.EntitiesCount; i++)
        {
            (i < e.OriginalReactantsCount ? listLHS : listRHS).Items.Add(e.GetEntity(i));
        }
    }

    private void OnListMouseDoubleClick(Object sender, MouseEventArgs e)
    {
        var list = (ListBox)sender;
        if (list.SelectedItems.Count != 1) return;
        var item = list.SelectedItems[0] ?? throw new InvalidOperationException();
        var indexNew = list.Items.Count - 1;

        list.Items.Remove(item);
        list.Items.Insert(indexNew, item);

        list.SelectedItems.Clear();
        PassTheStringBack();
    }

    private void PassTheStringBack()
    {
        var s = String.Join("+", listLHS.Items.OfType<String>()) + "=" + String.Join("+", listRHS.Items.OfType<String>());
        (Owner as MainForm)!.textBoxInput.Text = s;
        (Owner as MainForm)!.Balance();
    }
}