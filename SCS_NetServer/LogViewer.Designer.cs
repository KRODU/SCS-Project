namespace SCS.Net.Server
{
    partial class LogViewer
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
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.userListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.BackColor = System.Drawing.Color.White;
            this.logTextBox.Location = new System.Drawing.Point(95, 0);
            this.logTextBox.MaxLength = 999999999;
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(883, 653);
            this.logTextBox.TabIndex = 0;
            // 
            // userListBox
            // 
            this.userListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.userListBox.FormattingEnabled = true;
            this.userListBox.ItemHeight = 12;
            this.userListBox.Location = new System.Drawing.Point(0, 0);
            this.userListBox.Name = "userListBox";
            this.userListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.userListBox.Size = new System.Drawing.Size(95, 653);
            this.userListBox.Sorted = true;
            this.userListBox.TabIndex = 1;
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 653);
            this.Controls.Add(this.userListBox);
            this.Controls.Add(this.logTextBox);
            this.Name = "LogViewer";
            this.ShowIcon = false;
            this.Text = "서버 로그";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogViewer_FormClosed);
            this.Load += new System.EventHandler(this.LogViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.ListBox userListBox;
    }
}