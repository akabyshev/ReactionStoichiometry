namespace ReactionStoichiometry
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
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle12 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle11 = new DataGridViewCellStyle();
            containerInput = new GroupBox();
            textBoxInput = new TextBox();
            buttonBalance = new Button();
            txtGeneralForm = new TextBox();
            theTabControl = new TabControl();
            tabPlain = new TabPage();
            ctrlDetailedPlain = new TextBox();
            tabHtml = new TabPage();
            ctrlDetailedHtml = new TextBox();
            tabInstantiate = new TabPage();
            txtInstance = new TextBox();
            theGrid = new DataGridView();
            Substance = new DataGridViewTextBoxColumn();
            Expression = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            IsFreeVariable = new DataGridViewCheckBoxColumn();
            tabPermutate = new TabPage();
            listRHS = new ListBox();
            listLHS = new ListBox();
            containerInput.SuspendLayout();
            theTabControl.SuspendLayout();
            tabPlain.SuspendLayout();
            tabHtml.SuspendLayout();
            tabInstantiate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) theGrid).BeginInit();
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
            textBoxInput.Text = "IO4Qn + IQn = IO3Qn + I3Qn + H2O + OHQn";
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
            buttonBalance.Text = "Go!";
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
            theTabControl.Controls.Add(tabInstantiate);
            theTabControl.Controls.Add(tabPermutate);
            theTabControl.Controls.Add(tabPlain);
            theTabControl.Controls.Add(tabHtml);
            theTabControl.Dock = DockStyle.Fill;
            theTabControl.Location = new Point(0, 273);
            theTabControl.Name = "theTabControl";
            theTabControl.SelectedIndex = 0;
            theTabControl.Size = new Size(1888, 719);
            theTabControl.TabIndex = 10;
            // 
            // tabPlain
            // 
            tabPlain.Controls.Add(ctrlDetailedPlain);
            tabPlain.Location = new Point(10, 58);
            tabPlain.Name = "tabPlain";
            tabPlain.Padding = new Padding(3);
            tabPlain.Size = new Size(1868, 651);
            tabPlain.TabIndex = 0;
            tabPlain.Text = "Plain text";
            tabPlain.UseVisualStyleBackColor = true;
            // 
            // ctrlDetailedPlain
            // 
            ctrlDetailedPlain.Dock = DockStyle.Fill;
            ctrlDetailedPlain.Location = new Point(3, 3);
            ctrlDetailedPlain.Multiline = true;
            ctrlDetailedPlain.Name = "ctrlDetailedPlain";
            ctrlDetailedPlain.ReadOnly = true;
            ctrlDetailedPlain.ScrollBars = ScrollBars.Both;
            ctrlDetailedPlain.Size = new Size(1862, 645);
            ctrlDetailedPlain.TabIndex = 0;
            ctrlDetailedPlain.WordWrap = false;
            // 
            // tabHtml
            // 
            tabHtml.Controls.Add(ctrlDetailedHtml);
            tabHtml.Location = new Point(10, 58);
            tabHtml.Name = "tabHtml";
            tabHtml.Padding = new Padding(3);
            tabHtml.Size = new Size(1868, 651);
            tabHtml.TabIndex = 1;
            tabHtml.Text = "HTML";
            tabHtml.UseVisualStyleBackColor = true;
            // 
            // ctrlDetailedHtml
            // 
            ctrlDetailedHtml.Dock = DockStyle.Fill;
            ctrlDetailedHtml.Location = new Point(3, 3);
            ctrlDetailedHtml.Multiline = true;
            ctrlDetailedHtml.Name = "ctrlDetailedHtml";
            ctrlDetailedHtml.ReadOnly = true;
            ctrlDetailedHtml.Size = new Size(1862, 645);
            ctrlDetailedHtml.TabIndex = 1;
            ctrlDetailedHtml.WordWrap = false;
            // 
            // tabInstantiate
            // 
            tabInstantiate.Controls.Add(txtInstance);
            tabInstantiate.Controls.Add(theGrid);
            tabInstantiate.Location = new Point(10, 58);
            tabInstantiate.Name = "tabInstantiate";
            tabInstantiate.Padding = new Padding(3);
            tabInstantiate.Size = new Size(1868, 651);
            tabInstantiate.TabIndex = 2;
            tabInstantiate.Text = "Instantiate a solution";
            tabInstantiate.UseVisualStyleBackColor = true;
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
            // theGrid
            // 
            theGrid.AllowUserToAddRows = false;
            theGrid.AllowUserToDeleteRows = false;
            theGrid.AllowUserToResizeColumns = false;
            theGrid.AllowUserToResizeRows = false;
            theGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = SystemColors.Control;
            dataGridViewCellStyle9.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle9.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            theGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            theGrid.ColumnHeadersHeight = 58;
            theGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            theGrid.Columns.AddRange(new DataGridViewColumn[] { Substance, Expression, Value, IsFreeVariable });
            theGrid.Dock = DockStyle.Fill;
            theGrid.Location = new Point(3, 3);
            theGrid.Name = "theGrid";
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = SystemColors.Control;
            dataGridViewCellStyle12.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle12.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = SystemColors.HighlightText;
            theGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            theGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            theGrid.RowTemplate.Height = 49;
            theGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            theGrid.ShowCellToolTips = false;
            theGrid.ShowEditingIcon = false;
            theGrid.Size = new Size(1862, 645);
            theGrid.TabIndex = 4;
            theGrid.CellEndEdit += OnCellEndEdit;
            // 
            // Substance
            // 
            Substance.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Substance.Frozen = true;
            Substance.HeaderText = "Substance";
            Substance.MinimumWidth = 12;
            Substance.Name = "Substance";
            Substance.ReadOnly = true;
            Substance.SortMode = DataGridViewColumnSortMode.NotSortable;
            Substance.Width = 167;
            // 
            // Expression
            // 
            Expression.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.MiddleRight;
            Expression.DefaultCellStyle = dataGridViewCellStyle10;
            Expression.HeaderText = "Expression";
            Expression.MinimumWidth = 12;
            Expression.Name = "Expression";
            Expression.ReadOnly = true;
            Expression.SortMode = DataGridViewColumnSortMode.NotSortable;
            Expression.Width = 175;
            // 
            // Value
            // 
            Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.MiddleRight;
            Value.DefaultCellStyle = dataGridViewCellStyle11;
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
            // tabPermutate
            // 
            tabPermutate.Controls.Add(listRHS);
            tabPermutate.Controls.Add(listLHS);
            tabPermutate.Location = new Point(10, 58);
            tabPermutate.Name = "tabPermutate";
            tabPermutate.Size = new Size(1868, 651);
            tabPermutate.TabIndex = 3;
            tabPermutate.Text = "Permutate";
            tabPermutate.UseVisualStyleBackColor = true;
            // 
            // listRHS
            // 
            listRHS.Dock = DockStyle.Fill;
            listRHS.ItemHeight = 41;
            listRHS.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            listRHS.Location = new Point(0, 250);
            listRHS.Name = "listRHS";
            listRHS.ScrollAlwaysVisible = true;
            listRHS.Size = new Size(1868, 401);
            listRHS.TabIndex = 3;
            listRHS.TabStop = false;
            listRHS.MouseDoubleClick += OnListMouseDoubleClick;
            // 
            // listLHS
            // 
            listLHS.Dock = DockStyle.Top;
            listLHS.ItemHeight = 41;
            listLHS.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            listLHS.Location = new Point(0, 0);
            listLHS.Name = "listLHS";
            listLHS.ScrollAlwaysVisible = true;
            listLHS.Size = new Size(1868, 250);
            listLHS.TabIndex = 2;
            listLHS.TabStop = false;
            listLHS.MouseDoubleClick += OnListMouseDoubleClick;
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
            tabPlain.ResumeLayout(false);
            tabPlain.PerformLayout();
            tabHtml.ResumeLayout(false);
            tabHtml.PerformLayout();
            tabInstantiate.ResumeLayout(false);
            tabInstantiate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) theGrid).EndInit();
            tabPermutate.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox containerInput;
        private Button buttonBalance;
        internal TextBox textBoxInput;
        private TabControl theTabControl;
        private TabPage tabPlain;
        private TabPage tabHtml;
        private TextBox ctrlDetailedPlain;
        private TextBox ctrlDetailedHtml;
        private TabPage tabInstantiate;
        private DataGridView theGrid;
        private DataGridViewTextBoxColumn Substance;
        private DataGridViewTextBoxColumn Expression;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
        private TextBox txtGeneralForm;
        private TextBox txtInstance;
        private TabPage tabPermutate;
        private ListBox listRHS;
        private ListBox listLHS;
    }
}