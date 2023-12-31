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
        private void OnClick_buttonBalance(Object sender, EventArgs e)
        {
            Balance();
        }

        private void OnTextChanged_txtInput(Object sender, EventArgs e)
        {
            ResetControls();
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

        private void OnClick_buttonInstantiate(Object sender, EventArgs e)
        {
            RunTools(toolId: 1);
        }

        private void OnClick_buttonCombine(Object sender, EventArgs e)
        {
            RunTools(toolId: 2);
        }

        private void OnCellEndEdit_gridInstantiate(Object sender, DataGridViewCellEventArgs e)
        {
            textboxFinalResult.Clear();
            gridCombine.ClearSelection();
        }

        private void OnCellEndEdit_gridCombine(Object sender, DataGridViewCellEventArgs e)
        {
            textboxFinalResult.Clear();
            gridInstantiate.ClearSelection();
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

            #region setup 'Permute'
            foreach (var substance in _equation.Substances)
            {
                listPermutator.Items.Add(substance);
            }
            #endregion

            if (_equation.RowsBasedSolution.Success)
            {
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
            }

            gridCombine.Visible = _equation.ColumnsBasedSolution is { Success: true, IndependentSetsOfCoefficients.Count: > 1 };
            if (_equation.ColumnsBasedSolution.Success)
            {
                gridCombine.RowCount = _equation.ColumnsBasedSolution.IndependentSetsOfCoefficients!.Count;
                for (var i = 0; i < _equation.ColumnsBasedSolution.IndependentSetsOfCoefficients.Count; i++)
                {
                    gridCombine.Rows[i].Cells[gridCombineColumnEquilibrium.Name].Value =
                        _equation.EquationWithIntegerCoefficients(_equation.ColumnsBasedSolution.IndependentSetsOfCoefficients[i]);
                    gridCombine.Rows[i].Cells[gridCombineColumnCount.Name].Value = 0;
                }
            }

            webviewReport.NavigateToString(GetHtmlContentFromJson(_equation.ToJson()));

            RunTools(toolId: 1);
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

        private void RunTools(Byte toolId)
        {
            BigInteger[]? coefficients;

            try
            {
                coefficients = toolId switch
                {
                    1 => _equation.RowsBasedSolution.Instantiate(_equation.RowsBasedSolution.FreeCoefficientIndices
                                                                          .Select(selector: index => BigInteger.Parse(
                                                                                                (gridInstantiate.Rows[index]
                                                                                                     .Cells[gridInstantiateColumnValue.Name].Value
                                                                                              ?? throw new FormatException()).ToString()!))
                                                                          .ToArray())
                  , 2 => _equation.ColumnsBasedSolution.CombineIndependents(_equation.ColumnsBasedSolution.IndependentSetsOfCoefficients!
                                                                                     .Select(selector: (_, i) => Int32.Parse(
                                                                                                           (gridCombine.Rows[i]
                                                                                                                   .Cells[gridCombineColumnCount.Name].Value
                                                                                                            ?? throw new FormatException()).ToString()!))
                                                                                     .ToArray())
                  , _ => throw new InvalidOperationException()
                };
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

            if (toolId == 1)
            {
                // 'instantiate' was used, recalculate 'combine'
                var strings = Enumerable.Repeat(element: "#VALUE!", gridCombine.RowCount).ToArray();
                if (coefficients != null)
                {
                    var combination = _equation.ColumnsBasedSolution.FindCombination(coefficients);
                    if (combination != null)
                    {
                        strings = combination.Select(selector: static i => i.ToString()).ToArray();
                    }
                }

                for (var i = 0; i < gridCombine.Rows.Count; i++)
                {
                    gridCombine.Rows[i].Cells[gridCombineColumnCount.Name].Value = strings[i];
                }
            }

            ApplyGridVisuals();
        }
    }
}
