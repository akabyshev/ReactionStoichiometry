namespace ReactionStoichiometry;

internal sealed partial class MainForm : Form
{
    private InstantiationTool? _instantiatorForm;
    private readonly PermutationTool _permutationTool = new() {Visible = false};

    internal MainForm()
    {
        InitializeComponent();
        SyncControls();
    }

    private void On_buttonBalance_Click(Object sender, EventArgs e)
    {
        var s = textBoxInput.Text.Replace(" ", String.Empty);
        resultMT.Text = new BalancerThorne(s).ToString(ISpecialToStringProvider.OutputFormat.Plain);

        var balancer = new BalancerRisteskiRational(s);
        resultMR.Text = balancer.ToString(ISpecialToStringProvider.OutputFormat.Plain);
        _permutationTool.In(s);
        _permutationTool.Visible = true;

        // ReSharper disable once ArrangeThisQualifier
        _instantiatorForm = new InstantiationTool(balancer) { Width = this.Width};
        _instantiatorForm.Show();
    }

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => SyncControls();

    private void SyncControls()
    {
        resultMT.Text = String.Empty;
        resultMR.Text = String.Empty;
        if (_instantiatorForm is { IsDisposed: false })
        {
            _instantiatorForm.Close();
            _instantiatorForm.Dispose();
        }

        buttonBalance.Enabled = ChemicalReactionEquation.SeemsFine(textBoxInput.Text.Replace(" ", String.Empty));
    }
}