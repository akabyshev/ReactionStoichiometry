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
            statusStrip1 = new StatusStrip();
            resultMR = new TextBox();
            containerInput = new Panel();
            textBoxInput = new TextBox();
            buttonBalance = new Button();
            ((System.ComponentModel.ISupportInitialize) containerOutput).BeginInit();
            containerOutput.Panel1.SuspendLayout();
            containerOutput.Panel2.SuspendLayout();
            containerOutput.SuspendLayout();
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
            containerOutput.Panel2.Controls.Add(statusStrip1);
            containerOutput.Panel2.Controls.Add(resultMR);
            containerOutput.Size = new Size(2368, 1465);
            containerOutput.SplitterDistance = 1215;
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
            resultMT.Size = new Size(1215, 1465);
            resultMT.TabIndex = 6;
            resultMT.WordWrap = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(40, 40);
            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip1.Location = new Point(0, 1443);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1145, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
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
            resultMR.Size = new Size(1145, 1465);
            resultMR.TabIndex = 1;
            resultMR.WordWrap = false;
            // 
            // containerInput
            // 
            containerInput.Controls.Add(textBoxInput);
            containerInput.Controls.Add(buttonBalance);
            containerInput.Dock = DockStyle.Top;
            containerInput.Location = new Point(0, 0);
            containerInput.Name = "containerInput";
            containerInput.Size = new Size(2368, 47);
            containerInput.TabIndex = 7;
            // 
            // textBoxInput
            // 
            textBoxInput.Dock = DockStyle.Fill;
            textBoxInput.Location = new Point(0, 0);
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(2180, 47);
            textBoxInput.TabIndex = 4;
            textBoxInput.Text = "IO4Qn+IQn=IO3Qn+I3Qn+H2O+OHQn";
            textBoxInput.TextChanged += On_textBoxInput_TextChanged;
            // 
            // buttonBalance
            // 
            buttonBalance.Dock = DockStyle.Right;
            buttonBalance.Enabled = false;
            buttonBalance.Location = new Point(2180, 0);
            buttonBalance.Name = "buttonBalance";
            buttonBalance.Size = new Size(188, 47);
            buttonBalance.TabIndex = 5;
            buttonBalance.Text = "Balance";
            buttonBalance.UseVisualStyleBackColor = true;
            buttonBalance.Click += On_buttonBalance_Click;
            // 
            // MainForm
            // 
            AcceptButton = buttonBalance;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(2368, 1512);
            Controls.Add(containerOutput);
            Controls.Add(containerInput);
            DoubleBuffered = true;
            MaximizeBox = false;
            MinimumSize = new Size(2400, 1600);
            Name = "MainForm";
            Text = "Balancing algo";
            containerOutput.Panel1.ResumeLayout(false);
            containerOutput.Panel1.PerformLayout();
            containerOutput.Panel2.ResumeLayout(false);
            containerOutput.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) containerOutput).EndInit();
            containerOutput.ResumeLayout(false);
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
        private StatusStrip statusStrip1;
    }
}