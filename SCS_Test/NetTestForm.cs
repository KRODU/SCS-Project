using SCS.Common;
using SCS.Net.Client;
using SCS.Net.Common;
using SCS.Net.Server;
using SCS.Surveillance;
using System;
using System.Net;
using System.Windows.Forms;

namespace SCS_Test
{
    public partial class NetTestForm : Form
    {
        // 서버측
        private NetServer_Listener listener;
        private NetServer_Receiver serverReceiver;

        // 클라측
        private NetClient_Connector clinet1;
        private MonitorManager monitor;

        public NetTestForm()
        {
            InitializeComponent();

            listener = new NetServer_Listener();
            serverReceiver = new NetServer_Receiver(listener);
            listener.OpenPort(55);
            clinet1 = new NetClient_Connector();
            clinet1.ConnectServer(IPAddress.Parse("127.0.0.1"), 55, 1, 10, 5000, connectedAfter);
            Show();
        }

        private void connectedAfter(NetClient_Connector.ConnectState conState, int curRetryCount, Exception exception = null)
        {
            if (conState == NetClient_Connector.ConnectState.Connected)
            {
                monitor = new MonitorManager(clinet1.TcpSocket);
            }
        }

        private void Receiver_SpreadMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            monitor.InstantCapture();
        }

        private void NetTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            monitor.Dispose();
            listener.Dispose();
            clinet1.Dispose();
            Dispose();
        }
    }
}
