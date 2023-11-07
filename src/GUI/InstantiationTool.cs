namespace ReactionStoichiometry;

using System.Numerics;

internal sealed partial class InstantiationTool : Form
{
    private IBalancerInstantiatable _balancer;

    internal InstantiationTool() => InitializeComponent();

    internal void Init(IBalancerInstantiatable balancer)
    {
        _balancer = balancer;

        theGrid.Rows.Clear();
        theGrid.RowCount = _balancer.SubstancesCount;
        for (var i = 0; i < _balancer.SubstancesCount; i++)
        {
            theGrid.Rows[i].HeaderCell.Value = _balancer.LabelFor(i);
            theGrid.Rows[i].Cells["Substance"].Value = _balancer.GetSubstance(i);

            var expr = _balancer.GetCoefficientExpressionString(i);

            if (String.IsNullOrEmpty(expr))
            {
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = true;
                theGrid.Rows[i].Cells["Expression"].Value = "free variable";
                theGrid.Rows[i].Cells["Value"].Value = 0;
            }
            else
            {
                theGrid.Rows[i].Cells["IsFreeVariable"].Value = false;
                theGrid.Rows[i].Cells["Expression"].Value = expr;
            }
        }

        Instantiate();
        ApplyVisualStyle();
        theGrid.Refresh();
    }

    private void ApplyVisualStyle()
    {
        for (var i = 0; i < theGrid.Rows.Count; i++)
        {
            var cv = theGrid.Rows[i].Cells["IsFreeVariable"].Value ?? throw new InvalidOperationException();
            var isFreeVarRow = Boolean.Parse(cv.ToString()!);
            theGrid.Rows[i].Cells["Value"].Style.BackColor = isFreeVarRow ? Color.Bisque : Color.White;
            theGrid.Rows[i].Cells["Value"].ReadOnly = !isFreeVarRow;
        }
    }

    private void Instantiate()
    {
        BigInteger[]? coefficients;
        try
        {
            List<BigInteger> parameters = new();
            for (var i = 0; i < theGrid.Rows.Count; i++)
            {
                if (!Boolean.Parse(theGrid.Rows[i].Cells["IsFreeVariable"].Value.ToString()!)) continue;

                var cv = theGrid.Rows[i].Cells["Value"].Value ?? throw new FormatException();
                parameters.Add(BigInteger.Parse(cv.ToString()!));
            }

            (coefficients, txtInstance.Text) = _balancer.Instantiate(parameters.ToArray());
        }
        catch (FormatException)
        {
            txtInstance.Text = "Parsing error occurred";
            coefficients = null;
        }
        catch (BalancerException)
        {
            txtInstance.Text = "Could not get integer coefficients";
            coefficients = null;
        }

        for (var i = 0; i < theGrid.Rows.Count; i++)
        {
            var cv = theGrid.Rows[i].Cells["IsFreeVariable"].Value ?? throw new InvalidOperationException();
            var isFreeVarRow = Boolean.Parse(cv.ToString()!);
            if (!isFreeVarRow) theGrid.Rows[i].Cells["Value"].Value = coefficients != null ? coefficients[i] : String.Empty;
        }

        AdaptFormSize();
    }

    private void OnCellEndEdit(Object sender, DataGridViewCellEventArgs e) => Instantiate();

    private void AdaptFormSize()
    {
        Width = Owner.Width;
        Height = txtInstance.Height * (1 + theGrid.RowCount + 1) + 120;
    }
}
