namespace ReactionStoichiometry.GUI
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            containerInput = new GroupBox();
            txtInput = new TextBox();
            buttonBalance = new Button();
            txtGeneralForm = new TextBox();
            theTabControl = new TabControl();
            tabHtml = new TabPage();
            webviewReport = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabTools = new TabPage();
            splitTools = new SplitContainer();
            gridInstantiate = new DataGridView();
            gridInstantiateColumnSubstance = new DataGridViewTextBoxColumn();
            gridInstantiateColumnCoefficient = new DataGridViewTextBoxColumn();
            gridInstantiateColumnValue = new DataGridViewTextBoxColumn();
            gridInstantiateColumnIsFreeVariable = new DataGridViewCheckBoxColumn();
            buttonInstantiate = new Button();
            gridCombine = new DataGridView();
            gridCombineColumnEquilibrium = new DataGridViewTextBoxColumn();
            gridCombineColumnCount = new DataGridViewTextBoxColumn();
            buttonCombine = new Button();
            tabPermutate = new TabPage();
            listPermutator = new ListBox();
            textboxFinalResult = new TextBox();
            containerInput.SuspendLayout();
            theTabControl.SuspendLayout();
            tabHtml.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) webviewReport).BeginInit();
            tabTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitTools).BeginInit();
            splitTools.Panel1.SuspendLayout();
            splitTools.Panel2.SuspendLayout();
            splitTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) gridInstantiate).BeginInit();
            ((System.ComponentModel.ISupportInitialize) gridCombine).BeginInit();
            tabPermutate.SuspendLayout();
            SuspendLayout();
            // 
            // containerInput
            // 
            containerInput.Controls.Add(txtInput);
            containerInput.Controls.Add(buttonBalance);
            containerInput.Controls.Add(txtGeneralForm);
            containerInput.Dock = DockStyle.Top;
            containerInput.Location = new Point(0, 0);
            containerInput.Name = "containerInput";
            containerInput.Size = new Size(1888, 273);
            containerInput.TabIndex = 9;
            containerInput.TabStop = false;
            containerInput.Text = "Input";
            // 
            // txtInput
            // 
            txtInput.Dock = DockStyle.Fill;
            txtInput.Location = new Point(3, 43);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.Size = new Size(1694, 180);
            txtInput.TabIndex = 5;
            txtInput.Text = "CO+CO2+H2=CH4+H2O";
            txtInput.TextChanged += OnTextChanged_txtInput;
            // 
            // buttonBalance
            // 
            buttonBalance.Dock = DockStyle.Right;
            buttonBalance.Enabled = false;
            buttonBalance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonBalance.Location = new Point(1697, 43);
            buttonBalance.Name = "buttonBalance";
            buttonBalance.Size = new Size(188, 180);
            buttonBalance.TabIndex = 6;
            buttonBalance.Text = "Start";
            buttonBalance.UseVisualStyleBackColor = true;
            buttonBalance.Click += OnClick_buttonBalance;
            // 
            // txtGeneralForm
            // 
            txtGeneralForm.Dock = DockStyle.Bottom;
            txtGeneralForm.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point,  204);
            txtGeneralForm.Location = new Point(3, 223);
            txtGeneralForm.Name = "txtGeneralForm";
            txtGeneralForm.ReadOnly = true;
            txtGeneralForm.Size = new Size(1882, 47);
            txtGeneralForm.TabIndex = 7;
            txtGeneralForm.TextAlign = HorizontalAlignment.Center;
            // 
            // theTabControl
            // 
            theTabControl.Controls.Add(tabHtml);
            theTabControl.Controls.Add(tabTools);
            theTabControl.Controls.Add(tabPermutate);
            theTabControl.Dock = DockStyle.Fill;
            theTabControl.Location = new Point(0, 273);
            theTabControl.Name = "theTabControl";
            theTabControl.SelectedIndex = 0;
            theTabControl.Size = new Size(1888, 672);
            theTabControl.SizeMode = TabSizeMode.Fixed;
            theTabControl.TabIndex = 10;
            // 
            // tabHtml
            // 
            tabHtml.Controls.Add(webviewReport);
            tabHtml.Location = new Point(10, 58);
            tabHtml.Name = "tabHtml";
            tabHtml.Padding = new Padding(3);
            tabHtml.Size = new Size(1868, 604);
            tabHtml.TabIndex = 0;
            tabHtml.Text = "Report";
            tabHtml.UseVisualStyleBackColor = true;
            // 
            // webviewReport
            // 
            webviewReport.AllowExternalDrop = false;
            webviewReport.CreationProperties = null;
            webviewReport.DefaultBackgroundColor = Color.White;
            webviewReport.Dock = DockStyle.Fill;
            webviewReport.Location = new Point(3, 3);
            webviewReport.Name = "webviewReport";
            webviewReport.Size = new Size(1862, 598);
            webviewReport.TabIndex = 0;
            webviewReport.ZoomFactor = 1D;
            // 
            // tabTools
            // 
            tabTools.Controls.Add(splitTools);
            tabTools.Location = new Point(10, 58);
            tabTools.Name = "tabTools";
            tabTools.Size = new Size(1868, 604);
            tabTools.TabIndex = 5;
            tabTools.Text = "Tools";
            tabTools.UseVisualStyleBackColor = true;
            // 
            // splitTools
            // 
            splitTools.Dock = DockStyle.Fill;
            splitTools.Location = new Point(0, 0);
            splitTools.Name = "splitTools";
            // 
            // splitTools.Panel1
            // 
            splitTools.Panel1.Controls.Add(gridInstantiate);
            splitTools.Panel1.Controls.Add(buttonInstantiate);
            // 
            // splitTools.Panel2
            // 
            splitTools.Panel2.Controls.Add(gridCombine);
            splitTools.Panel2.Controls.Add(buttonCombine);
            splitTools.Size = new Size(1868, 604);
            splitTools.SplitterDistance = 934;
            splitTools.TabIndex = 0;
            // 
            // gridInstantiate
            // 
            gridInstantiate.AllowUserToAddRows = false;
            gridInstantiate.AllowUserToDeleteRows = false;
            gridInstantiate.AllowUserToOrderColumns = true;
            gridInstantiate.AllowUserToResizeRows = false;
            gridInstantiate.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gridInstantiate.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gridInstantiate.ColumnHeadersHeight = 58;
            gridInstantiate.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridInstantiate.Columns.AddRange(new DataGridViewColumn[] { gridInstantiateColumnSubstance, gridInstantiateColumnCoefficient, gridInstantiateColumnValue, gridInstantiateColumnIsFreeVariable });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            gridInstantiate.DefaultCellStyle = dataGridViewCellStyle2;
            gridInstantiate.Dock = DockStyle.Fill;
            gridInstantiate.Location = new Point(0, 58);
            gridInstantiate.Name = "gridInstantiate";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.Padding = new Padding(8, 2, 0, 2);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            gridInstantiate.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            gridInstantiate.RowHeadersWidth = 58;
            gridInstantiate.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gridInstantiate.ScrollBars = ScrollBars.Vertical;
            gridInstantiate.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridInstantiate.ShowCellToolTips = false;
            gridInstantiate.ShowEditingIcon = false;
            gridInstantiate.Size = new Size(934, 546);
            gridInstantiate.TabIndex = 6;
            gridInstantiate.CellEndEdit += OnCellEndEdit_gridInstantiate;
            // 
            // gridInstantiateColumnSubstance
            // 
            gridInstantiateColumnSubstance.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            gridInstantiateColumnSubstance.HeaderText = "Substance";
            gridInstantiateColumnSubstance.MinimumWidth = 12;
            gridInstantiateColumnSubstance.Name = "gridInstantiateColumnSubstance";
            gridInstantiateColumnSubstance.ReadOnly = true;
            gridInstantiateColumnSubstance.SortMode = DataGridViewColumnSortMode.NotSortable;
            gridInstantiateColumnSubstance.Width = 167;
            // 
            // gridInstantiateColumnCoefficient
            // 
            gridInstantiateColumnCoefficient.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            gridInstantiateColumnCoefficient.HeaderText = "Coefficient";
            gridInstantiateColumnCoefficient.MinimumWidth = 12;
            gridInstantiateColumnCoefficient.Name = "gridInstantiateColumnCoefficient";
            gridInstantiateColumnCoefficient.ReadOnly = true;
            gridInstantiateColumnCoefficient.SortMode = DataGridViewColumnSortMode.NotSortable;
            gridInstantiateColumnCoefficient.Width = 179;
            // 
            // gridInstantiateColumnValue
            // 
            gridInstantiateColumnValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridInstantiateColumnValue.HeaderText = "Value";
            gridInstantiateColumnValue.MinimumWidth = 12;
            gridInstantiateColumnValue.Name = "gridInstantiateColumnValue";
            gridInstantiateColumnValue.ReadOnly = true;
            gridInstantiateColumnValue.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // gridInstantiateColumnIsFreeVariable
            // 
            gridInstantiateColumnIsFreeVariable.HeaderText = "IsFreeVariable";
            gridInstantiateColumnIsFreeVariable.MinimumWidth = 12;
            gridInstantiateColumnIsFreeVariable.Name = "gridInstantiateColumnIsFreeVariable";
            gridInstantiateColumnIsFreeVariable.ReadOnly = true;
            gridInstantiateColumnIsFreeVariable.Visible = false;
            gridInstantiateColumnIsFreeVariable.Width = 222;
            // 
            // buttonInstantiate
            // 
            buttonInstantiate.Dock = DockStyle.Top;
            buttonInstantiate.Location = new Point(0, 0);
            buttonInstantiate.Name = "buttonInstantiate";
            buttonInstantiate.Size = new Size(934, 58);
            buttonInstantiate.TabIndex = 7;
            buttonInstantiate.Text = "Instantiate";
            buttonInstantiate.UseVisualStyleBackColor = true;
            buttonInstantiate.Click += OnClick_buttonInstantiate;
            // 
            // gridCombine
            // 
            gridCombine.AllowUserToAddRows = false;
            gridCombine.AllowUserToDeleteRows = false;
            gridCombine.AllowUserToOrderColumns = true;
            gridCombine.AllowUserToResizeRows = false;
            gridCombine.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            gridCombine.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            gridCombine.ColumnHeadersHeight = 58;
            gridCombine.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridCombine.Columns.AddRange(new DataGridViewColumn[] { gridCombineColumnEquilibrium, gridCombineColumnCount });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.BackColor = SystemColors.Window;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            gridCombine.DefaultCellStyle = dataGridViewCellStyle5;
            gridCombine.Dock = DockStyle.Fill;
            gridCombine.Location = new Point(0, 58);
            gridCombine.Name = "gridCombine";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.BackColor = SystemColors.Control;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle6.Padding = new Padding(8, 2, 0, 2);
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            gridCombine.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            gridCombine.RowHeadersWidth = 58;
            gridCombine.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gridCombine.ScrollBars = ScrollBars.Vertical;
            gridCombine.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridCombine.ShowCellToolTips = false;
            gridCombine.ShowEditingIcon = false;
            gridCombine.Size = new Size(930, 546);
            gridCombine.TabIndex = 6;
            gridCombine.CellEndEdit += OnCellEndEdit_gridCombine;
            // 
            // gridCombineColumnEquilibrium
            // 
            gridCombineColumnEquilibrium.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridCombineColumnEquilibrium.HeaderText = "Equilibrium";
            gridCombineColumnEquilibrium.MinimumWidth = 12;
            gridCombineColumnEquilibrium.Name = "gridCombineColumnEquilibrium";
            gridCombineColumnEquilibrium.ReadOnly = true;
            gridCombineColumnEquilibrium.Resizable = DataGridViewTriState.False;
            gridCombineColumnEquilibrium.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // gridCombineColumnCount
            // 
            gridCombineColumnCount.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            gridCombineColumnCount.HeaderText = "Count";
            gridCombineColumnCount.MinimumWidth = 12;
            gridCombineColumnCount.Name = "gridCombineColumnCount";
            gridCombineColumnCount.Resizable = DataGridViewTriState.False;
            gridCombineColumnCount.SortMode = DataGridViewColumnSortMode.NotSortable;
            gridCombineColumnCount.Width = 109;
            // 
            // buttonCombine
            // 
            buttonCombine.Dock = DockStyle.Top;
            buttonCombine.Location = new Point(0, 0);
            buttonCombine.Name = "buttonCombine";
            buttonCombine.Size = new Size(930, 58);
            buttonCombine.TabIndex = 7;
            buttonCombine.Text = "Combine";
            buttonCombine.UseVisualStyleBackColor = true;
            buttonCombine.Click += OnClick_buttonCombine;
            // 
            // tabPermutate
            // 
            tabPermutate.Controls.Add(listPermutator);
            tabPermutate.Location = new Point(10, 58);
            tabPermutate.Name = "tabPermutate";
            tabPermutate.Size = new Size(1868, 604);
            tabPermutate.TabIndex = 3;
            tabPermutate.Text = "Permutate";
            tabPermutate.UseVisualStyleBackColor = true;
            // 
            // listPermutator
            // 
            listPermutator.Dock = DockStyle.Fill;
            listPermutator.ItemHeight = 41;
            listPermutator.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            listPermutator.Location = new Point(0, 0);
            listPermutator.Name = "listPermutator";
            listPermutator.ScrollAlwaysVisible = true;
            listPermutator.Size = new Size(1868, 604);
            listPermutator.TabIndex = 2;
            listPermutator.TabStop = false;
            listPermutator.MouseDoubleClick += OnMouseDoubleClick_listPermutator;
            // 
            // textboxFinalResult
            // 
            textboxFinalResult.Dock = DockStyle.Bottom;
            textboxFinalResult.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point,  204);
            textboxFinalResult.Location = new Point(0, 945);
            textboxFinalResult.Name = "textboxFinalResult";
            textboxFinalResult.ReadOnly = true;
            textboxFinalResult.Size = new Size(1888, 47);
            textboxFinalResult.TabIndex = 11;
            textboxFinalResult.TextAlign = HorizontalAlignment.Center;
            // 
            // FormMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1888, 992);
            Controls.Add(theTabControl);
            Controls.Add(containerInput);
            Controls.Add(textboxFinalResult);
            DoubleBuffered = true;
            MinimumSize = new Size(1920, 1080);
            Name = "FormMain";
            Text = "Balancing algo";
            containerInput.ResumeLayout(false);
            containerInput.PerformLayout();
            theTabControl.ResumeLayout(false);
            tabHtml.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) webviewReport).EndInit();
            tabTools.ResumeLayout(false);
            splitTools.Panel1.ResumeLayout(false);
            splitTools.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitTools).EndInit();
            splitTools.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) gridInstantiate).EndInit();
            ((System.ComponentModel.ISupportInitialize) gridCombine).EndInit();
            tabPermutate.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox containerInput;
        private Button buttonBalance;
        private TextBox txtInput;
        private TabControl theTabControl;
        private TabPage tabHtml;
        private TabPage tabPermutate;
        private TextBox txtGeneralForm;
        private ListBox listPermutator;
        private Microsoft.Web.WebView2.WinForms.WebView2 webviewReport;
        private TabPage tabTools;
        private SplitContainer splitTools;
        private DataGridView gridInstantiate;
        private DataGridViewTextBoxColumn gridInstantiateColumnSubstance;
        private DataGridViewTextBoxColumn gridInstantiateColumnCoefficient;
        private DataGridViewTextBoxColumn gridInstantiateColumnValue;
        private DataGridViewCheckBoxColumn gridInstantiateColumnIsFreeVariable;
        private DataGridView gridCombine;
        private TextBox textboxFinalResult;
        private DataGridViewTextBoxColumn gridCombineColumnEquilibrium;
        private DataGridViewTextBoxColumn gridCombineColumnCount;
        private Button buttonInstantiate;
        private Button buttonCombine;
    }
}