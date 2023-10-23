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
        resultMT.Text = Helpers.SimpleStackedOutput(new BalancerThorne(textBoxInput.Text));
        resultMR.Text = Helpers.SimpleStackedOutput(new BalancerRisteskiDouble(textBoxInput.Text));
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

#pragma warning disable CA1822 // Mark members as static
    private void UpdateTable()
#pragma warning restore CA1822 // Mark members as static
    {
        //const string EXCEL_LIKE_UNDEF = "#VALUE!";
    }

    private void SyncControls()
    {
        resultMT.Text = string.Empty;
        resultMR.Text = string.Empty;
        buttonBalance.Enabled = Regex.IsMatch(textBoxInput.Text, RegexPatterns.MinimalSkeletalStructure);
    }
}