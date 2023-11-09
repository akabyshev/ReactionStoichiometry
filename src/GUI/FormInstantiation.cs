namespace ReactionStoichiometry;

using System.Numerics;

internal sealed partial class FormInstantiation : Form
{
    private BalancerRisteski? _balancer;

    internal FormInstantiation() => InitializeComponent();

    internal void Init(BalancerRisteski balancer)
    {
        _balancer = balancer;
        txtGeneralForm.Text = _balancer.EquationWithPlaceholders();
        theGrid.Rows.Clear();
        theGrid.RowCount = _balancer.Equation.Substances.Count;
        for (var i = 0; i < _balancer.Equation.Substances.Count; i++)
        {
            theGrid.Rows[i].HeaderCell.Value = _balancer.LabelFor(i);
            theGrid.Rows[i].Cells[columnName: "Substance"].Value = _balancer.Equation.Substances[i];

            var expr = _balancer.AlgebraicExpressionForCoefficient(i);

            if (String.IsNullOrEmpty(expr))
            {
                theGrid.Rows[i].Cells[columnName: "IsFreeVariable"].Value = true;
                theGrid.Rows[i].Cells[columnName: "Expression"].Value = "\u27a2";
                theGrid.Rows[i].Cells[columnName: "Value"].Value = 0;
                theGrid.Rows[i].Cells[columnName: "Value"].ReadOnly = false;
            }
            else
            {
                theGrid.Rows[i].Cells[columnName: "IsFreeVariable"].Value = false;
                theGrid.Rows[i].Cells[columnName: "Expression"].Value = expr;
                theGrid.Rows[i].Cells[columnName: "Value"].ReadOnly = true;
            }
        }

        AdaptFormSize();
        Instantiate();
    }

    private void ApplyVisualStyle()
    {
        for (var i = 0; i < theGrid.Rows.Count; i++)
        {
            theGrid.Rows[i].DefaultCellStyle.BackColor = Color.White;

            var cv = theGrid.Rows[i].Cells[columnName: "Value"].Value;
            if (cv == null || !BigInteger.TryParse(cv.ToString()!, out var value))
                continue;

            if (value > 0)
                theGrid.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            else if (value < 0)
                theGrid.Rows[i].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
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
                if (!Boolean.Parse(theGrid.Rows[i].Cells[columnName: "IsFreeVariable"].Value.ToString()!))
                    continue;

                var cv = theGrid.Rows[i].Cells[columnName: "Value"].Value ?? throw new FormatException();
                parameters.Add(BigInteger.Parse(cv.ToString()!));
            }

            coefficients = _balancer!.Instantiate(parameters.ToArray());
            txtInstance.Text = _balancer!.EquationWithIntegerCoefficients(coefficients);
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
            var cv = theGrid.Rows[i].Cells[columnName: "IsFreeVariable"].Value ?? throw new InvalidOperationException();
            var isFreeVarRow = Boolean.Parse(cv.ToString()!);
            if (!isFreeVarRow)
                theGrid.Rows[i].Cells[columnName: "Value"].Value = coefficients != null ? coefficients[i] : "#VALUE!";
        }

        ApplyVisualStyle();
    }

    private void OnCellEndEdit(Object sender, DataGridViewCellEventArgs e) => Instantiate();

    private void AdaptFormSize()
    {
        Width = Owner!.Width / 2;
        Height = 144 + txtInstance.Height + 50 * theGrid.RowCount + txtGeneralForm.Height;
    }
}
