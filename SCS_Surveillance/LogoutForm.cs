using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Surveillance
{
    public partial class LogoutForm : Form
    {
        public LogoutForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.gear_forbidden;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Program.monitorObj == null)
            {
                MessageBox.Show("이 컴퓨터는 이미 보호 상태가 아닙니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            if (passTextBox.TextLength == 0)
            {
                MessageBox.Show("패스워드를 입력하지 않았습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passTextBox.Focus();
                return;
            }

            Program.monitorObj.PasswordCheck(passTextBox.Text);
        }
    }
}
