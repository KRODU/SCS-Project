using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Surveillance
{
    public partial class LoginForm : Form
    {
        /// <summary>
        /// 입력된 IP 주소를 반환합니다.
        /// </summary>
        public IPAddress IP => IPAddress.Parse(serverIPText.Text);

        /// <summary>
        /// 입력된 포트번호를 반환합니다.
        /// </summary>
        public ushort Port => (ushort)int.Parse(serverPortText.Text);

        public LoginForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try { IPAddress.Parse(serverIPText.Text); }
            catch
            {
                MessageBox.Show("올바른 IP주소가 아닙니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                serverIPText.Focus();
                return;
            }

            int portNum;

            if (!int.TryParse(serverPortText.Text, out portNum) || portNum < 0 || portNum > 65535)
            {
                MessageBox.Show("0부터 65535 사이의 올바른 포트번호를 입력해주세요.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                serverPortText.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
