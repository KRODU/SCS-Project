namespace SCS.Net.Server
{
    partial class AddDep
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
            this.lblDep = new System.Windows.Forms.Label();
            this.txtDep = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblCapInt = new System.Windows.Forms.Label();
            this.nudCapInt = new System.Windows.Forms.NumericUpDown();
            this.txtUnProtect = new System.Windows.Forms.TextBox();
            this.lblUnprotect = new System.Windows.Forms.Label();
            this.txtDepDes = new System.Windows.Forms.TextBox();
            this.lblDepDes = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudCapInt)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDep
            // 
            this.lblDep.AutoSize = true;
            this.lblDep.Location = new System.Drawing.Point(48, 21);
            this.lblDep.Name = "lblDep";
            this.lblDep.Size = new System.Drawing.Size(73, 12);
            this.lblDep.TabIndex = 0;
            this.lblDep.Text = "부서명 입력:";
            // 
            // txtDep
            // 
            this.txtDep.Location = new System.Drawing.Point(127, 18);
            this.txtDep.Name = "txtDep";
            this.txtDep.Size = new System.Drawing.Size(120, 21);
            this.txtDep.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(69, 140);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(118, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblCapInt
            // 
            this.lblCapInt.AutoSize = true;
            this.lblCapInt.Location = new System.Drawing.Point(32, 47);
            this.lblCapInt.Name = "lblCapInt";
            this.lblCapInt.Size = new System.Drawing.Size(89, 12);
            this.lblCapInt.TabIndex = 2;
            this.lblCapInt.Text = "화면 캡쳐 간격:";
            // 
            // nudCapInt
            // 
            this.nudCapInt.Location = new System.Drawing.Point(127, 45);
            this.nudCapInt.Name = "nudCapInt";
            this.nudCapInt.Size = new System.Drawing.Size(120, 21);
            this.nudCapInt.TabIndex = 3;
            this.nudCapInt.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // txtUnProtect
            // 
            this.txtUnProtect.Location = new System.Drawing.Point(127, 72);
            this.txtUnProtect.Name = "txtUnProtect";
            this.txtUnProtect.Size = new System.Drawing.Size(120, 21);
            this.txtUnProtect.TabIndex = 5;
            // 
            // lblUnprotect
            // 
            this.lblUnprotect.AutoSize = true;
            this.lblUnprotect.Location = new System.Drawing.Point(12, 75);
            this.lblUnprotect.Name = "lblUnprotect";
            this.lblUnprotect.Size = new System.Drawing.Size(109, 12);
            this.lblUnprotect.TabIndex = 4;
            this.lblUnprotect.Text = "보호해제 비밀번호:";
            // 
            // txtDepDes
            // 
            this.txtDepDes.Location = new System.Drawing.Point(127, 99);
            this.txtDepDes.Name = "txtDepDes";
            this.txtDepDes.Size = new System.Drawing.Size(120, 21);
            this.txtDepDes.TabIndex = 7;
            // 
            // lblDepDes
            // 
            this.lblDepDes.AutoSize = true;
            this.lblDepDes.Location = new System.Drawing.Point(60, 102);
            this.lblDepDes.Name = "lblDepDes";
            this.lblDepDes.Size = new System.Drawing.Size(61, 12);
            this.lblDepDes.TabIndex = 6;
            this.lblDepDes.Text = "부서 설명:";
            // 
            // AddDep
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(256, 175);
            this.ControlBox = false;
            this.Controls.Add(this.txtDepDes);
            this.Controls.Add(this.lblDepDes);
            this.Controls.Add(this.txtUnProtect);
            this.Controls.Add(this.lblUnprotect);
            this.Controls.Add(this.nudCapInt);
            this.Controls.Add(this.lblCapInt);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtDep);
            this.Controls.Add(this.lblDep);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddDep";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "부서 추가";
            ((System.ComponentModel.ISupportInitialize)(this.nudCapInt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDep;
        private System.Windows.Forms.TextBox txtDep;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblCapInt;
        private System.Windows.Forms.NumericUpDown nudCapInt;
        private System.Windows.Forms.TextBox txtUnProtect;
        private System.Windows.Forms.Label lblUnprotect;
        private System.Windows.Forms.TextBox txtDepDes;
        private System.Windows.Forms.Label lblDepDes;
    }
}