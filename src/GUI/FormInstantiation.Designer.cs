namespace ReactionStoichiometry
{
    partial class FormInstantiation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            theGrid = new DataGridView();
            txtInstance = new TextBox();
            txtGeneralForm = new TextBox();
            Substance = new DataGridViewTextBoxColumn();
            Expression = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            IsFreeVariable = new DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize) theGrid).BeginInit();
            SuspendLayout();
            // 
            // theGrid
            // 
            theGrid.AllowUserToAddRows = false;
            theGrid.AllowUserToDeleteRows = false;
            theGrid.AllowUserToResizeColumns = false;
            theGrid.AllowUserToResizeRows = false;
            theGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            theGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            theGrid.ColumnHeadersHeight = 58;
            theGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            theGrid.Columns.AddRange(new DataGridViewColumn[] { Substance, Expression, Value, IsFreeVariable });
            theGrid.Dock = DockStyle.Fill;
            theGrid.Location = new Point(0, 47);
            theGrid.Name = "theGrid";
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            theGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            theGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            theGrid.RowTemplate.Height = 49;
            theGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            theGrid.ShowCellToolTips = false;
            theGrid.ShowEditingIcon = false;
            theGrid.Size = new Size(808, 314);
            theGrid.TabIndex = 3;
            theGrid.CellEndEdit += OnCellEndEdit;
            // 
            // txtInstance
            // 
            txtInstance.Dock = DockStyle.Bottom;
            txtInstance.Location = new Point(0, 361);
            txtInstance.Name = "txtInstance";
            txtInstance.ReadOnly = true;
            txtInstance.Size = new Size(808, 47);
            txtInstance.TabIndex = 4;
            // 
            // txtGeneralForm
            // 
            txtGeneralForm.Dock = DockStyle.Top;
            txtGeneralForm.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            txtGeneralForm.Location = new Point(0, 0);
            txtGeneralForm.Name = "txtGeneralForm";
            txtGeneralForm.ReadOnly = true;
            txtGeneralForm.Size = new Size(808, 47);
            txtGeneralForm.TabIndex = 5;
            txtGeneralForm.TextAlign = HorizontalAlignment.Center;
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
            // FormInstantiation
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(808, 408);
            ControlBox = false;
            Controls.Add(theGrid);
            Controls.Add(txtGeneralForm);
            Controls.Add(txtInstance);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimumSize = new Size(840, 440);
            Name = "FormInstantiation";
            Text = "Instantiation";
            ((System.ComponentModel.ISupportInitialize) theGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtInstance;
        private DataGridView theGrid;
        private TextBox txtGeneralForm;
        private DataGridViewTextBoxColumn Substance;
        private DataGridViewTextBoxColumn Expression;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
    }
}