using MySql.Data.MySqlClient;
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
    public partial class AddUserForm : Form
    {
        private List<int> depCodeList = new List<int>();

        public AddUserForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.server_connection;
        }

        private void AddUser_Load(object sender, EventArgs e)
        {
            // 부서 목록 가져오기
            var depRows = ExecuteQuery("SELECT DepCode_Num, DepCode_Name FROM DepCode");

            foreach (DataRow eachDep in depRows)
            {
                depComboBox.Items.Add((string)eachDep[1]);
                depCodeList.Add((int)eachDep[0]);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (depComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("부서명을 입력하지 않았습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                depComboBox.Focus();
                return;
            }

            if (IpTextBox.TextLength == 0)
            {
                MessageBox.Show("IP주소를 입력하지 않았습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                IpTextBox.Focus();
                return;
            }

            if (NameTextBox.TextLength == 0)
            {
                MessageBox.Show("이름을 입력하지 않았습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NameTextBox.Focus();
                return;
            }

            MySqlCommand command = new MySqlCommand();
            command.CommandText = "INSERT INTO User (User_IP, User_Name, DepCode_Num) VALUES(@userIP, @userName, @depCode);";
            command.Parameters.AddWithValue("@userIP", IpTextBox.Text);
            command.Parameters.AddWithValue("@userName", NameTextBox.Text);
            command.Parameters.AddWithValue("@depCode", depCodeList[depComboBox.SelectedIndex]);

            if (ExecuteNonQuery(command) == 1)
            {
                MessageBox.Show("정상적으로 등록되었습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }
    }
}
