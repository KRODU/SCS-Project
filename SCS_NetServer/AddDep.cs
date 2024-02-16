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
using MySql.Data.MySqlClient;

namespace SCS.Net.Server
{
    public partial class AddDep : Form
    {
        public AddDep()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtDep.TextLength == 0)
            {
                MessageBox.Show("부서명을 입력하지 않았습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDep.Focus();
                return;
            }

            if (txtUnProtect.TextLength == 0)
            {
                MessageBox.Show("보호해제 비밀번호를 입력하지 않았습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnProtect.Focus();
                return;
            }

            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = "SELECT DepCode_Num FROM DepCode WHERE DepCode_Name = @depName;";
            comm.Parameters.AddWithValue("@depName", txtDep.Text);

            var duplCheck = ExecuteQuery(comm);
            if (duplCheck.Count != 0)
            {
                MessageBox.Show("같은 부서명이 이미 존재합니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDep.Focus();
                return;
            }

            int depCode = 0;
            var depCodeRows = ExecuteQuery("SELECT MAX(DepCode_Num) FROM DepCode");
            if (depCodeRows.Count > 0)
            {
                depCode = (int)depCodeRows[0][0];
                depCode++;
            }

            comm = new MySqlCommand();
            comm.CommandText = "INSERT INTO DepCode (DepCode_Num, DepCode_Name, CaptureInterval, ProtectPassword, DepInfo) VALUES " +
                "(@depCode, @DepCode_Name, @CaptureInterval, @ProtectPassword, @DepInfo);";
            comm.Parameters.AddWithValue("@depCode", depCode);
            comm.Parameters.AddWithValue("@DepCode_Name", txtDep.Text);
            comm.Parameters.AddWithValue("@CaptureInterval", nudCapInt.Value);
            comm.Parameters.AddWithValue("@ProtectPassword", txtUnProtect.Text);
            comm.Parameters.AddWithValue("@DepInfo", txtDepDes.Text);
            if (ExecuteNonQuery(comm) == 1)
            {
                MessageBox.Show("부서가 등록되었습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
