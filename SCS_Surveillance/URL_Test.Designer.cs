﻿namespace SCS.Surveillance
{
    partial class URL_Test
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.bt_Insert = new System.Windows.Forms.Button();
            this.bt_Delete = new System.Windows.Forms.Button();
            this.bt_Check = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 15);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "정보출력";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(106, 15);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 29);
            this.button2.TabIndex = 1;
            this.button2.Text = "핸들얻기";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(199, 13);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(86, 29);
            this.button4.TabIndex = 3;
            this.button4.Text = "종료";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // bt_Insert
            // 
            this.bt_Insert.Location = new System.Drawing.Point(25, 291);
            this.bt_Insert.Name = "bt_Insert";
            this.bt_Insert.Size = new System.Drawing.Size(75, 23);
            this.bt_Insert.TabIndex = 4;
            this.bt_Insert.Text = "추가";
            this.bt_Insert.UseVisualStyleBackColor = true;
            this.bt_Insert.Click += new System.EventHandler(this.bt_Insert_Click);
            // 
            // bt_Delete
            // 
            this.bt_Delete.Location = new System.Drawing.Point(117, 291);
            this.bt_Delete.Name = "bt_Delete";
            this.bt_Delete.Size = new System.Drawing.Size(75, 23);
            this.bt_Delete.TabIndex = 4;
            this.bt_Delete.Text = "삭제";
            this.bt_Delete.UseVisualStyleBackColor = true;
            this.bt_Delete.Click += new System.EventHandler(this.bt_Delete_Click);
            // 
            // bt_Check
            // 
            this.bt_Check.Location = new System.Drawing.Point(210, 291);
            this.bt_Check.Name = "bt_Check";
            this.bt_Check.Size = new System.Drawing.Size(75, 23);
            this.bt_Check.TabIndex = 4;
            this.bt_Check.Text = "확인";
            this.bt_Check.UseVisualStyleBackColor = true;
            this.bt_Check.Click += new System.EventHandler(this.bt_Check_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 270);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "방화벽";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(14, 85);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(271, 37);
            this.button3.TabIndex = 6;
            this.button3.Text = "Naver Close";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(14, 157);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(271, 37);
            this.button5.TabIndex = 6;
            this.button5.Text = "카카오톡, 메모장, 그림판 Close";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // URL_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 326);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_Check);
            this.Controls.Add(this.bt_Delete);
            this.Controls.Add(this.bt_Insert);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "URL_Test";
            this.Text = "URL_Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button bt_Insert;
        private System.Windows.Forms.Button bt_Delete;
        private System.Windows.Forms.Button bt_Check;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button5;
    }
}