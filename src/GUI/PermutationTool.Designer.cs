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
            listEntities = new ListView();
            panel1 = new Panel();
            buttonMoveDown = new Button();
            buttonMoveUp = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // listEntities
            // 
            listEntities.Dock = DockStyle.Fill;
            listEntities.FullRowSelect = true;
            listEntities.GridLines = true;
            listEntities.Location = new Point(64, 0);
            listEntities.MultiSelect = false;
            listEntities.Name = "listEntities";
            listEntities.Size = new Size(504, 512);
            listEntities.TabIndex = 0;
            listEntities.UseCompatibleStateImageBehavior = false;
            listEntities.View = View.SmallIcon;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonMoveDown);
            panel1.Controls.Add(buttonMoveUp);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(64, 512);
            panel1.TabIndex = 3;
            // 
            // buttonMoveDown
            // 
            buttonMoveDown.Dock = DockStyle.Bottom;
            buttonMoveDown.Location = new Point(0, 384);
            buttonMoveDown.Name = "buttonMoveDown";
            buttonMoveDown.Size = new Size(64, 128);
            buttonMoveDown.TabIndex = 5;
            buttonMoveDown.Text = "▼";
            buttonMoveDown.UseVisualStyleBackColor = true;
            // 
            // buttonMoveUp
            // 
            buttonMoveUp.Dock = DockStyle.Top;
            buttonMoveUp.Location = new Point(0, 0);
            buttonMoveUp.Name = "buttonMoveUp";
            buttonMoveUp.Size = new Size(64, 128);
            buttonMoveUp.TabIndex = 4;
            buttonMoveUp.Text = "▲";
            buttonMoveUp.UseVisualStyleBackColor = true;
            // 
            // PermutationTool
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(568, 512);
            ControlBox = false;
            Controls.Add(listEntities);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "PermutationTool";
            Text = "PermutationTool";
            TopMost = true;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView listEntities;
        private Panel panel1;
        private Button buttonMoveDown;
        private Button buttonMoveUp;
    }
}