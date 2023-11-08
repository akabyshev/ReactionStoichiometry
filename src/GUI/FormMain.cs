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
        textBoxInput.Text = textBoxInput.Text.Replace(" ", String.Empty);
        var s = textBoxInput.Text;

        {
            var balancerThorne = new BalancerThorne(s);
            balancerThorne.Balance();
            resultMT.Text = balancerThorne.ToString(OutputFormat.DetailedPlain);
        }
        {
            var balancerRisteski = new BalancerRisteski(s);
            balancerRisteski.Balance();
            resultMR.Text = balancerRisteski.ToString(OutputFormat.DetailedPlain);

            if (resultMR.Text.Contains("FAIL")) return; // todo: Boolean StatusSuccess
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

        buttonBalance.Enabled = StringOperations.SeemsFine(textBoxInput.Text.Replace(" ", String.Empty));
        _instantiationTool.Visible = false;
        _permutationTool.Visible = false;
    }
}
