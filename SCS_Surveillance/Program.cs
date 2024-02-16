using SCS.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Surveillance
{
    static class Program
    {
        /// <summary>
        /// 서버 설정이 저장되는 파일 경로를 반환합니다.
        /// </summary>
        public static readonly string ServerInfoFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System) + Path.DirectorySeparatorChar + "SCS C";

        /// <summary>
        /// 서버와 연결하는 객체입니다. 이 객체는 먼저 생성됩니다.
        /// </summary>
        public static NetClient_Connector connectorObj { get; private set; } = new NetClient_Connector();

        /// <summary>
        /// 감시 클래스들을 관리하는 클래스입니다. 이 객체는 서버와의 연결이 성립된 이후에 생성됩니다.
        /// </summary>
        public static MonitorManager monitorObj { get; private set; }

        /// <summary>
        /// 트레이 아이콘입니다.
        /// </summary>
        public static NotifyIcon ProgramTrayIcon { get; private set; }

        /// <summary>
        /// 트레이 아이콘 메뉴입니다.
        /// </summary>
        private static ContextMenuStrip trayMenu;

        /// <summary>
        /// 트레이 아이콘 메뉴로 표시되는 '보호 해제' 버튼 입니다.
        /// </summary>
        private static ToolStripMenuItem unLockMenuItem;

        /// <summary>
        /// 파일 삭제를 방지하기 위해 서버 설정이 저장되는 파일에 대한 스트림을 일부러 열어놓습니다.
        /// </summary>
        private static Stream m_ServerFileStream;

        /// <summary>
        /// 프로그램이 종료되어야 하는지 여부가 저장됩니다.
        /// </summary>
        public static bool m_ProgramExit { get; private set; } = false;

        /// <summary>
        /// 로그아웃 폼입니다.
        /// </summary>
        private static LogoutForm m_logOutObj;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 트레이 아이콘 만들기 및 트레이 아이콘의 메뉴 초기화 작업 --------------------------

            ProgramTrayIcon = new NotifyIcon();
            trayMenu = new ContextMenuStrip();
            unLockMenuItem = new ToolStripMenuItem();

            ProgramTrayIcon.ContextMenuStrip = trayMenu;
            ProgramTrayIcon.Text = Application.ProductName;
            ProgramTrayIcon.Visible = true;
            ProgramTrayIcon.DoubleClick += ShowLogoutForm;
            ProgramTrayIcon.Icon = Properties.Resources.gear_forbidden;

            trayMenu.Items.AddRange(new ToolStripItem[] { unLockMenuItem });
            trayMenu.Name = "trayMenu";
            trayMenu.Size = new System.Drawing.Size(127, 26);

            unLockMenuItem.Name = "unLockMenuItem";
            unLockMenuItem.Size = new System.Drawing.Size(126, 22);
            unLockMenuItem.Text = "보호 해제";
            unLockMenuItem.Click += ShowLogoutForm;

            // 서버에 연결하기 위해 사용할 정보가 이미 저장되어 있는 경우 읽습니다.
            if (false)
            {
                try
                {
                    m_ServerFileStream = File.Open(ServerInfoFilePath, FileMode.Open);

                    // BinaryReader를 이용해서 읽습니다.
                    using (BinaryReader br = new BinaryReader(m_ServerFileStream, Encoding.ASCII, true))
                    {
                        TryConnectServer(new IPAddress(br.ReadBytes(4)), br.ReadUInt16());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("서버에 대한 설정 파일을 읽던 도중 오류가 발생했습니다.\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // 오류가 발생한 경우 기존 파일을 삭제함.
                    if (m_ServerFileStream != null)
                        m_ServerFileStream.Close();

                    if (File.Exists(ServerInfoFilePath))
                        File.Delete(ServerInfoFilePath);

                    InputServerInfo();
                }

            }
            // 저장된 서버 정보가 없는 경우 새로 입력하게 합니다.
            else
            {
                InputServerInfo();
            }

            if (!m_ProgramExit)
                Application.Run(); // 메시지 루프를 시작합니다.
        }

        /// <summary>
        /// 서버와의 연결을 시도합니다.
        /// </summary>
        private static void TryConnectServer(IPAddress ip, ushort port)
        {
            // 서버로 연결을 시도합니다. conStateChangeEvent로 결과가 전달됩니다.
            connectorObj.ConnectServer(ip, port, 3, 1000 * 5, 3000, conStateChangeShow);
        }

        /// <summary>
        /// 서버와의 연결 상태에 변화가 발생한 경우 호출됩니다.
        /// </summary>
        private static void conStateChangeShow(NetClient_Connector.ConnectState conState, int curRetryCount, Exception exception)
        {
            switch (conState)
            {
                // 성공적으로 연결된 경우
                case NetClient_Connector.ConnectState.Connected:
                    monitorObj = new MonitorManager(connectorObj.TcpSocket); // 모니터링 클래스를 초기화합니다.
                    ProgramTrayIcon.ShowBalloonTip(2000, Application.ProductName, "서버에 성공적으로 연결되었습니다.", ToolTipIcon.Info);
                    break;
                case NetClient_Connector.ConnectState.ConnectError:
                    ProgramTrayIcon.ShowBalloonTip(2000, Application.ProductName, "[" + curRetryCount + "번째 재시도] 서버와의 연결에 실패했습니다.\r\n" + exception.Message, ToolTipIcon.Error);
                    break;
                case NetClient_Connector.ConnectState.ConnectFail:
                    MessageBox.Show("서버 연결 재시도 횟수가 초과하였습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InputServerInfo();
                    break;
                case NetClient_Connector.ConnectState.ConnectRefuse:
                    MessageBox.Show("서버에서 연결을 거부했습니다.\r\n접속이 허가된 IP주소가 아닐 수 있습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InputServerInfo();
                    break;
            }
        }

        /// <summary>
        /// 연결 정보가 저장된 파일이 없거나 서버와의 연결이 불가능할 경우 서버 정보를 새로 입력하기 위해 호출됩니다.
        /// </summary>
        private static void InputServerInfo()
        {
            try
            {
                if (File.Exists(ServerInfoFilePath))
                {
                    if (m_ServerFileStream != null)
                        m_ServerFileStream.Close();
                    File.Delete(ServerInfoFilePath);
                }
            }
            catch { Debug.Fail("ServerFile Delete Fail"); }

            LoginForm loginFormObj = new LoginForm();

            // 정상적인 IP와 포트번호가 입력된 경우
            if (loginFormObj.ShowDialog() == DialogResult.OK)
            {
                // 스트림이 이미 열려있는 경우 닫음.
                if (m_ServerFileStream != null)
                    m_ServerFileStream.Close();

                // 새로운 파일을 만들어서 스트림을 연다. 이미 있을 경우 덮어 씌운다.
                m_ServerFileStream = File.Open(ServerInfoFilePath, FileMode.Create);

                using (BinaryWriter bw = new BinaryWriter(m_ServerFileStream, Encoding.UTF8, true))
                {
                    bw.Write(loginFormObj.IP.GetAddressBytes());
                    bw.Write(loginFormObj.Port);
                }

                // 서버에 연결합니다.
                TryConnectServer(loginFormObj.IP, loginFormObj.Port);
            }
            // 서버 정보 입력이 취소된 경우 프로그램을 닫습니다.
            else
            {
                EndProgram();
            }
        }

        /// <summary>
        /// 트레이 아이콘을 클릭하면 로그아웃 창을 보여줍니다.
        /// </summary>
        private static void ShowLogoutForm(object sender, EventArgs e)
        {
            if (monitorObj != null)
            {
                if (m_logOutObj == null || m_logOutObj.IsDisposed)
                {
                    m_logOutObj = new LogoutForm();
                    m_logOutObj.Show();
                }
                m_logOutObj.Focus();
            }
        }

        /// <summary>
        /// 모든 감시 프로세스를 종료하고 프로그램을 종료합니다.
        /// </summary>
        internal static void EndProgram()
        {
            m_ProgramExit = true;

            // 컴퓨터 감시 객체 해제
            if (monitorObj != null)
                monitorObj.Dispose();

            // 서버와의 연결 해제
            connectorObj.Dispose();

            // 컨트롤 객체 해제
            //unLockMenuItem.Dispose();
            //trayMenu.BeginInvoke(new Action(() => trayMenu.Dispose()));
            ProgramTrayIcon.Dispose();

            // 프로그램을 종료합니다.
            Application.Exit();
        }
    }
}
