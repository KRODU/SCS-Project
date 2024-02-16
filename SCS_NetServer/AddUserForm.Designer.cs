namespace SCS.Net.Server
{
    partial class AddUserForm
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
            this.userInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.depComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IpTextBox = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.userInfoGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // userInfoGroupBox
            // 
            this.userInfoGroupBox.Controls.Add(this.depComboBox);
            this.userInfoGroupBox.Controls.Add(this.label3);
            this.userInfoGroupBox.Controls.Add(this.label1);
            this.userInfoGroupBox.Controls.Add(this.label2);
            this.userInfoGroupBox.Controls.Add(this.IpTextBox);
            this.userInfoGroupBox.Controls.Add(this.NameTextBox);
            this.userInfoGroupBox.Location = new System.Drawing.Point(12, 12);
            this.userInfoGroupBox.Name = "userInfoGroupBox";
            this.userInfoGroupBox.Size = new System.Drawing.Size(241, 125);
            this.userInfoGroupBox.TabIndex = 0;
            this.userInfoGroupBox.TabStop = false;
            this.userInfoGroupBox.Text = "이용자 정보 입력";
            // 
            // depComboBox
            // 
            this.depComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.depComboBox.Location = new System.Drawing.Point(45, 27);
            this.depComboBox.Name = "depComboBox";
            this.depComboBox.Size = new System.Drawing.Size(179, 20);
            this.depComboBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "부서:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "이름:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP:";
            // 
            // IpTextBox
            // 
            this.IpTextBox.Location = new System.Drawing.Point(45, 55);
            this.IpTextBox.Name = "IpTextBox";
            this.IpTextBox.Size = new System.Drawing.Size(179, 21);
            this.IpTextBox.TabIndex = 3;
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(45, 87);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(179, 21);
            this.NameTextBox.TabIndex = 5;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(93, 143);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "확인";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // AddUserForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(262, 172);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.userInfoGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AddUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "사용자 추가";
            this.Load += new System.EventHandler(this.AddUser_Load);
            this.userInfoGroupBox.ResumeLayout(false);
            this.userInfoGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox userInfoGroupBox;
        private System.Windows.Forms.ComboBox depComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IpTextBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Button okButton;
    }
}