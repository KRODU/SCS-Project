using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SCS.Net.Server.DBConnector;

namespace SCS.Net.Server
{
    public partial class OrganizationForm : Form
    {
        private struct userTag
        {
            public int userCode;
            public string userIP;
            public string userName;

            public userTag(int userCode, string userIP, string userName)
            {
                this.userCode = userCode;
                this.userIP = userIP;
                this.userName = userName;
            }
        }

        public OrganizationForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.server_connection;
        }

        private void OrganizationForm_Load(object sender, EventArgs e)
        {
            RefreshViewer();
        }

        private void refButton_Click(object sender, EventArgs e)
        {
            RefreshViewer();
        }

        private void RefreshViewer()
        {
            orgView.Nodes.Clear();

            // 부서 목록을 가져옵니다.
            var depRows = ExecuteQuery("SELECT DepCode_Num, DepCode_Name FROM DepCode");

            foreach (DataRow eachDep in depRows)
            {
                // 부서별 사원 목록을 가져옵니다.
                var parentNode = orgView.Nodes.Add((string)eachDep[1]);
                var userRows = ExecuteQuery("SELECT User_Num, User_IP, User_Name FROM User WHERE DepCode_Num = '" + (int)eachDep[0] + "'");
                foreach (DataRow eachUser in userRows)
                {
                    parentNode.Nodes.Add((string)eachUser[2]).Tag = new userTag((int)eachUser[0], (string)eachUser[1], (string)eachUser[2]);
                }
            }
            orgView.ExpandAll();
            selectUser(false);
        }

        private void orgView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(orgView.SelectedNode.Tag is userTag))
            {
                selectUser(false);
                return;
            }
            userTag tagData = (userTag)orgView.SelectedNode.Tag;

            IpTextBox.Text = tagData.userIP;
            NameTextBox.Text = tagData.userName;
            selectUser(true);
        }

        private void selectUser(bool user)
        {
            if (!user)
            {
                IpTextBox.Text = "";
                NameTextBox.Text = "";
            }
            IpTextBox.Enabled = user;
            NameTextBox.Enabled = user;
            modifyButton.Enabled = user;
        }

        private void UserAddButton_Click(object sender, EventArgs e)
        {
            if (new AddUserForm().ShowDialog() == DialogResult.OK)
                RefreshViewer();
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (!(orgView.SelectedNode.Tag is userTag))
                return;

            userTag tagData = (userTag)orgView.SelectedNode.Tag;
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand();
            command.CommandText = "UPDATE User SET User_IP = @ip, User_Name = @userName WHERE User_Num = @userNum";
            command.Parameters.AddWithValue("@ip", IpTextBox.Text);
            command.Parameters.AddWithValue("@userName", NameTextBox.Text);
            command.Parameters.AddWithValue("@userNum", tagData.userCode);
            int result = ExecuteNonQuery(command);
            if (result == 1)
            {
                MessageBox.Show("정상적으로 수정되었습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshViewer();
            }
        }

        private void depAddButton_Click(object sender, EventArgs e)
        {
            if (new AddDep().ShowDialog() == DialogResult.OK)
            {
                RefreshViewer();
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogViewer().ShowDialog();
        }
    }
}
