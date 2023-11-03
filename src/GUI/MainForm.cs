namespace ReactionStoichiometry;

internal sealed partial class MainForm : Form
{
    private readonly InstantiationTool _instantiationTool = new();
    private readonly PermutationTool _permutationTool = new();

    internal MainForm()
    {
        InitializeComponent();
        SyncControls();
        _permutationTool.Owner = this;
        _instantiationTool.Owner = this;

    }

    private void On_buttonBalance_Click(Object sender, EventArgs e) => Balance();

    internal void Balance()
    {
        textBoxInput.Text = textBoxInput.Text.Replace(" ", String.Empty);
        var s = textBoxInput.Text;

        resultMT.Text = new BalancerThorne(s).ToString(ISpecialToStringProvider.OutputFormat.Plain);

        var balancer = new BalancerRisteskiRational(s);
        resultMR.Text = balancer.ToString(ISpecialToStringProvider.OutputFormat.Plain);

        _permutationTool.Init(s);
        _instantiationTool.Init(balancer);

        _permutationTool.Visible = true;
        _instantiationTool.Visible = true;
    }

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => SyncControls();

    private void SyncControls()
    {
        resultMT.Text = String.Empty;
        resultMR.Text = String.Empty;

        buttonBalance.Enabled = ChemicalReactionEquation.SeemsFine(textBoxInput.Text.Replace(" ", String.Empty));
        _instantiationTool.Visible = false;
        _permutationTool.Visible = false;
    }
}