﻿namespace ReactionStoichiometry.GUI
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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
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
            tabDetailedMultiline = new TabPage();
            txtDetailedMultiline = new TextBox();
            containerInput.SuspendLayout();
            theTabControl.SuspendLayout();
            tabInstantiate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) gridCoefficients).BeginInit();
            tabPermutate.SuspendLayout();
            tabDetailedMultiline.SuspendLayout();
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
            theTabControl.Controls.Add(tabDetailedMultiline);
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
            gridCoefficients.Columns.AddRange(new DataGridViewColumn[] { Substance, Expression, Value, IsFreeVariable });
            gridCoefficients.Dock = DockStyle.Fill;
            gridCoefficients.Location = new Point(3, 3);
            gridCoefficients.Name = "gridCoefficients";
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            gridCoefficients.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
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
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            Expression.DefaultCellStyle = dataGridViewCellStyle2;
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
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            Value.DefaultCellStyle = dataGridViewCellStyle3;
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
            // tabDetailedMultiline
            // 
            tabDetailedMultiline.Controls.Add(txtDetailedMultiline);
            tabDetailedMultiline.Location = new Point(10, 58);
            tabDetailedMultiline.Name = "tabDetailedMultiline";
            tabDetailedMultiline.Padding = new Padding(3);
            tabDetailedMultiline.Size = new Size(1868, 651);
            tabDetailedMultiline.TabIndex = 0;
            tabDetailedMultiline.Text = "Text";
            tabDetailedMultiline.UseVisualStyleBackColor = true;
            // 
            // txtDetailedMultiline
            // 
            txtDetailedMultiline.Dock = DockStyle.Fill;
            txtDetailedMultiline.Location = new Point(3, 3);
            txtDetailedMultiline.Multiline = true;
            txtDetailedMultiline.Name = "txtDetailedMultiline";
            txtDetailedMultiline.ReadOnly = true;
            txtDetailedMultiline.ScrollBars = ScrollBars.Both;
            txtDetailedMultiline.Size = new Size(1862, 645);
            txtDetailedMultiline.TabIndex = 0;
            txtDetailedMultiline.WordWrap = false;
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
            tabDetailedMultiline.ResumeLayout(false);
            tabDetailedMultiline.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox containerInput;
        private Button buttonBalance;
        private TextBox textBoxInput;
        private TabControl theTabControl;
        private TabPage tabDetailedMultiline;
        private TabPage tabInstantiate;
        private TabPage tabPermutate;
        private TextBox txtDetailedMultiline;
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