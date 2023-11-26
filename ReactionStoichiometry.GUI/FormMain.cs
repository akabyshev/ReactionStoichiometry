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
            await webviewReport.EnsureCoreWebView2Async(environment: null);
            webviewReport.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        }

        private async void ResetControls()
        {
            await webviewReport.EnsureCoreWebView2Async(environment: null);
            webviewReport.NavigateToString(String.Empty);

            txtGeneralForm.Text = String.Empty;
            textboxInstantiate.Text = String.Empty;
            listPermutator.Items.Clear();
            gridInstantiate.Rows.Clear();

            buttonBalance.Enabled = ChemicalReactionEquation.IsValidString(textBoxInput.Text);
        }

        private void Balance()
        {
            var s = textBoxInput.Text;
            _equation = new ChemicalReactionEquation(s);
            txtGeneralForm.Text = _equation.InGeneralForm;

            if (_equation.RowsBasedSolution.Success)
            {
                #region 'Instantiation' tool
                gridInstantiate.RowCount = _equation.Substances.Count;
                for (var i = 0; i < _equation.Substances.Count; i++)
                {
                    gridInstantiate.Rows[i].Cells[gridInstantiateColumnSubstance.Name].Value = _equation.Substances[i];

                    var expr = _equation.RowsBasedSolution.AlgebraicExpressions![i];
                    gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnCoefficient.Name].Value = expr;

                    if (expr.Contains(value: " = "))
                    {
                        gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnIsFreeVariable.Name].Value = false;
                        gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].ReadOnly = true;
                    }
                    else
                    {
                        gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnIsFreeVariable.Name].Value = true;
                        gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].ReadOnly = false;
                        gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].Value =
                            _equation.RowsBasedSolution.SimplestSolution != null ? _equation.RowsBasedSolution.SimplestSolution[i] : 0;
                        gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].Style.Font =
                            new Font(gridInstantiate.Font, FontStyle.Bold | FontStyle.Underline);
                    }
                }
                Instantiate();
                # endregion

                InitPermutation();
                webviewReport.NavigateToString(GetHtmlContentFromJson(_equation.ToJson()));
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
            for (var i = 0; i < gridInstantiate.Rows.Count; i++)
            {
                gridInstantiate.Rows[i].DefaultCellStyle.BackColor = Color.White;

                var cv = gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].Value;
                if (cv == null || !BigInteger.TryParse(cv.ToString()!, out var value))
                {
                    continue;
                }

                if (value > 0)
                {
                    gridInstantiate.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                }
                else if (value < 0)
                {
                    gridInstantiate.Rows[i].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                }
            }
        }

        private void Instantiate()
        {
            BigInteger[]? coefficients;
            try
            {
                List<BigInteger> parameters = new();
                for (var i = 0; i < gridInstantiate.Rows.Count; i++)
                {
                    if (!Boolean.Parse(gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnIsFreeVariable.Name].Value.ToString()!))
                    {
                        continue;
                    }

                    var cv = gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].Value ?? throw new FormatException();
                    parameters.Add(BigInteger.Parse(cv.ToString()!));
                }

                coefficients = _equation.RowsBasedSolution.Instantiate(parameters.ToArray());
                textboxInstantiate.Text = _equation.EquationWithIntegerCoefficients(coefficients);
            }
            catch (FormatException)
            {
                textboxInstantiate.Text = "Parsing error occurred";
                coefficients = null;
            }
            catch (AppSpecificException)
            {
                textboxInstantiate.Text = "Failed to get valid coefficients";
                coefficients = null;
            }

            for (var i = 0; i < gridInstantiate.Rows.Count; i++)
            {
                var cv = gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnIsFreeVariable.Name].Value ?? throw new InvalidOperationException();
                var isFreeVarRow = Boolean.Parse(cv.ToString()!);
                if (!isFreeVarRow)
                {
                    gridInstantiate.Rows[i].Cells[columnName: gridInstantiateColumnValue.Name].Value = coefficients != null ? coefficients[i] : "#VALUE!";
                }
            }

            ApplyGridVisuals();
        }
    }
}
