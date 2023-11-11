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
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            containerInput = new GroupBox();
            textBoxInput = new TextBox();
            buttonBalance = new Button();
            txtGeneralForm = new TextBox();
            theTabControl = new TabControl();
            tabInstantiate = new TabPage();
            txtInstance = new TextBox();
            gridCoefficients = new DataGridView();
            Substance = new DataGridViewTextBoxColumn();
            Expression = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            IsFreeVariable = new DataGridViewCheckBoxColumn();
            tabPermutate = new TabPage();
            listPermutator = new ListBox();
            tabPlain = new TabPage();
            txtDetailedPlain = new TextBox();
            tabHtml = new TabPage();
            txtDetailedHtml = new TextBox();
            containerInput.SuspendLayout();
            theTabControl.SuspendLayout();
            tabInstantiate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) gridCoefficients).BeginInit();
            tabPermutate.SuspendLayout();
            tabPlain.SuspendLayout();
            tabHtml.SuspendLayout();
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
            textBoxInput.Text = "C6H5COOH + O2 = CO2 + H2O";
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
            // tabInstantiate
            // 
            tabInstantiate.Controls.Add(txtInstance);
            tabInstantiate.Controls.Add(gridCoefficients);
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
            // gridCoefficients
            // 
            gridCoefficients.AllowUserToAddRows = false;
            gridCoefficients.AllowUserToDeleteRows = false;
            gridCoefficients.AllowUserToResizeColumns = false;
            gridCoefficients.AllowUserToResizeRows = false;
            gridCoefficients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = SystemColors.Control;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            gridCoefficients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            gridCoefficients.ColumnHeadersHeight = 58;
            gridCoefficients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridCoefficients.Columns.AddRange(new DataGridViewColumn[] { Substance, Expression, Value, IsFreeVariable });
            gridCoefficients.Dock = DockStyle.Fill;
            gridCoefficients.Location = new Point(3, 3);
            gridCoefficients.Name = "gridCoefficients";
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = SystemColors.Control;
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle8.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            gridCoefficients.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            gridCoefficients.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            gridCoefficients.RowTemplate.Height = 49;
            gridCoefficients.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridCoefficients.ShowCellToolTips = false;
            gridCoefficients.ShowEditingIcon = false;
            gridCoefficients.Size = new Size(1862, 645);
            gridCoefficients.TabIndex = 4;
            gridCoefficients.CellEndEdit += OnCellEndEdit;
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
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleRight;
            Expression.DefaultCellStyle = dataGridViewCellStyle6;
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
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleRight;
            Value.DefaultCellStyle = dataGridViewCellStyle7;
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
            // tabPlain
            // 
            tabPlain.Controls.Add(txtDetailedPlain);
            tabPlain.Location = new Point(10, 58);
            tabPlain.Name = "tabPlain";
            tabPlain.Padding = new Padding(3);
            tabPlain.Size = new Size(1868, 651);
            tabPlain.TabIndex = 0;
            tabPlain.Text = "Plain text";
            tabPlain.UseVisualStyleBackColor = true;
            // 
            // txtDetailedPlain
            // 
            txtDetailedPlain.Dock = DockStyle.Fill;
            txtDetailedPlain.Location = new Point(3, 3);
            txtDetailedPlain.Multiline = true;
            txtDetailedPlain.Name = "txtDetailedPlain";
            txtDetailedPlain.ReadOnly = true;
            txtDetailedPlain.ScrollBars = ScrollBars.Both;
            txtDetailedPlain.Size = new Size(1862, 645);
            txtDetailedPlain.TabIndex = 0;
            txtDetailedPlain.WordWrap = false;
            // 
            // tabHtml
            // 
            tabHtml.Controls.Add(txtDetailedHtml);
            tabHtml.Location = new Point(10, 58);
            tabHtml.Name = "tabHtml";
            tabHtml.Padding = new Padding(3);
            tabHtml.Size = new Size(1868, 651);
            tabHtml.TabIndex = 1;
            tabHtml.Text = "HTML";
            tabHtml.UseVisualStyleBackColor = true;
            // 
            // txtDetailedHtml
            // 
            txtDetailedHtml.Dock = DockStyle.Fill;
            txtDetailedHtml.Location = new Point(3, 3);
            txtDetailedHtml.Multiline = true;
            txtDetailedHtml.Name = "txtDetailedHtml";
            txtDetailedHtml.ReadOnly = true;
            txtDetailedHtml.Size = new Size(1862, 645);
            txtDetailedHtml.TabIndex = 1;
            txtDetailedHtml.WordWrap = false;
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
            tabInstantiate.ResumeLayout(false);
            tabInstantiate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) gridCoefficients).EndInit();
            tabPermutate.ResumeLayout(false);
            tabPlain.ResumeLayout(false);
            tabPlain.PerformLayout();
            tabHtml.ResumeLayout(false);
            tabHtml.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox containerInput;
        private Button buttonBalance;
        private TextBox textBoxInput;
        private TabControl theTabControl;
        private TabPage tabPlain;
        private TabPage tabHtml;
        private TabPage tabInstantiate;
        private TabPage tabPermutate;
        private TextBox txtDetailedPlain;
        private TextBox txtDetailedHtml;
        private DataGridView gridCoefficients;
        private DataGridViewTextBoxColumn Substance;
        private DataGridViewTextBoxColumn Expression;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
        private TextBox txtGeneralForm;
        private TextBox txtInstance;
        private ListBox listPermutator;
    }
}