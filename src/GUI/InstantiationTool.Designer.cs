namespace ReactionStoichiometry
{
    partial class InstantiationTool
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
            Entity = new DataGridViewTextBoxColumn();
            Expression = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            IsFreeVariable = new DataGridViewCheckBoxColumn();
            txtInstance = new TextBox();
            ((System.ComponentModel.ISupportInitialize) theGrid).BeginInit();
            SuspendLayout();
            // 
            // theGrid
            // 
            theGrid.AllowUserToAddRows = false;
            theGrid.AllowUserToDeleteRows = false;
            theGrid.AllowUserToResizeColumns = false;
            theGrid.AllowUserToResizeRows = false;
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
            theGrid.Columns.AddRange(new DataGridViewColumn[] { Entity, Expression, Value, IsFreeVariable });
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
            theGrid.ScrollBars = ScrollBars.None;
            theGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            theGrid.ShowCellToolTips = false;
            theGrid.Size = new Size(798, 311);
            theGrid.TabIndex = 3;
            theGrid.CellEndEdit += OnCellEndEdit;
            // 
            // Entity
            // 
            Entity.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Entity.HeaderText = "Entity";
            Entity.MinimumWidth = 12;
            Entity.Name = "Entity";
            Entity.ReadOnly = true;
            Entity.SortMode = DataGridViewColumnSortMode.NotSortable;
            Entity.Width = 107;
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
            IsFreeVariable.Width = 207;
            // 
            // txtInstance
            // 
            txtInstance.Dock = DockStyle.Top;
            txtInstance.Location = new Point(0, 0);
            txtInstance.Name = "txtInstance";
            txtInstance.ReadOnly = true;
            txtInstance.Size = new Size(798, 47);
            txtInstance.TabIndex = 4;
            txtInstance.TextChanged += OnTextChanged;
            // 
            // InstantiationTool
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(798, 358);
            ControlBox = false;
            Controls.Add(theGrid);
            Controls.Add(txtInstance);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MinimumSize = new Size(800, 360);
            Name = "InstantiationTool";
            Text = "Instantiation";
            ((System.ComponentModel.ISupportInitialize) theGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtInstance;
        private DataGridView theGrid;
        private DataGridViewTextBoxColumn Entity;
        private DataGridViewTextBoxColumn Expression;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
    }
}