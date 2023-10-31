namespace ReactionStoichiometry.GUI;

using System.Text.RegularExpressions;

public partial class MainForm : Form
{
    private readonly RisteskiInstantiatorForm _risteskiInstantiatorForm = new();

    public MainForm()
    {
        InitializeComponent();
        textBoxInput.Text = "Fe2(SO4)3 + PrTlTe3 + H3PO4 = Fe0.996(H2PO4)2H2O + Tl1.987(SO3)3 + Pr1.998(SO4)3 + Te2O3 + P2O5 + H2S";
        SyncControls();
    }

    private void On_buttonBalance_Click(Object sender, EventArgs e)
    {
        resultMT.Text = new BalancerThorne(textBoxInput.Text).ToString(IImplementsSpecialToString.OutputFormat.Plain);

        var balancer = new BalancerRisteskiRational(textBoxInput.Text);
        resultMR.Text = balancer.ToString(IImplementsSpecialToString.OutputFormat.Plain);

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