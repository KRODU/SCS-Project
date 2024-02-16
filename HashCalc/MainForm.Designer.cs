namespace HashCalc
{
    partial class MainForm
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
            this.filePathText = new System.Windows.Forms.TextBox();
            this.l1 = new System.Windows.Forms.Label();
            this.l2 = new System.Windows.Forms.Label();
            this.fileBrowser = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.calcButton = new System.Windows.Forms.Button();
            this.hashResultText = new System.Windows.Forms.TextBox();
            this.clipCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // filePathText
            // 
            this.filePathText.Location = new System.Drawing.Point(79, 35);
            this.filePathText.Name = "filePathText";
            this.filePathText.Size = new System.Drawing.Size(357, 21);
            this.filePathText.TabIndex = 0;
            this.filePathText.TextChanged += new System.EventHandler(this.filePathText_TextChanged);
            // 
            // l1
            // 
            this.l1.AutoSize = true;
            this.l1.Location = new System.Drawing.Point(12, 9);
            this.l1.Name = "l1";
            this.l1.Size = new System.Drawing.Size(167, 12);
            this.l1.TabIndex = 1;
            this.l1.Text = "SHA256 해시값을 계산합니다.";
            // 
            // l2
            // 
            this.l2.AutoSize = true;
            this.l2.Location = new System.Drawing.Point(12, 38);
            this.l2.Name = "l2";
            this.l2.Size = new System.Drawing.Size(61, 12);
            this.l2.TabIndex = 2;
            this.l2.Text = "파일 경로:";
            // 
            // fileBrowser
            // 
            this.fileBrowser.Location = new System.Drawing.Point(442, 35);
            this.fileBrowser.Name = "fileBrowser";
            this.fileBrowser.Size = new System.Drawing.Size(31, 21);
            this.fileBrowser.TabIndex = 3;
            this.fileBrowser.Text = "...";
            this.fileBrowser.UseVisualStyleBackColor = true;
            this.fileBrowser.Click += new System.EventHandler(this.fileBrowser_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "exe 파일|*.exe|모든 파일|*";
            // 
            // calcButton
            // 
            this.calcButton.Location = new System.Drawing.Point(205, 62);
            this.calcButton.Name = "calcButton";
            this.calcButton.Size = new System.Drawing.Size(75, 23);
            this.calcButton.TabIndex = 4;
            this.calcButton.Text = "계산하기";
            this.calcButton.UseVisualStyleBackColor = true;
            this.calcButton.Click += new System.EventHandler(this.calcButton_Click);
            // 
            // hashResultText
            // 
            this.hashResultText.BackColor = System.Drawing.Color.White;
            this.hashResultText.Location = new System.Drawing.Point(12, 100);
            this.hashResultText.Name = "hashResultText";
            this.hashResultText.ReadOnly = true;
            this.hashResultText.Size = new System.Drawing.Size(461, 21);
            this.hashResultText.TabIndex = 5;
            // 
            // clipCopy
            // 
            this.clipCopy.Location = new System.Drawing.Point(186, 127);
            this.clipCopy.Name = "clipCopy";
            this.clipCopy.Size = new System.Drawing.Size(106, 23);
            this.clipCopy.TabIndex = 6;
            this.clipCopy.Text = "클립보드로 복사";
            this.clipCopy.UseVisualStyleBackColor = true;
            this.clipCopy.Click += new System.EventHandler(this.clipCopy_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(484, 157);
            this.Controls.Add(this.clipCopy);
            this.Controls.Add(this.hashResultText);
            this.Controls.Add(this.calcButton);
            this.Controls.Add(this.fileBrowser);
            this.Controls.Add(this.l2);
            this.Controls.Add(this.l1);
            this.Controls.Add(this.filePathText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "해시값 계산기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filePathText;
        private System.Windows.Forms.Label l1;
        private System.Windows.Forms.Label l2;
        private System.Windows.Forms.Button fileBrowser;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button calcButton;
        private System.Windows.Forms.TextBox hashResultText;
        private System.Windows.Forms.Button clipCopy;
    }
}

