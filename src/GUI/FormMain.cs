namespace ReactionStoichiometry;

internal sealed partial class FormMain : Form
{
    private readonly FormInstantiation _instantiationTool = new();
    private readonly FormPermutation _permutationTool = new();

    internal FormMain()
    {
        InitializeComponent();
        SyncControls();
        _permutationTool.Owner = this;
        _instantiationTool.Owner = this;
    }

    internal void Balance()
    {
        textBoxInput.Text = textBoxInput.Text.Replace(oldValue: " ", String.Empty);
        var s = textBoxInput.Text;

        {
            var balancerThorne = new BalancerThorne(s);
            balancerThorne.Run();
            resultMT.Text = balancerThorne.ToString(Balancer.OutputFormat.DetailedPlain);
        }
        {
            var balancerRisteski = new BalancerRisteski(s);
            var ok = balancerRisteski.Run();
            resultMR.Text = balancerRisteski.ToString(Balancer.OutputFormat.DetailedPlain);

            if (!ok) return;
            _permutationTool.Init(s);
            _instantiationTool.Init(balancerRisteski);

            _permutationTool.Visible = true;
            _instantiationTool.Visible = true;
        }
    }

    private void On_buttonBalance_Click(Object sender, EventArgs e) => Balance();

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => SyncControls();

    private void SyncControls()
    {
        resultMT.Text = String.Empty;
        resultMR.Text = String.Empty;

        buttonBalance.Enabled = StringOperations.SeemsFine(textBoxInput.Text.Replace(oldValue: " ", String.Empty));
        _instantiationTool.Visible = false;
        _permutationTool.Visible = false;
    }
}
