namespace SCS.Net.Server
{
    partial class PortSelectForm
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
            this.portLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.desLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(27, 46);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(57, 12);
            this.portLabel.TabIndex = 0;
            this.portLabel.Text = "포트번호:";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(99, 43);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(55, 21);
            this.portTextBox.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(26, 70);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(128, 30);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "확인";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // desLabel
            // 
            this.desLabel.AutoSize = true;
            this.desLabel.Location = new System.Drawing.Point(12, 9);
            this.desLabel.Name = "desLabel";
            this.desLabel.Size = new System.Drawing.Size(157, 24);
            this.desLabel.TabIndex = 3;
            this.desLabel.Text = "서버에서 사용할 포트번호를\r\n지정합니다.";
            // 
            // PortSelectForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(184, 124);
            this.Controls.Add(this.desLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.portLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PortSelectForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label desLabel;
    }
}