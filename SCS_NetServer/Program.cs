using SCS.Common;
using SCS.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SCS.Net.Server.DBConnector;

namespace SCS.Net.Server
{
    static class Program
    {
        /// <summary>
        /// 서버에서 사용할 하나의 <see cref="DataQueueSender{T}"/> 객체입니다.
        /// </summary>
        public static DataQueueSender<SendingObj> m_QueueSender { private set; get; } = new DataQueueSender<SendingObj>();

        /// <summary>
        /// 클라이언트로부터 받은 데이터를 처리하는 객체입니다.
        /// </summary>
        private static NetServer_Receiver m_serverReceiver;

        /// <summary>
        /// 클라이언트의 연결을 받는 객체입니다.
        /// </summary>
        private static NetServer_Listener m_listenerObj = new NetServer_Listener();

        /// <summary>
        /// 새로운 연결이 발생할 때 호출됩니다.
        /// </summary>
        public static event Action<IPAddress> ConEvent;

        /// <summary>
        /// 연결이 끊어질 때 호출됩니다.
        /// </summary>
        public static event Action<IPAddress> DisconEvent;

        /// <summary>
        /// 연결된 IP주소 목록을 가져옵니다.
        /// </summary>
        public static IPAddress[] ConnectionList => m_listenerObj.connectedIP;

        /// <summary>
        /// 트레이 아이콘입니다.
        /// </summary>
        private static NotifyIcon trayIcon;

        /// <summary>
        /// 트레이 아이콘 메뉴입니다.
        /// </summary>
        private static ContextMenuStrip trayMenu;

        /// <summary>
        /// 트레이 아이콘 메뉴로 표시되는 '서버 종료' 버튼 입니다.
        /// </summary>
        private static ToolStripMenuItem serverExitMenuItem;

        /// <summary>
        /// 서버 로그 기록기입니다.
        /// </summary>
        public static Logger m_Logger { get; private set; } = new Logger();

        /// <summary>
        /// 서버 UI 폼입니다.
        /// </summary>
        private static OrganizationForm m_ServerUI;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PortSelectForm psf = new PortSelectForm();
            if (psf.ShowDialog() != DialogResult.OK)
            {
                ServerExit();
                return;
            }

            m_listenerObj.OpenPort(psf.Port);
            m_listenerObj.NewConnection += (IPAddress ip) => ConEvent?.Invoke(ip);
            m_listenerObj.NewConnection += ListenerObj_NewConnection;
            psf.Dispose();
            psf = null;
            m_serverReceiver = new NetServer_Receiver(m_listenerObj);
            m_serverReceiver.DisconnectedEvent += ServerReceiver_DisconnectedEvent;
            m_listenerObj.IPFilter = IPFilterRet;

            // 트레이 아이콘 및 연결되는 메뉴 작성
            trayIcon = new NotifyIcon();
            trayMenu = new ContextMenuStrip();
            serverExitMenuItem = new ToolStripMenuItem();

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Text = Application.ProductName;
            trayIcon.Visible = true;
            trayIcon.Icon = Properties.Resources.server_connection;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            trayMenu.Items.AddRange(new ToolStripItem[] { serverExitMenuItem });
            trayMenu.Name = "trayMenu";
            trayMenu.Size = new System.Drawing.Size(127, 26);

            serverExitMenuItem.Name = "serverExitMenuItem";
            serverExitMenuItem.Size = new System.Drawing.Size(126, 22);
            serverExitMenuItem.Text = "서버 종료";
            serverExitMenuItem.Click += ServerEndCheck;

            TrayIcon_DoubleClick(null, null); // 로그창을 띄웁니다.

            Application.Run();
        }

        /// <summary>
        /// 지정된 IP만 접속할 수 있도록 설정합니다.
        /// </summary>
        private static bool IPFilterRet(IPAddress ip)
        {
            if (GetUserCode(ip) == -1)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 새로운 연결이 발생한 경우 호출됩니다.
        /// </summary>
        private static void ListenerObj_NewConnection(IPAddress ip)
        {
            ExecuteNonQuery("UPDATE User SET LoginStats = '1' WHERE User_IP = '" + ip.ToString() + "'");
        }

        private static void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (m_ServerUI == null || m_ServerUI.IsDisposed)
            {
                m_ServerUI = new OrganizationForm();
                m_ServerUI.Show();
            }
        }

        /// <summary>
        /// 클라이언트와의 연결이 끊긴 경우 호출됩니다.
        /// </summary>
        private static void ServerReceiver_DisconnectedEvent(IPAddress obj)
        {
            ExecuteNonQuery("UPDATE User SET LoginStats = '0' WHERE User_IP = '" + obj.ToString() + "'");
            m_listenerObj.CloseConnection(obj);
            DisconEvent?.Invoke(obj);
        }

        private static void ServerEndCheck(object sender, EventArgs e)
        {
            if (MessageBox.Show("정말로 서버를 종료하겠습니까?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                ServerExit();
        }

        /// <summary>
        /// 서버를 종료합니다.
        /// </summary>
        private static void ServerExit()
        {
            // 컨트롤 객체 해제
            if (serverExitMenuItem != null)
                serverExitMenuItem.Dispose();
            if (trayMenu != null)
                trayMenu.Dispose();
            if (trayIcon != null)
                trayIcon.Dispose();
            if (m_serverReceiver != null)
                m_serverReceiver.Dispose();

            DBConnector.Close();

            m_QueueSender.Dispose();
            m_listenerObj.Dispose();
            m_Logger.Dispose();
            Application.Exit();
        }
    }
}
