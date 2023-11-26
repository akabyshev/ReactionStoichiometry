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
            textBoxInput = new TextBox();
            buttonBalance = new Button();
            txtGeneralForm = new TextBox();
            theTabControl = new TabControl();
            tabHtml = new TabPage();
            webviewReport = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabInstantiate = new TabPage();
            gridInstantiate = new DataGridView();
            gridInstantiateColumnSubstance = new DataGridViewTextBoxColumn();
            gridInstantiateColumnCoefficient = new DataGridViewTextBoxColumn();
            gridInstantiateColumnValue = new DataGridViewTextBoxColumn();
            gridInstantiateColumnIsFreeVariable = new DataGridViewCheckBoxColumn();
            textboxInstantiate = new TextBox();
            tabCombine = new TabPage();
            gridCombine = new DataGridView();
            gridCombineColumnEquilibrium = new DataGridViewTextBoxColumn();
            gridCombineColumnCount = new DataGridViewTextBoxColumn();
            tabPermutate = new TabPage();
            listPermutator = new ListBox();
            tabTools = new TabPage();
            containerInput.SuspendLayout();
            theTabControl.SuspendLayout();
            tabHtml.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) webviewReport).BeginInit();
            tabInstantiate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) gridInstantiate).BeginInit();
            tabCombine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) gridCombine).BeginInit();
            tabPermutate.SuspendLayout();
            SuspendLayout();
            // 
            // containerInput
            // 
            containerInput.Controls.Add(textBoxInput);
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
            // textBoxInput
            // 
            textBoxInput.Dock = DockStyle.Fill;
            textBoxInput.Location = new Point(3, 43);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(1694, 180);
            textBoxInput.TabIndex = 5;
            textBoxInput.Text = "IO4Qn+IQn=I3Qn+H2O+OHQn+IO3Qn";
            textBoxInput.TextChanged += On_textBoxInput_TextChanged;
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
            buttonBalance.Click += On_buttonBalance_Click;
            // 
            // txtGeneralForm
            // 
            txtGeneralForm.Dock = DockStyle.Bottom;
            txtGeneralForm.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
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
            theTabControl.Controls.Add(tabInstantiate);
            theTabControl.Controls.Add(tabCombine);
            theTabControl.Controls.Add(tabPermutate);
            theTabControl.Dock = DockStyle.Fill;
            theTabControl.Location = new Point(0, 273);
            theTabControl.Name = "theTabControl";
            theTabControl.SelectedIndex = 0;
            theTabControl.Size = new Size(1888, 719);
            theTabControl.TabIndex = 10;
            // 
            // tabHtml
            // 
            tabHtml.Controls.Add(webviewReport);
            tabHtml.Location = new Point(10, 58);
            tabHtml.Name = "tabHtml";
            tabHtml.Padding = new Padding(3);
            tabHtml.Size = new Size(1868, 651);
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
            webviewReport.Size = new Size(1862, 645);
            webviewReport.TabIndex = 0;
            webviewReport.ZoomFactor = 1D;
            // 
            // tabInstantiate
            // 
            tabInstantiate.Controls.Add(gridInstantiate);
            tabInstantiate.Controls.Add(textboxInstantiate);
            tabInstantiate.Location = new Point(10, 58);
            tabInstantiate.Name = "tabInstantiate";
            tabInstantiate.Padding = new Padding(3);
            tabInstantiate.Size = new Size(1868, 651);
            tabInstantiate.TabIndex = 2;
            tabInstantiate.Text = "Instantiate";
            tabInstantiate.UseVisualStyleBackColor = true;
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
            gridInstantiate.Location = new Point(3, 3);
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
            gridInstantiate.Size = new Size(1862, 598);
            gridInstantiate.TabIndex = 4;
            gridInstantiate.CellEndEdit += OnCellEndEdit;
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
            // textboxInstantiate
            // 
            textboxInstantiate.Dock = DockStyle.Bottom;
            textboxInstantiate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            textboxInstantiate.Location = new Point(3, 601);
            textboxInstantiate.Name = "textboxInstantiate";
            textboxInstantiate.ReadOnly = true;
            textboxInstantiate.Size = new Size(1862, 47);
            textboxInstantiate.TabIndex = 5;
            textboxInstantiate.TextAlign = HorizontalAlignment.Center;
            // 
            // tabCombine
            // 
            tabCombine.Controls.Add(gridCombine);
            tabCombine.Location = new Point(10, 58);
            tabCombine.Name = "tabCombine";
            tabCombine.Size = new Size(1868, 651);
            tabCombine.TabIndex = 4;
            tabCombine.Text = "Combine";
            tabCombine.UseVisualStyleBackColor = true;
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
            gridCombine.Location = new Point(0, 0);
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
            gridCombine.Size = new Size(1868, 651);
            gridCombine.TabIndex = 5;
            // 
            // gridCombineColumnEquilibrium
            // 
            gridCombineColumnEquilibrium.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridCombineColumnEquilibrium.HeaderText = "Equilibrium";
            gridCombineColumnEquilibrium.MinimumWidth = 12;
            gridCombineColumnEquilibrium.Name = "gridCombineColumnEquilibrium";
            gridCombineColumnEquilibrium.ReadOnly = true;
            gridCombineColumnEquilibrium.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // gridCombineColumnCount
            // 
            gridCombineColumnCount.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            gridCombineColumnCount.HeaderText = "Count";
            gridCombineColumnCount.MinimumWidth = 12;
            gridCombineColumnCount.Name = "gridCombineColumnCount";
            gridCombineColumnCount.ReadOnly = true;
            gridCombineColumnCount.SortMode = DataGridViewColumnSortMode.NotSortable;
            gridCombineColumnCount.Width = 109;
            // 
            // tabPermutate
            // 
            tabPermutate.Controls.Add(listPermutator);
            tabPermutate.Location = new Point(10, 58);
            tabPermutate.Name = "tabPermutate";
            tabPermutate.Size = new Size(1868, 651);
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
            listPermutator.Size = new Size(1868, 651);
            listPermutator.TabIndex = 2;
            listPermutator.TabStop = false;
            listPermutator.MouseDoubleClick += OnListMouseDoubleClick;
            // 
            // tabTools
            // 
            tabTools.Location = new Point(10, 58);
            tabTools.Name = "tabTools";
            tabTools.Size = new Size(1868, 651);
            tabTools.TabIndex = 5;
            tabTools.Text = "Tools";
            tabTools.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1888, 992);
            Controls.Add(theTabControl);
            Controls.Add(containerInput);
            DoubleBuffered = true;
            MinimumSize = new Size(1920, 1080);
            Name = "FormMain";
            Text = "Balancing algo";
            containerInput.ResumeLayout(false);
            containerInput.PerformLayout();
            theTabControl.ResumeLayout(false);
            tabHtml.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) webviewReport).EndInit();
            tabInstantiate.ResumeLayout(false);
            tabInstantiate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) gridInstantiate).EndInit();
            tabCombine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) gridCombine).EndInit();
            tabPermutate.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox containerInput;
        private Button buttonBalance;
        private TextBox textBoxInput;
        private TabControl theTabControl;
        private TabPage tabHtml;
        private TabPage tabInstantiate;
        private TabPage tabPermutate;
        private DataGridView gridInstantiate;
        private TextBox txtGeneralForm;
        private TextBox textboxInstantiate;
        private ListBox listPermutator;
        private Microsoft.Web.WebView2.WinForms.WebView2 webviewReport;
        private TabPage tabCombine;
        private DataGridView gridCombine;
        private DataGridViewTextBoxColumn gridInstantiateColumnSubstance;
        private DataGridViewTextBoxColumn gridInstantiateColumnCoefficient;
        private DataGridViewTextBoxColumn gridInstantiateColumnValue;
        private DataGridViewCheckBoxColumn gridInstantiateColumnIsFreeVariable;
        private DataGridViewTextBoxColumn gridCombineColumnEquilibrium;
        private DataGridViewTextBoxColumn gridCombineColumnCount;
        private TabPage tabTools;
    }
}