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
            containerInput = new GroupBox();
            textBoxInput = new TextBox();
            buttonBalance = new Button();
            txtGeneralForm = new TextBox();
            theTabControl = new TabControl();
            tabHtml = new TabPage();
            webviewResult = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabInstantiate = new TabPage();
            gridCoefficients = new DataGridView();
            Substance = new DataGridViewTextBoxColumn();
            Coefficient = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            IsFreeVariable = new DataGridViewCheckBoxColumn();
            txtInstance = new TextBox();
            tabPermutate = new TabPage();
            listPermutator = new ListBox();
            containerInput.SuspendLayout();
            theTabControl.SuspendLayout();
            tabHtml.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) webviewResult).BeginInit();
            tabInstantiate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) gridCoefficients).BeginInit();
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
            textBoxInput.Text = "TiO2 + C + Cl2 = TiCl4 + CO + CO2";
            textBoxInput.TextChanged += On_textBoxInput_TextChanged;
            // 
            // buttonBalance
            // 
            buttonBalance.Dock = DockStyle.Right;
            buttonBalance.Enabled = false;
            buttonBalance.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
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
            txtGeneralForm.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
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
            theTabControl.Controls.Add(tabInstantiate);
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
            tabHtml.Controls.Add(webviewResult);
            tabHtml.Location = new Point(10, 58);
            tabHtml.Name = "tabHtml";
            tabHtml.Padding = new Padding(3);
            tabHtml.Size = new Size(1868, 651);
            tabHtml.TabIndex = 0;
            tabHtml.Text = "Result";
            tabHtml.UseVisualStyleBackColor = true;
            // 
            // webviewResult
            // 
            webviewResult.AllowExternalDrop = false;
            webviewResult.CreationProperties = null;
            webviewResult.DefaultBackgroundColor = Color.White;
            webviewResult.Dock = DockStyle.Fill;
            webviewResult.Location = new Point(3, 3);
            webviewResult.Name = "webviewResult";
            webviewResult.Size = new Size(1862, 645);
            webviewResult.TabIndex = 0;
            webviewResult.ZoomFactor = 1D;
            // 
            // tabInstantiate
            // 
            tabInstantiate.Controls.Add(gridCoefficients);
            tabInstantiate.Controls.Add(txtInstance);
            tabInstantiate.Location = new Point(10, 58);
            tabInstantiate.Name = "tabInstantiate";
            tabInstantiate.Padding = new Padding(3);
            tabInstantiate.Size = new Size(1868, 651);
            tabInstantiate.TabIndex = 2;
            tabInstantiate.Text = "Instantiate";
            tabInstantiate.UseVisualStyleBackColor = true;
            // 
            // gridCoefficients
            // 
            gridCoefficients.AllowUserToAddRows = false;
            gridCoefficients.AllowUserToDeleteRows = false;
            gridCoefficients.AllowUserToOrderColumns = true;
            gridCoefficients.AllowUserToResizeRows = false;
            gridCoefficients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gridCoefficients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gridCoefficients.ColumnHeadersHeight = 58;
            gridCoefficients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridCoefficients.Columns.AddRange(new DataGridViewColumn[] { Substance, Coefficient, Value, IsFreeVariable });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            gridCoefficients.DefaultCellStyle = dataGridViewCellStyle2;
            gridCoefficients.Dock = DockStyle.Fill;
            gridCoefficients.Location = new Point(3, 3);
            gridCoefficients.Name = "gridCoefficients";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.Padding = new Padding(8, 2, 0, 2);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            gridCoefficients.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            gridCoefficients.RowHeadersWidth = 58;
            gridCoefficients.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gridCoefficients.RowTemplate.Height = 49;
            gridCoefficients.ScrollBars = ScrollBars.Vertical;
            gridCoefficients.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridCoefficients.ShowCellToolTips = false;
            gridCoefficients.ShowEditingIcon = false;
            gridCoefficients.Size = new Size(1862, 598);
            gridCoefficients.TabIndex = 4;
            gridCoefficients.CellEndEdit += OnCellEndEdit;
            // 
            // Substance
            // 
            Substance.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Substance.HeaderText = "Substance";
            Substance.MinimumWidth = 12;
            Substance.Name = "Substance";
            Substance.ReadOnly = true;
            Substance.SortMode = DataGridViewColumnSortMode.NotSortable;
            Substance.Width = 167;
            // 
            // Coefficient
            // 
            Coefficient.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Coefficient.HeaderText = "Coefficient";
            Coefficient.MinimumWidth = 12;
            Coefficient.Name = "Coefficient";
            Coefficient.ReadOnly = true;
            Coefficient.SortMode = DataGridViewColumnSortMode.NotSortable;
            Coefficient.Width = 179;
            // 
            // Value
            // 
            Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Value.HeaderText = "Value";
            Value.MinimumWidth = 12;
            Value.Name = "Value";
            Value.ReadOnly = true;
            Value.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // IsFreeVariable
            // 
            IsFreeVariable.HeaderText = "IsFreeVariable";
            IsFreeVariable.MinimumWidth = 12;
            IsFreeVariable.Name = "IsFreeVariable";
            IsFreeVariable.ReadOnly = true;
            IsFreeVariable.Visible = false;
            IsFreeVariable.Width = 222;
            // 
            // txtInstance
            // 
            txtInstance.Dock = DockStyle.Bottom;
            txtInstance.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            txtInstance.Location = new Point(3, 601);
            txtInstance.Name = "txtInstance";
            txtInstance.ReadOnly = true;
            txtInstance.Size = new Size(1862, 47);
            txtInstance.TabIndex = 5;
            txtInstance.TextAlign = HorizontalAlignment.Center;
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
            ((System.ComponentModel.ISupportInitialize) webviewResult).EndInit();
            tabInstantiate.ResumeLayout(false);
            tabInstantiate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) gridCoefficients).EndInit();
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
        private DataGridView gridCoefficients;
        private TextBox txtGeneralForm;
        private TextBox txtInstance;
        private ListBox listPermutator;
        private Microsoft.Web.WebView2.WinForms.WebView2 webviewResult;
        private DataGridViewTextBoxColumn Substance;
        private DataGridViewTextBoxColumn Coefficient;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
    }
}