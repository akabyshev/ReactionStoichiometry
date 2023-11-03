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
            theGrid.Rows[i].Cells["Entity"].Value = _balancer.GetEntity(i);

            if (String.IsNullOrEmpty(_balancer.GetCoefficientExpressionString(i)))
            {
                theGrid.Rows[i].Cells["Value"].ReadOnly = false;
                theGrid.Rows[i].Cells["Value"].Value = 1;
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = true;
            }
            else
            {
                theGrid.Rows[i].Cells["Value"].ReadOnly = true;
                theGrid.Rows[i].Cells["Value"].Value = _balancer.GetCoefficientExpressionString(i);
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = false;
            }
        }

        txtInstance.Text = Instantiate();
        theGrid.Refresh();
        ApplyVisualStyle();
    }

    private void ApplyVisualStyle()
    {
        for (var i = 0; i < theGrid.Rows.Count; i++)
        {
            var cv = theGrid.Rows[i].Cells["IsFreeVariable"].Value ?? throw new InvalidOperationException();
            var isFreeVarRow = Boolean.Parse(cv.ToString()!);
            theGrid.Rows[i].Cells["Value"].Style.BackColor = isFreeVarRow ? Color.Bisque : Color.White;
        }
    }

    private String Instantiate()
    {
        try
        {
            List<BigInteger> parameters = new();
            for (var i = 0; i < theGrid.Rows.Count; i++)
            {
                if (!Boolean.Parse(theGrid.Rows[i].Cells["IsFreeVariable"].Value.ToString()!)) continue;

                var cv = theGrid.Rows[i].Cells["Value"].Value ?? throw new FormatException();
                parameters.Add(BigInteger.Parse(cv.ToString()!));
            }

            return _balancer!.Instantiate(parameters.ToArray());
        }
        catch (FormatException)
        {
            return "Parsing error occurred";
        }
        catch (BalancerException)
        {
            return "Could not get integer coefficients";
        }
    }

    private void OnCellEndEdit(Object sender, DataGridViewCellEventArgs e) => txtInstance.Text = Instantiate();

    private void OnTextChanged(Object sender, EventArgs e) => ResizeForm();

    private void ResizeForm()
    {
        var size = TextRenderer.MeasureText(txtInstance.Text, txtInstance.Font);
        Width = size.Width + 25;
        Height = txtInstance.Height * (1 + theGrid.RowCount + 1) + 120;
    }
}