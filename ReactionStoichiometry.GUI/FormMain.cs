using System.Numerics;
using System.Windows.Forms;

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
        private void OnClick_buttonBalance(Object sender, EventArgs e)
        {
            Balance();
        }

        private void OnTextChanged_txtInput(Object sender, EventArgs e)
        {
            ResetControls();
        }

        private void OnCellEndEdit_gridInstantiate(Object sender, DataGridViewCellEventArgs e)
        {
            RunTools(priorityTool: 1);
        }

        private void OnMouseDoubleClick_listPermutator(Object sender, MouseEventArgs e)
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

            txtInput.Text = String.Join(separator: " + ", listPermutator.Items.OfType<String>()) + " = 0";
            Balance();
        }

        private void OnCellEndEdit_gridCombine(Object sender, DataGridViewCellEventArgs e)
        {
            RunTools(priorityTool: 2);
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
            textboxFinalResult.Text = String.Empty;
            listPermutator.Items.Clear();
            gridInstantiate.Rows.Clear();
            gridCombine.Rows.Clear();

            buttonBalance.Enabled = ChemicalReactionEquation.IsValidString(txtInput.Text);
        }

        private void Balance()
        {
            ResetControls();

            var s = txtInput.Text;
            _equation = new ChemicalReactionEquation(s);
            txtGeneralForm.Text = _equation.InGeneralForm;

            if (_equation.RowsBasedSolution.Success)
            {
                #region setup 'Instantiate'
                gridInstantiate.RowCount = _equation.Substances.Count;
                for (var i = 0; i < _equation.Substances.Count; i++)
                {
                    gridInstantiate.Rows[i].Cells[gridInstantiateColumnSubstance.Name].Value = _equation.Substances[i];

                    var expr = _equation.RowsBasedSolution.AlgebraicExpressions![i];
                    gridInstantiate.Rows[i].Cells[gridInstantiateColumnCoefficient.Name].Value = expr;

                    if (expr.Contains(value: " = "))
                    {
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnIsFreeVariable.Name].Value = false;
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].ReadOnly = true;
                    }
                    else
                    {
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnIsFreeVariable.Name].Value = true;
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].ReadOnly = false;
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].Value = _equation.RowsBasedSolution.InstanceSample![i];
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].Style.Font =
                            new Font(gridInstantiate.Font, FontStyle.Bold | FontStyle.Underline);
                    }
                }
                #endregion

                #region setup 'Combine'
                gridCombine.RowCount = _equation.ColumnsBasedSolution.IndependentSetsOfCoefficients.Count;
                for (var i = 0; i < _equation.ColumnsBasedSolution.IndependentSetsOfCoefficients.Count; i++)
                {
                    gridCombine.Rows[i].Cells[gridCombineColumnEquilibrium.Name].Value =
                        _equation.EquationWithIntegerCoefficients(_equation.ColumnsBasedSolution.IndependentSetsOfCoefficients[i]);
                    gridCombine.Rows[i].Cells[gridCombineColumnCount.Name].Value = 0;
                }
                #endregion

                #region setup 'Permute'
                foreach (var substance in _equation.Substances)
                {
                    listPermutator.Items.Add(substance);
                }
                #endregion

                webviewReport.NavigateToString(GetHtmlContentFromJson(_equation.ToJson()));
                theTabControl.Enabled = true;

                RunTools(priorityTool: 1);
            }
            else
            {
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

        private void ApplyGridVisuals()
        {
            for (var i = 0; i < gridInstantiate.Rows.Count; i++)
            {
                gridInstantiate.Rows[i].DefaultCellStyle.BackColor = Color.White;

                var cv = gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].Value;
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

        private void RunTools(Byte priorityTool)
        {
            switch (priorityTool)
            {
                case 1:
                {
                    BigInteger[]? coefficients;
                    try
                    {
                        List<BigInteger> parameters = new();
                        for (var i = 0; i < gridInstantiate.Rows.Count; i++)
                        {
                            if (!Boolean.Parse(gridInstantiate.Rows[i].Cells[gridInstantiateColumnIsFreeVariable.Name].Value.ToString()!))
                            {
                                continue;
                            }

                            var cv = gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].Value ?? throw new FormatException();
                            parameters.Add(BigInteger.Parse(cv.ToString()!));
                        }

                        coefficients = _equation.RowsBasedSolution.Instantiate(parameters.ToArray());
                        textboxFinalResult.Text = _equation.EquationWithIntegerCoefficients(coefficients);
                    }
                    catch (FormatException)
                    {
                        textboxFinalResult.Text = "Parsing error occurred";
                        coefficients = null;
                    }
                    catch (AppSpecificException)
                    {
                        textboxFinalResult.Text = "Failed to get valid coefficients";
                        coefficients = null;
                    }

                    for (var i = 0; i < gridInstantiate.Rows.Count; i++)
                    {
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].Value = coefficients != null ? coefficients[i] : "#VALUE!";
                    }
                    for (var i = 0; i < gridCombine.Rows.Count; i++)
                    {
                        gridCombine.Rows[i].Cells[gridCombineColumnCount.Name].Value = "(?)";
                    }
                    break;
                }
                case 2:
                {
                    BigInteger[]? coefficients;
                    try
                    {
                        List<Int32> combination = new();
                        for (var i = 0; i < gridCombine.Rows.Count; i++)
                        {
                            var cv = gridCombine.Rows[i].Cells[gridCombineColumnCount.Name].Value ?? throw new FormatException();
                            combination.Add(Int32.Parse(cv.ToString()!));
                        }

                        coefficients = _equation.ColumnsBasedSolution.CombineIndependents(combination.ToArray());
                        textboxFinalResult.Text = _equation.EquationWithIntegerCoefficients(coefficients);
                    }
                    catch (FormatException)
                    {
                        textboxFinalResult.Text = "Parsing error occurred";
                        coefficients = null;
                    }
                    catch (AppSpecificException)
                    {
                        textboxFinalResult.Text = "Failed to get valid coefficients";
                        coefficients = null;
                    }

                    for (var i = 0; i < gridInstantiate.Rows.Count; i++)
                    {
                        gridInstantiate.Rows[i].Cells[gridInstantiateColumnValue.Name].Value = coefficients != null ? coefficients[i] : "#VALUE!";
                    }
                    break;
                }
            }

            ApplyGridVisuals();
        }

        private void OnSelectionChanged(Object sender, EventArgs e)
        {
            var currentGrid = sender as DataGridView;

            if (currentGrid == gridCombine)
            {
                gridInstantiate.ClearSelection();
            }
            else if (currentGrid == gridInstantiate)
            {
                gridCombine.ClearSelection();
            }
        }
    }
}
