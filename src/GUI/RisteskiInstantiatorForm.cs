namespace ReactionStoichiometry.GUI;

using System.Numerics;


internal sealed partial class RisteskiInstantiatorForm : Form
{
    internal IBalancerInstantiatable? Balancer;
    internal RisteskiInstantiatorForm() => InitializeComponent();

    internal void InitRisteskiTable()
    {
        if (Balancer == null) return;
        theGrid.Rows.Clear();
        theGrid.RowCount = Balancer.EntitiesCount;
        for (var i = 0; i < Balancer.EntitiesCount; i++)
        {
            theGrid.Rows[i].HeaderCell.Value = Balancer.LabelFor(i);
            theGrid.Rows[i].Cells["Entity"].Value = Balancer.Entity(i);

            if (Balancer.GetCoefficientExpression(i) == String.Empty)
            {
                theGrid.Rows[i].Cells["Value"].ReadOnly = false;
                theGrid.Rows[i].Cells["Value"].Style.BackColor = Color.Ivory;
                theGrid.Rows[i].Cells["Value"].Value = 1;
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = true;
            }
            else
            {
                theGrid.Rows[i].Cells["Value"].Value = Balancer.GetCoefficientExpression(i);
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = false;
            }
        }

        theGrid.Refresh();

        UpdateTable();
    }

    private void UpdateTable()
    {
        if (Balancer == null) return;

        try
        {
            var parameters = new List<BigInteger>();
            for (var i = 0; i < theGrid.Rows.Count; i++)
            {
                if (!Boolean.Parse(theGrid.Rows[i].Cells["IsFreeVariable"].Value.ToString()!)) continue;

                var cell = theGrid.Rows[i].Cells["Value"].Value ?? throw new FormatException();
                parameters.Add(BigInteger.Parse(cell.ToString()!));
            }

            txtInstance.Text = Balancer.Instantiate(parameters.ToArray());
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
        Height = txtInstance.Height * (1 + Balancer!.EntitiesCount + 1) + 120;
    }
}