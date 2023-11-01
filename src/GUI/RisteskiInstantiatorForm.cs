namespace ReactionStoichiometry.GUI;

using System.Numerics;

internal sealed partial class RisteskiInstantiatorForm : Form
{
    private IBalancerInstantiatable? _balancer;
    internal RisteskiInstantiatorForm() => InitializeComponent();

    internal void InitRisteskiTable(IBalancerInstantiatable b)
    {
        _balancer = b;

        theGrid.Rows.Clear();
        theGrid.RowCount = _balancer.EntitiesCount;
        for (var i = 0; i < _balancer.EntitiesCount; i++)
        {
            theGrid.Rows[i].HeaderCell.Value = _balancer.LabelFor(i);
            theGrid.Rows[i].Cells["Entity"].Value = _balancer.Entity(i);

            if (_balancer.GetCoefficientExpression(i) == String.Empty)
            {
                theGrid.Rows[i].Cells["Value"].ReadOnly = false;
                theGrid.Rows[i].Cells["Value"].Value = 1;
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = true;
            }
            else
            {
                theGrid.Rows[i].Cells["Value"].ReadOnly = true;
                theGrid.Rows[i].Cells["Value"].Value = _balancer.GetCoefficientExpression(i);
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = false;
            }
        }

        theGrid.Refresh();

        UpdateTable();
    }

    private void UpdateTable()
    {
        if (_balancer == null) return;

        try
        {
            var parameters = new List<BigInteger>();
            for (var i = 0; i < theGrid.Rows.Count; i++)
            {
                if (!Boolean.Parse(theGrid.Rows[i].Cells["IsFreeVariable"].Value.ToString()!)) continue;

                var cell = theGrid.Rows[i].Cells["Value"].Value ?? throw new FormatException();
                parameters.Add(BigInteger.Parse(cell.ToString()!));
            }

            txtInstance.Text = _balancer.Instantiate(parameters.ToArray());
        } catch (InvalidOperationException)
        {
            txtInstance.Text = "Could not get integer coefficients";
        } catch (FormatException)
        {
            txtInstance.Text = "Parsing error occurred";
        }
    }

    private void OnCellEndEdit(Object sender, DataGridViewCellEventArgs e) => UpdateTable();

    private void On_txtInstance_TextChanged(Object sender, EventArgs e)
    {
        var size = TextRenderer.MeasureText(txtInstance.Text, txtInstance.Font);
        Width = size.Width + 25;
        Height = txtInstance.Height * (1 + _balancer!.EntitiesCount + 1) + 120;
    }
}