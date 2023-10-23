namespace ReactionStoichiometry.GUI
{
    partial class MainForm
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
            containerOutput = new SplitContainer();
            resultMT = new TextBox();
            resultMR = new TextBox();
            dataGridView1 = new DataGridView();
            Expression = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            containerInput = new Panel();
            textBoxInput = new TextBox();
            buttonBalance = new Button();
            ((System.ComponentModel.ISupportInitialize) containerOutput).BeginInit();
            containerOutput.Panel1.SuspendLayout();
            containerOutput.Panel2.SuspendLayout();
            containerOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) dataGridView1).BeginInit();
            containerInput.SuspendLayout();
            SuspendLayout();
            // 
            // containerOutput
            // 
            containerOutput.Dock = DockStyle.Fill;
            containerOutput.Location = new Point(0, 47);
            containerOutput.Name = "containerOutput";
            // 
            // containerOutput.Panel1
            // 
            containerOutput.Panel1.Controls.Add(resultMT);
            // 
            // containerOutput.Panel2
            // 
            containerOutput.Panel2.Controls.Add(resultMR);
            containerOutput.Panel2.Controls.Add(dataGridView1);
            containerOutput.Size = new Size(1888, 945);
            containerOutput.SplitterDistance = 969;
            containerOutput.SplitterWidth = 8;
            containerOutput.TabIndex = 6;
            // 
            // resultMT
            // 
            resultMT.Dock = DockStyle.Fill;
            resultMT.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point);
            resultMT.Location = new Point(0, 0);
            resultMT.Multiline = true;
            resultMT.Name = "resultMT";
            resultMT.ReadOnly = true;
            resultMT.ScrollBars = ScrollBars.Both;
            resultMT.Size = new Size(969, 945);
            resultMT.TabIndex = 6;
            resultMT.WordWrap = false;
            // 
            // resultMR
            // 
            resultMR.Dock = DockStyle.Fill;
            resultMR.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point);
            resultMR.Location = new Point(0, 0);
            resultMR.Multiline = true;
            resultMR.Name = "resultMR";
            resultMR.ReadOnly = true;
            resultMR.ScrollBars = ScrollBars.Both;
            resultMR.Size = new Size(911, 474);
            resultMR.TabIndex = 1;
            resultMR.WordWrap = false;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Expression, Value });
            dataGridView1.Dock = DockStyle.Bottom;
            dataGridView1.Location = new Point(0, 474);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 102;
            dataGridView1.RowTemplate.Height = 49;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.Size = new Size(911, 471);
            dataGridView1.TabIndex = 2;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            // 
            // Expression
            // 
            Expression.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Expression.HeaderText = "Expression";
            Expression.MinimumWidth = 12;
            Expression.Name = "Expression";
            Expression.ReadOnly = true;
            Expression.Resizable = DataGridViewTriState.False;
            Expression.SortMode = DataGridViewColumnSortMode.NotSortable;
            Expression.Width = 165;
            // 
            // Value
            // 
            Value.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Value.HeaderText = "Value";
            Value.MinimumWidth = 12;
            Value.Name = "Value";
            Value.ReadOnly = true;
            Value.Resizable = DataGridViewTriState.False;
            // 
            // containerInput
            // 
            containerInput.Controls.Add(textBoxInput);
            containerInput.Controls.Add(buttonBalance);
            containerInput.Dock = DockStyle.Top;
            containerInput.Location = new Point(0, 0);
            containerInput.Name = "containerInput";
            containerInput.Size = new Size(1888, 47);
            containerInput.TabIndex = 7;
            // 
            // textBoxInput
            // 
            textBoxInput.Dock = DockStyle.Fill;
            textBoxInput.Location = new Point(0, 0);
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(1700, 47);
            textBoxInput.TabIndex = 4;
            textBoxInput.Text = "Fe2(SO4)3 + PrTlTe3 + H3PO4 = Fe0.996(H2PO4)2H2O + Tl1.987(SO3)3 + Pr1.998(SO4)3 + Te2O3 + P2O5 + H2S";
            textBoxInput.TextChanged += textBoxInput_TextChanged;
            // 
            // buttonBalance
            // 
            buttonBalance.Dock = DockStyle.Right;
            buttonBalance.Enabled = false;
            buttonBalance.Location = new Point(1700, 0);
            buttonBalance.Name = "buttonBalance";
            buttonBalance.Size = new Size(188, 47);
            buttonBalance.TabIndex = 5;
            buttonBalance.Text = "Balance";
            buttonBalance.UseVisualStyleBackColor = true;
            buttonBalance.Click += buttonBalance_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1888, 992);
            Controls.Add(containerOutput);
            Controls.Add(containerInput);
            DoubleBuffered = true;
            MaximizeBox = false;
            MinimumSize = new Size(1600, 800);
            Name = "MainForm";
            Text = "Balancing algo";
            WindowState = FormWindowState.Maximized;
            containerOutput.Panel1.ResumeLayout(false);
            containerOutput.Panel1.PerformLayout();
            containerOutput.Panel2.ResumeLayout(false);
            containerOutput.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) containerOutput).EndInit();
            containerOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) dataGridView1).EndInit();
            containerInput.ResumeLayout(false);
            containerInput.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer containerOutput;
        private Panel containerInput;
        private Button buttonBalance;
        private TextBox textBoxInput;
        private TextBox resultMT;
        private TextBox resultMR;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Expression;
        private DataGridViewTextBoxColumn Value;
    }
}