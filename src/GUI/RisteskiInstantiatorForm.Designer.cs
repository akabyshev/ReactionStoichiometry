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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            txtInstance = new TextBox();
            Fragment = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            IsFreeVariable = new DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize) dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeight = 58;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Fragment, Value, IsFreeVariable });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 47);
            dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridView1.RowTemplate.Height = 49;
            dataGridView1.ScrollBars = ScrollBars.None;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.Size = new Size(798, 311);
            dataGridView1.TabIndex = 3;
            dataGridView1.CellEndEdit += On_dataGridView1_CellEndEdit;
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
            // Fragment
            // 
            Fragment.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Fragment.HeaderText = "Fragment";
            Fragment.MinimumWidth = 12;
            Fragment.Name = "Fragment";
            Fragment.ReadOnly = true;
            Fragment.SortMode = DataGridViewColumnSortMode.NotSortable;
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
            // RisteskiInstantiatorForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(798, 358);
            ControlBox = false;
            Controls.Add(dataGridView1);
            Controls.Add(txtInstance);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(800, 360);
            Name = "RisteskiInstantiatorForm";
            Text = "Instantiation";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize) dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtInstance;
        private DataGridViewTextBoxColumn Fragment;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn IsFreeVariable;
        private DataGridView dataGridView1;
    }
}