namespace SCS.Surveillance
{
    partial class LoginForm
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
            this.L1 = new System.Windows.Forms.Label();
            this.serverInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.serverPortText = new System.Windows.Forms.TextBox();
            this.L3 = new System.Windows.Forms.Label();
            this.serverIPText = new System.Windows.Forms.TextBox();
            this.L2 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.serverInfoGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // L1
            // 
            this.L1.AutoSize = true;
            this.L1.Location = new System.Drawing.Point(12, 9);
            this.L1.Name = "L1";
            this.L1.Size = new System.Drawing.Size(253, 48);
            this.L1.TabIndex = 0;
            this.L1.Text = "서버에 로그인하기 위한 정보를 입력해주세요.\r\n\r\n입력한 모든 정보는 자동으로 저장되어\r\n다음번 프로그램 실행시 자동으로 사용됩니다.";
            // 
            // serverInfoGroupBox
            // 
            this.serverInfoGroupBox.Controls.Add(this.serverPortText);
            this.serverInfoGroupBox.Controls.Add(this.L3);
            this.serverInfoGroupBox.Controls.Add(this.serverIPText);
            this.serverInfoGroupBox.Controls.Add(this.L2);
            this.serverInfoGroupBox.Location = new System.Drawing.Point(12, 72);
            this.serverInfoGroupBox.Name = "serverInfoGroupBox";
            this.serverInfoGroupBox.Size = new System.Drawing.Size(248, 97);
            this.serverInfoGroupBox.TabIndex = 1;
            this.serverInfoGroupBox.TabStop = false;
            this.serverInfoGroupBox.Text = "서버 정보";
            // 
            // serverPortText
            // 
            this.serverPortText.Location = new System.Drawing.Point(111, 59);
            this.serverPortText.Name = "serverPortText";
            this.serverPortText.Size = new System.Drawing.Size(100, 21);
            this.serverPortText.TabIndex = 3;
            // 
            // L3
            // 
            this.L3.AutoSize = true;
            this.L3.Location = new System.Drawing.Point(16, 62);
            this.L3.Name = "L3";
            this.L3.Size = new System.Drawing.Size(89, 12);
            this.L3.TabIndex = 2;
            this.L3.Text = "서버 포트 번호:";
            // 
            // serverIPText
            // 
            this.serverIPText.Location = new System.Drawing.Point(111, 23);
            this.serverIPText.Name = "serverIPText";
            this.serverIPText.Size = new System.Drawing.Size(100, 21);
            this.serverIPText.TabIndex = 1;
            // 
            // L2
            // 
            this.L2.AutoSize = true;
            this.L2.Location = new System.Drawing.Point(33, 26);
            this.L2.Name = "L2";
            this.L2.Size = new System.Drawing.Size(72, 12);
            this.L2.TabIndex = 0;
            this.L2.Text = "서버 IP주소:";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(86, 175);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(112, 32);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "확인";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(274, 218);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.serverInfoGroupBox);
            this.Controls.Add(this.L1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Text = "서버에 로그인";
            this.serverInfoGroupBox.ResumeLayout(false);
            this.serverInfoGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label L1;
        private System.Windows.Forms.GroupBox serverInfoGroupBox;
        private System.Windows.Forms.TextBox serverPortText;
        private System.Windows.Forms.Label L3;
        private System.Windows.Forms.TextBox serverIPText;
        private System.Windows.Forms.Label L2;
        private System.Windows.Forms.Button okButton;
    }
}