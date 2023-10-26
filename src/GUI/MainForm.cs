using System.Text.RegularExpressions;

namespace ReactionStoichiometry.GUI;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        SyncControls();
    }

#pragma warning disable IDE1006 // Naming Styles
    private void buttonBalance_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        resultMT.Text = new BalancerThorne(textBoxInput.Text).SimpleStackedOutput();
        resultMR.Text = new BalancerRisteskiDouble(textBoxInput.Text).SimpleStackedOutput();
        UpdateTable();
    }

#pragma warning disable IDE1006 // Naming Styles
    private void textBoxInput_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        SyncControls();
    }

#pragma warning disable IDE1006 // Naming Styles
    private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        UpdateTable();
    }

    private static void UpdateTable()
    {
        //const string EXCEL_LIKE_UNDEF = "#VALUE!";
    }

    private void SyncControls()
    {
        resultMT.Text = string.Empty;
        resultMR.Text = string.Empty;
        buttonBalance.Enabled = Regex.IsMatch(textBoxInput.Text, RegexPatterns.MINIMAL_SKELETAL_STRUCTURE);
    }
}