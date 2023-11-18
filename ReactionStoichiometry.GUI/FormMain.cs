using System.Numerics;

namespace ReactionStoichiometry.GUI
{
    internal sealed partial class FormMain : Form
    {
        private ChemicalReactionEquation _equation = null!;

        internal FormMain()
        {
            InitializeComponent();
            InitializeWebView();
            ResetControls();
        }

        #region Event Handlers
        private void On_buttonBalance_Click(Object sender, EventArgs e)
        {
            Balance();
        }

        private void On_textBoxInput_TextChanged(Object sender, EventArgs e)
        {
            ResetControls();
        }

        private void OnCellEndEdit(Object sender, DataGridViewCellEventArgs e)
        {
            Instantiate();
        }

        private void OnListMouseDoubleClick(Object sender, MouseEventArgs e)
        {
            if (listPermutator.SelectedItems.Count != 1)
            {
                return;
            }
            var item = listPermutator.SelectedItems[index: 0] ?? throw new InvalidOperationException();
            var indexNew = listPermutator.Items.Count - 1;

            listPermutator.Items.Remove(item);
            listPermutator.Items.Insert(indexNew, item);

            listPermutator.SelectedItems.Clear();

            var s = String.Join(separator: " + ", listPermutator.Items.OfType<String>()) + " = 0";
            textBoxInput.Text = s;
            Balance();
        }
        #endregion

        private async void InitializeWebView()
        {
            await webviewResult.EnsureCoreWebView2Async(environment: null);
            webviewResult.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        }

        private async void ResetControls()
        {
            await webviewResult.EnsureCoreWebView2Async(environment: null);
            webviewResult.NavigateToString(String.Empty);

            txtGeneralForm.Text = String.Empty;
            txtInstance.Text = String.Empty;
            listPermutator.Items.Clear();
            gridCoefficients.Rows.Clear();

            buttonBalance.Enabled = ChemicalReactionEquation.IsValidString(textBoxInput.Text);
        }

        private void Balance()
        {
            var s = textBoxInput.Text;
            _equation = new ChemicalReactionEquation(s
                                                   , ChemicalReactionEquation.SolutionTypes.Generalized | ChemicalReactionEquation.SolutionTypes.InverseBased);
            txtGeneralForm.Text = _equation.GeneralizedEquation;

            var generalizedSolution = _equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized) as GeneralizedSolution;

            if (generalizedSolution!.Success)
            {
                #region Init instantiation tool
                gridCoefficients.RowCount = _equation.Substances.Count;
                for (var i = 0; i < _equation.Substances.Count; i++)
                {
                    gridCoefficients.Rows[i].Cells[columnName: "Substance"].Value = _equation.Substances[i];

                    var expr = generalizedSolution.AlgebraicExpressions[i];
                    gridCoefficients.Rows[i].Cells[columnName: "Coefficient"].Value = expr;

                    if (expr.Contains(value: " = "))
                    {
                        gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value = false;
                        gridCoefficients.Rows[i].Cells[columnName: "Value"].ReadOnly = true;
                    }
                    else
                    {
                        gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value = true;
                        gridCoefficients.Rows[i].Cells[columnName: "Value"].ReadOnly = false;
                        gridCoefficients.Rows[i].Cells[columnName: "Value"].Value = generalizedSolution.GuessedSimplestSolution ?? 0;
                        gridCoefficients.Rows[i].Cells[columnName: "Value"].Style.Font = new Font(gridCoefficients.Font, FontStyle.Bold | FontStyle.Underline);
                    }
                }
                Instantiate();
                # endregion

                InitPermutation();
                webviewResult.NavigateToString(GetHtmlContentFromJson(_equation.ToJson()));
                theTabControl.Enabled = true;
            }
            else
            {
                ResetControls();
                MessageBox.Show(text: "Balancing failed. Check your syntax and try again", caption: "Failed", MessageBoxButtons.OK);
            }
        }

        private static String GetHtmlContentFromJson(String jsonContent)
        {
            var htmlContent = ResourcesWebview.htmlContent.Replace(oldValue: "%jsContent%", ResourcesWebview.jsContent)
                                              .Replace(oldValue: "%cssContent%", ResourcesWebview.cssContent)
                                              .Replace(oldValue: "%jsonContent%", jsonContent);
            return htmlContent;
        }

        private void InitPermutation()
        {
            listPermutator.Items.Clear();
            foreach (var s in _equation.Substances)
            {
                listPermutator.Items.Add(s);
            }
        }

        private void ApplyGridVisuals()
        {
            for (var i = 0; i < gridCoefficients.Rows.Count; i++)
            {
                gridCoefficients.Rows[i].DefaultCellStyle.BackColor = Color.White;

                var cv = gridCoefficients.Rows[i].Cells[columnName: "Value"].Value;
                if (cv == null || !BigInteger.TryParse(cv.ToString()!, out var value))
                {
                    continue;
                }

                if (value > 0)
                {
                    gridCoefficients.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                }
                else if (value < 0)
                {
                    gridCoefficients.Rows[i].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                }
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
                    {
                        continue;
                    }

                    var cv = gridCoefficients.Rows[i].Cells[columnName: "Value"].Value ?? throw new FormatException();
                    parameters.Add(BigInteger.Parse(cv.ToString()!));
                }

                coefficients = _equation.Instantiate(parameters.ToArray());
                txtInstance.Text = _equation.EquationWithIntegerCoefficients(coefficients);
            }
            catch (FormatException)
            {
                txtInstance.Text = "Parsing error occurred";
                coefficients = null;
            }
            catch (AppSpecificException)
            {
                txtInstance.Text = "Failed to get valid coefficients";
                coefficients = null;
            }

            for (var i = 0; i < gridCoefficients.Rows.Count; i++)
            {
                var cv = gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value ?? throw new InvalidOperationException();
                var isFreeVarRow = Boolean.Parse(cv.ToString()!);
                if (!isFreeVarRow)
                {
                    gridCoefficients.Rows[i].Cells[columnName: "Value"].Value = coefficients != null ? coefficients[i] : "#VALUE!";
                }
            }

            ApplyGridVisuals();
        }
    }
}
