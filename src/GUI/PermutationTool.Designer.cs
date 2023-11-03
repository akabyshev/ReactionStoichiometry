namespace ReactionStoichiometry
{
    partial class PermutationTool
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
            listLHS = new ListBox();
            listRHS = new ListBox();
            SuspendLayout();
            // 
            // listLHS
            // 
            listLHS.Dock = DockStyle.Top;
            listLHS.ItemHeight = 41;
            listLHS.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            listLHS.Location = new Point(0, 0);
            listLHS.Name = "listLHS";
            listLHS.ScrollAlwaysVisible = true;
            listLHS.Size = new Size(568, 250);
            listLHS.TabIndex = 0;
            listLHS.TabStop = false;
            listLHS.MouseDoubleClick += OnListMouseDoubleClick;
            // 
            // listRHS
            // 
            listRHS.Dock = DockStyle.Fill;
            listRHS.ItemHeight = 41;
            listRHS.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            listRHS.Location = new Point(0, 250);
            listRHS.Name = "listRHS";
            listRHS.ScrollAlwaysVisible = true;
            listRHS.Size = new Size(568, 262);
            listRHS.TabIndex = 1;
            listRHS.TabStop = false;
            listRHS.MouseDoubleClick += OnListMouseDoubleClick;
            // 
            // PermutationTool
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(568, 512);
            ControlBox = false;
            Controls.Add(listRHS);
            Controls.Add(listLHS);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "PermutationTool";
            Text = "PermutationTool";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listLHS;
        private ListBox listRHS;
    }
}