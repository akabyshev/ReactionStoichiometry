namespace ReactionStoichiometryLegacyGUI;

using System.Numerics;
using ReactionStoichiometry;

internal sealed partial class FormMain : Form
{
    private BalancerGeneralized _balancer;

    internal FormMain()
    {
        InitializeComponent();
        ResetControls();
    }

    #region Event Handlers
    private void On_buttonBalance_Click(Object sender, EventArgs e) => Balance();

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => ResetControls();

    private void OnCellEndEdit(Object sender, DataGridViewCellEventArgs e) => Instantiate();

    private void OnListMouseDoubleClick(Object sender, MouseEventArgs e)
    {
        if (listPermutator.SelectedItems.Count != 1)
            return;
        var item = listPermutator.SelectedItems[index: 0] ?? throw new InvalidOperationException();
        var indexNew = listPermutator.Items.Count - 1;

        listPermutator.Items.Remove(item);
        listPermutator.Items.Insert(indexNew, item);

        listPermutator.SelectedItems.Clear();

        var s = String.Join(separator: "+", listPermutator.Items.OfType<String>()) + "=0";
        textBoxInput.Text = s;
        Balance();
    }
    #endregion

    private void ResetControls()
    {
        txtDetailedPlain.Text = String.Empty;
        txtDetailedHtml.Text = String.Empty;
        txtGeneralForm.Text = String.Empty;
        txtInstance.Text = String.Empty;
        listPermutator.Items.Clear();
        gridCoefficients.Rows.Clear();
        buttonBalance.Enabled = StringOperations.SeemsFine(textBoxInput.Text.Replace(oldValue: " ", String.Empty));
    }

    private void Balance()
    {
        var s = textBoxInput.Text.Replace(oldValue: " ", String.Empty);

        _balancer = new BalancerGeneralized(s);
        txtGeneralForm.Text = _balancer.EquationWithPlaceholders();

        if (_balancer.Run())
        {
            InitInstantiation();
            InitPermutation();
            txtDetailedPlain.Text = _balancer.ToString(Balancer.OutputFormat.DetailedPlain);
            txtDetailedHtml.Text = _balancer.ToString(Balancer.OutputFormat.DetailedHtml);
        }
        else
        {
            ResetControls();
            MessageBox.Show(text: "Balancing failed. Check your syntax and try again", caption: "Failed", MessageBoxButtons.OK);
        }
    }

    private void InitInstantiation()
    {
        gridCoefficients.RowCount = _balancer.Equation.Substances.Count;
        for (var i = 0; i < _balancer.Equation.Substances.Count; i++)
        {
            gridCoefficients.Rows[i].HeaderCell.Value = _balancer.LabelFor(i);
            gridCoefficients.Rows[i].Cells[columnName: "Substance"].Value = _balancer.Equation.Substances[i];

            var expr = _balancer.AlgebraicExpressionForCoefficient(i);

            if (String.IsNullOrEmpty(expr))
            {
                gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value = true;
                gridCoefficients.Rows[i].Cells[columnName: "Expression"].Value = "\u27a2";
                gridCoefficients.Rows[i].Cells[columnName: "Value"].Value = 0;
                gridCoefficients.Rows[i].Cells[columnName: "Value"].ReadOnly = false;
            }
            else
            {
                gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value = false;
                gridCoefficients.Rows[i].Cells[columnName: "Expression"].Value = expr;
                gridCoefficients.Rows[i].Cells[columnName: "Value"].ReadOnly = true;
            }
        }

        Instantiate();
    }

    private void InitPermutation()
    {
        foreach (var s in _balancer.Equation.Substances)
            listPermutator.Items.Add(s);
    }

    private void ApplyGridVisuals()
    {
        for (var i = 0; i < gridCoefficients.Rows.Count; i++)
        {
            gridCoefficients.Rows[i].DefaultCellStyle.BackColor = Color.White;

            var cv = gridCoefficients.Rows[i].Cells[columnName: "Value"].Value;
            if (cv == null || !BigInteger.TryParse(cv.ToString()!, out var value))
                continue;

            if (value > 0)
                gridCoefficients.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            else if (value < 0)
                gridCoefficients.Rows[i].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
        }
    }

    private void Instantiate()
    {
        BigInteger[]? coefficients;
        try
        {
            List<BigInteger> parameters = new();
            for (var i = 0; i < gridCoefficients.Rows.Count; i++)
            {
                if (!Boolean.Parse(gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value.ToString()!))
                    continue;

                var cv = gridCoefficients.Rows[i].Cells[columnName: "Value"].Value ?? throw new FormatException();
                parameters.Add(BigInteger.Parse(cv.ToString()!));
            }

            coefficients = _balancer.Instantiate(parameters.ToArray());
            txtInstance.Text = _balancer.EquationWithIntegerCoefficients(coefficients);
        }
        catch (FormatException)
        {
            txtInstance.Text = "Parsing error occurred";
            coefficients = null;
        }
        catch (AppSpecificException)
        {
            txtInstance.Text = "Could not get integer coefficients";
            coefficients = null;
        }

        for (var i = 0; i < gridCoefficients.Rows.Count; i++)
        {
            var cv = gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value ?? throw new InvalidOperationException();
            var isFreeVarRow = Boolean.Parse(cv.ToString()!);
            if (!isFreeVarRow)
                gridCoefficients.Rows[i].Cells[columnName: "Value"].Value = coefficients != null ? coefficients[i] : "#VALUE!";
        }

        ApplyGridVisuals();
    }
}
