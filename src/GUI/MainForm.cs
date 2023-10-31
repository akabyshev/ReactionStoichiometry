namespace ReactionStoichiometry.GUI;

using System.Text.RegularExpressions;

internal sealed partial class MainForm : Form
{
    private readonly RisteskiInstantiatorForm _risteskiInstantiatorForm = new();

    internal MainForm()
    {
        InitializeComponent();
        textBoxInput.Text = "H2+O2=H2O+O3+H5O3";
        SyncControls();
    }

    private void On_buttonBalance_Click(Object sender, EventArgs e)
    {
        resultMT.Text = new BalancerThorne(textBoxInput.Text).ToString(ISpecialToStringProvider.OutputFormat.Plain);

        var balancer = new BalancerRisteskiRational(textBoxInput.Text);
        resultMR.Text = balancer.ToString(ISpecialToStringProvider.OutputFormat.Plain);

        _risteskiInstantiatorForm.Balancer = balancer;
        _risteskiInstantiatorForm.InitRisteskiTable();
        _risteskiInstantiatorForm.Show();
    }

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => SyncControls();

    private void SyncControls()
    {
        resultMT.Text = String.Empty;
        resultMR.Text = String.Empty;
        _risteskiInstantiatorForm.Visible = false;
        buttonBalance.Enabled = Regex.IsMatch(textBoxInput.Text, Parsing.MINIMAL_SKELETAL_STRUCTURE);
    }
}