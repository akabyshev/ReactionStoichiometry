namespace ReactionStoichiometry;

using System.Numerics;

internal sealed partial class FormMain : Form
{
    private BalancerRisteski _balancer;

    internal FormMain()
    {
        InitializeComponent();
        ResetControls();
    }

    private void Balance()
    {
        var s = textBoxInput.Text.Replace(oldValue: " ", String.Empty);

        _balancer = new BalancerRisteski(s);
        if (_balancer.Run())
        {
            txtGeneralForm.Text = _balancer.EquationWithPlaceholders();
            InitInstantiation();
            InitPermutation();
            ctrlDetailedPlain.Text = _balancer.ToString(Balancer.OutputFormat.DetailedPlain);
            ctrlDetailedHtml.Text = _balancer.ToString(Balancer.OutputFormat.DetailedHtml);
        }
        else
        {
            ResetControls();
            MessageBox.Show(text: "Balancing failed. Check your syntax and try again", caption: "Failed", MessageBoxButtons.OK);
        }
    }

    private void InitInstantiation()
    {
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

        Instantiate();
    }

    private void InitPermutation()
    {
        listLHS.Items.Clear();
        listRHS.Items.Clear();

        for (var i = 0; i < _balancer.Equation.Substances.Count; i++)
            (i < _balancer.Equation.OriginalReactantsCount ? listLHS : listRHS).Items.Add(_balancer.Equation.Substances[i]);
    }

    private void On_buttonBalance_Click(Object sender, EventArgs e) => Balance();

    private void On_textBoxInput_TextChanged(Object sender, EventArgs e) => ResetControls();

    private void ResetControls()
    {
        ctrlDetailedPlain.Text = String.Empty;
        ctrlDetailedHtml.Text = String.Empty;

        buttonBalance.Enabled = StringOperations.SeemsFine(textBoxInput.Text.Replace(oldValue: " ", String.Empty));
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

            coefficients = _balancer.Instantiate(parameters.ToArray());
            txtInstance.Text = _balancer.EquationWithIntegerCoefficients(coefficients);
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

    private void OnListMouseDoubleClick(Object sender, MouseEventArgs e)
    {
        var list = (ListBox)sender;
        if (list.SelectedItems.Count != 1)
            return;
        var item = list.SelectedItems[index: 0] ?? throw new InvalidOperationException();
        var indexNew = list.Items.Count - 1;

        list.Items.Remove(item);
        list.Items.Insert(indexNew, item);

        list.SelectedItems.Clear();
        PassTheStringBack();
    }

    private void PassTheStringBack()
    {
        var s = String.Join(separator: "+", listLHS.Items.OfType<String>()) + "=" + String.Join(separator: "+", listRHS.Items.OfType<String>());
        textBoxInput.Text = s;
        Balance();
    }
}
