using System.Numerics;
using Microsoft.Web.WebView2.Core;

namespace ReactionStoichiometry.GUI
{
    internal sealed partial class FormMain : Form
    {
        private BalancerGeneralized _balancer = null!;

        internal FormMain()
        {
            InitializeComponent();
            InitializeWebView();
            ResetControls();
        }

        private async void InitializeWebView()
        {
            await webviewPrintable.EnsureCoreWebView2Async(environment: null);
            webviewPrintable.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
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

            var s = String.Join(separator: "+", listPermutator.Items.OfType<String>()) + "=0";
            textBoxInput.Text = s;
            Balance();
        }
        #endregion

        private async void ResetControls()
        {
            await webviewPrintable.EnsureCoreWebView2Async(environment: null);
            webviewPrintable.NavigateToString(String.Empty);
            txtGeneralForm.Text = String.Empty;
            txtInstance.Text = String.Empty;
            listPermutator.Items.Clear();
            gridCoefficients.Rows.Clear();
            theTabControl.Enabled = false;

            buttonBalance.Enabled = textBoxInput.Text.LooksLikeChemicalReactionEquation();
        }

        private void Balance()
        {
            var s = textBoxInput.Text;

            _balancer = new BalancerGeneralized(s);
            txtGeneralForm.Text = _balancer.Equation.GeneralizedEquation;

            if (_balancer.Balance())
            {
                InitInstantiation();
                InitPermutation();
                webviewPrintable.NavigateToString(GetHtmlContentFromJson(_balancer.ToString(Balancer.OutputFormat.Json)));
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
            var htmlContent = WebViewResources.htmlContent.Replace(oldValue: "%jsContent%", WebViewResources.jsContent)
                                              .Replace(oldValue: "%cssContent%", WebViewResources.cssContent)
                                              .Replace(oldValue: "%jsonContent%", jsonContent);
            return htmlContent;
        }

        private void InitInstantiation()
        {
            gridCoefficients.RowCount = _balancer.Equation.Substances.Count;
            for (var i = 0; i < _balancer.Equation.Substances.Count; i++)
            {
                gridCoefficients.Rows[i].HeaderCell.Value = _balancer.Equation.LabelFor(i);
                gridCoefficients.Rows[i].Cells[columnName: "Substance"].Value = _balancer.Equation.Substances[i];

                var expr = _balancer.AlgebraicExpressionForCoefficient(i);
                var propose = _balancer.GuessedSimplestSolution.singleFreeVarValue;

                if (String.IsNullOrEmpty(expr))
                {
                    gridCoefficients.Rows[i].Cells[columnName: "IsFreeVariable"].Value = true;
                    gridCoefficients.Rows[i].Cells[columnName: "Expression"].Value = "\u27a2";
                    gridCoefficients.Rows[i].Cells[columnName: "Value"].Value = propose ?? 0;
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
            listPermutator.Items.Clear();
            foreach (var s in _balancer.Equation.Substances)
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

                coefficients = _balancer.Instantiate(parameters.ToArray());
                txtInstance.Text = _balancer.Equation.EquationWithIntegerCoefficients(coefficients);
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
