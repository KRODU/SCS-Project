using System;
using System.Windows.Forms;

namespace SCS.Net.Server
{
    public partial class PortSelectForm : Form
    {
        public PortSelectForm()
        {
            InitializeComponent();
        }

        public ushort Port => ushort.Parse(portTextBox.Text);

        private void okButton_Click(object sender, EventArgs e)
        {
            if (portTextBox.TextLength == 0)
            {
                MessageBox.Show("포트번호를 입력해주세요.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                portTextBox.Focus();
                return;
            }

            try
            {
                ushort.Parse(portTextBox.Text);
                DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("1-65535 사이의 정수를 입력해주세요.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                portTextBox.Focus();
                return;
            }
        }
    }
}
