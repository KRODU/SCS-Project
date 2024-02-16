using SCS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HashCalc
{
    public partial class MainForm : Form
    {
        private readonly Size extendSize = new Size(500, 195);

        private readonly Size contractSize = new Size(500, 130);

        public MainForm()
        {
            InitializeComponent();
            Size = contractSize;
            Icon = Properties.Resources.transform2;
        }

        private void calcButton_Click(object sender, EventArgs e)
        {
            try
            {
                hashResultText.Text = FileHash.GetFileSHA256Str(filePathText.Text);
                Size = extendSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류가 발생했습니다.\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Size = contractSize;
            }
        }

        private void fileBrowser_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathText.Text = openFileDialog.FileName;
            }
        }

        private void clipCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(hashResultText.Text);
            MessageBox.Show("클립보드로 복사되었습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void filePathText_TextChanged(object sender, EventArgs e)
        {
            Size = contractSize;
        }
    }
}
