using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Net.Server
{
    public partial class ServerMainForm : Form
    {
        public ServerMainForm()
        {
            InitializeComponent();
        }

        private void ServerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("정말로 서버를 종료하겠습니까?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
