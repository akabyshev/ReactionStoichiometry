namespace ReactionStoichiometry.GUI;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        SyncControls();
    }

#pragma warning disable IDE1006 // Naming Styles
    private void buttonBalance_Click(Object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        resultMT.Text = new BalancerThorne(textBoxInput.Text).ToString(ISpecialToString.OutputFormat.Html);
        resultMR.Text = new BalancerRisteskiDouble(textBoxInput.Text).ToString();
        UpdateTable();
    }

#pragma warning disable IDE1006 // Naming Styles
    private void textBoxInput_TextChanged(Object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        SyncControls();
    }

#pragma warning disable IDE1006 // Naming Styles
    private void dataGridView1_CellEndEdit(Object sender, DataGridViewCellEventArgs e)
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
        resultMT.Text = String.Empty;
        resultMR.Text = String.Empty;
        buttonBalance.Enabled = System.Text.RegularExpressions.Regex.IsMatch(textBoxInput.Text, Parsing.MINIMAL_SKELETAL_STRUCTURE);
    }
}