namespace ReactionStoichiometry.GUI;

internal sealed partial class MainForm : Form
{
    private RisteskiInstantiatorForm? _risteskiInstantiatorForm;

    internal MainForm()
    {
        InitializeComponent();
        SyncControls();
    }

    private void On_buttonBalance_Click(Object sender, EventArgs e)
    {
        resultMT.Text = new BalancerThorne(textBoxInput.Text).ToString(ISpecialToStringProvider.OutputFormat.Plain);

        var balancer = new BalancerRisteskiRational(textBoxInput.Text);
        resultMR.Text = balancer.ToString(ISpecialToStringProvider.OutputFormat.Plain);

        // ReSharper disable once ArrangeThisQualifier
        _risteskiInstantiatorForm = new RisteskiInstantiatorForm(balancer) { Width = this.Width};
        _risteskiInstantiatorForm.Show();
    }

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => SyncControls();

    private void SyncControls()
    {
        resultMT.Text = String.Empty;
        resultMR.Text = String.Empty;
        if (_risteskiInstantiatorForm is { IsDisposed: false })
        {
            _risteskiInstantiatorForm.Close();
            _risteskiInstantiatorForm.Dispose();
        }

        buttonBalance.Enabled = ChemicalReactionEquation.SeemsFine(textBoxInput.Text.Replace(" ", String.Empty));
    }
}