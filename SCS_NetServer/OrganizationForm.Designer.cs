namespace SCS.Net.Server
{
    partial class OrganizationForm
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
            this.orgView = new System.Windows.Forms.TreeView();
            this.refButton = new System.Windows.Forms.Button();
            this.IpTextBox = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.UserAddButton = new System.Windows.Forms.Button();
            this.userInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ipLabel = new System.Windows.Forms.Label();
            this.modifyButton = new System.Windows.Forms.Button();
            this.depAddButton = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userInfoGroupBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // orgView
            // 
            this.orgView.HideSelection = false;
            this.orgView.Location = new System.Drawing.Point(12, 33);
            this.orgView.Name = "orgView";
            this.orgView.ShowRootLines = false;
            this.orgView.Size = new System.Drawing.Size(235, 488);
            this.orgView.TabIndex = 0;
            this.orgView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.orgView_AfterSelect);
            // 
            // refButton
            // 
            this.refButton.Location = new System.Drawing.Point(463, 33);
            this.refButton.Name = "refButton";
            this.refButton.Size = new System.Drawing.Size(75, 23);
            this.refButton.TabIndex = 1;
            this.refButton.Text = "새로고침";
            this.refButton.UseVisualStyleBackColor = true;
            this.refButton.Click += new System.EventHandler(this.refButton_Click);
            // 
            // IpTextBox
            // 
            this.IpTextBox.Location = new System.Drawing.Point(45, 44);
            this.IpTextBox.Name = "IpTextBox";
            this.IpTextBox.Size = new System.Drawing.Size(179, 21);
            this.IpTextBox.TabIndex = 2;
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(45, 85);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(179, 21);
            this.NameTextBox.TabIndex = 3;
            // 
            // UserAddButton
            // 
            this.UserAddButton.Location = new System.Drawing.Point(261, 33);
            this.UserAddButton.Name = "UserAddButton";
            this.UserAddButton.Size = new System.Drawing.Size(88, 23);
            this.UserAddButton.TabIndex = 4;
            this.UserAddButton.Text = "이용자 추가";
            this.UserAddButton.UseVisualStyleBackColor = true;
            this.UserAddButton.Click += new System.EventHandler(this.UserAddButton_Click);
            // 
            // userInfoGroupBox
            // 
            this.userInfoGroupBox.Controls.Add(this.label1);
            this.userInfoGroupBox.Controls.Add(this.ipLabel);
            this.userInfoGroupBox.Controls.Add(this.modifyButton);
            this.userInfoGroupBox.Controls.Add(this.IpTextBox);
            this.userInfoGroupBox.Controls.Add(this.NameTextBox);
            this.userInfoGroupBox.Location = new System.Drawing.Point(253, 164);
            this.userInfoGroupBox.Name = "userInfoGroupBox";
            this.userInfoGroupBox.Size = new System.Drawing.Size(285, 149);
            this.userInfoGroupBox.TabIndex = 6;
            this.userInfoGroupBox.TabStop = false;
            this.userInfoGroupBox.Text = "이용자 정보";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "이름:";
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(6, 47);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(20, 12);
            this.ipLabel.TabIndex = 8;
            this.ipLabel.Text = "IP:";
            // 
            // modifyButton
            // 
            this.modifyButton.Location = new System.Drawing.Point(104, 112);
            this.modifyButton.Name = "modifyButton";
            this.modifyButton.Size = new System.Drawing.Size(75, 23);
            this.modifyButton.TabIndex = 7;
            this.modifyButton.Text = "수정";
            this.modifyButton.UseVisualStyleBackColor = true;
            this.modifyButton.Click += new System.EventHandler(this.modifyButton_Click);
            // 
            // depAddButton
            // 
            this.depAddButton.Location = new System.Drawing.Point(12, 527);
            this.depAddButton.Name = "depAddButton";
            this.depAddButton.Size = new System.Drawing.Size(88, 23);
            this.depAddButton.TabIndex = 10;
            this.depAddButton.Text = "부서 추가";
            this.depAddButton.UseVisualStyleBackColor = true;
            this.depAddButton.Click += new System.EventHandler(this.depAddButton_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(550, 24);
            this.menuStrip.TabIndex = 11;
            this.menuStrip.Text = "menuStrip1";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.logToolStripMenuItem.Text = "서버 로그";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // OrganizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(550, 554);
            this.Controls.Add(this.depAddButton);
            this.Controls.Add(this.UserAddButton);
            this.Controls.Add(this.userInfoGroupBox);
            this.Controls.Add(this.refButton);
            this.Controls.Add(this.orgView);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "OrganizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "조직도";
            this.Load += new System.EventHandler(this.OrganizationForm_Load);
            this.userInfoGroupBox.ResumeLayout(false);
            this.userInfoGroupBox.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView orgView;
        private System.Windows.Forms.Button refButton;
        private System.Windows.Forms.TextBox IpTextBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Button UserAddButton;
        private System.Windows.Forms.GroupBox userInfoGroupBox;
        private System.Windows.Forms.Button modifyButton;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button depAddButton;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
    }
}