namespace SCS.Surveillance
{
    partial class LogoutForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.desLabel = new System.Windows.Forms.Label();
            this.passTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.deleteLoginInfo = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // desLabel
            // 
            this.desLabel.AutoSize = true;
            this.desLabel.Location = new System.Drawing.Point(12, 19);
            this.desLabel.Name = "desLabel";
            this.desLabel.Size = new System.Drawing.Size(145, 12);
            this.desLabel.TabIndex = 0;
            this.desLabel.Text = "비밀번호를 입력해주세요.";
            // 
            // passTextBox
            // 
            this.passTextBox.Location = new System.Drawing.Point(12, 45);
            this.passTextBox.Name = "passTextBox";
            this.passTextBox.Size = new System.Drawing.Size(274, 21);
            this.passTextBox.TabIndex = 1;
            this.passTextBox.UseSystemPasswordChar = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(112, 98);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "확인";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // deleteLoginInfo
            // 
            this.deleteLoginInfo.AutoSize = true;
            this.deleteLoginInfo.Location = new System.Drawing.Point(14, 76);
            this.deleteLoginInfo.Name = "deleteLoginInfo";
            this.deleteLoginInfo.Size = new System.Drawing.Size(260, 16);
            this.deleteLoginInfo.TabIndex = 3;
            this.deleteLoginInfo.Text = "컴퓨터에 저장된 로그인 정보를 삭제합니다.";
            this.deleteLoginInfo.UseVisualStyleBackColor = true;
            // 
            // LogoutForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(299, 130);
            this.Controls.Add(this.deleteLoginInfo);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.passTextBox);
            this.Controls.Add(this.desLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LogoutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "보호 종료";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label desLabel;
        private System.Windows.Forms.TextBox passTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox deleteLoginInfo;
    }
}

