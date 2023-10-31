namespace ReactionStoichiometry.GUI
{
    partial class RisteskiInstantiatorForm
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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            theGrid = new DataGridView();
            Entity = new DataGridViewTextBoxColumn();
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
            theGrid.AllowUserToResizeRows = false;
            theGrid.ColumnHeadersHeight = 58;
            theGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            theGrid.Columns.AddRange(new DataGridViewColumn[] { Entity, Value, IsFreeVariable });
            theGrid.Dock = DockStyle.Fill;
            theGrid.Location = new Point(0, 47);
            theGrid.Name = "theGrid";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            theGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
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
            Entity.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Entity.HeaderText = "Entity";
            Entity.MinimumWidth = 12;
            Entity.Name = "Entity";
            Entity.ReadOnly = true;
            Entity.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Value
            // 
            Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Value.HeaderText = "Value";
            Value.MinimumWidth = 12;
            Value.Name = "Value";
            Value.ReadOnly = true;
            Value.Resizable = DataGridViewTriState.False;
            Value.SortMode = DataGridViewColumnSortMode.NotSortable;
            Value.Width = 96;
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
            txtInstance.TextChanged += On_txtInstance_TextChanged;
            // 
            // RisteskiInstantiatorForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(798, 358);
            ControlBox = false;
            Controls.Add(theGrid);
            Controls.Add(txtInstance);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(800, 360);
            Name = "RisteskiInstantiatorForm";
            Text = "Instantiation";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize) theGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtInstance;
        private DataGridViewTextBoxColumn Entity;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
        private DataGridView theGrid;
    }
}