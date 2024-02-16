using SCS.Common;
using SCS.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Net.Client
{
    [Obsolete(null, true)]
    public class NetClient_Sender
    {
        /// <summary>
        /// 서버와 연결하는 객체입니다.
        /// </summary>
        private TcpClient m_TcpClient;

        /// <summary>
        /// 큐에 담아 서버에 전송하기 위한 객체입니다.
        /// </summary>
        private DataQueueSender<SendingObj> m_QueueSender = new DataQueueSender<SendingObj>();

        public NetClient_Sender(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException();

            m_TcpClient = tcpClient;
        }

        /// <summary>
        /// 현재 <see cref="m_TcpClient"/> 객체가 연결된 상태인지를 점검하고,
        /// 연결되지 않았을 경우 <see cref="NotConnectedException"/>을 throw 합니다.
        /// </summary>
        private void TcpConnectCheck()
        {
            if (m_TcpClient == null || !m_TcpClient.Connected)
                throw new NotConnectedException();
        }

        /// <summary>
        /// 캡쳐된 화면 이미지를 서버로 전송합니다.
        /// </summary>
        public void SendScreen(System.Drawing.Image image)
        {
            if (image == null)
                throw new ArgumentNullException();

            TcpConnectCheck();

            // 이미지 전송
            m_QueueSender.EnqueueData(m_TcpClient.GetStream(), new SendingObj(SendingType.CapturedScreenToServer, new CapturedScreenTag(DateTime.Now, image)));
        }

        /// <summary>
        /// 서버와의 연결을 확인하게 위해 서버의 응답을 요청합니다.
        /// </summary>
        public void Ping()
        {
            TcpConnectCheck();
            m_QueueSender.EnqueueData(m_TcpClient.GetStream(), new SendingObj(SendingType.PingToServer));
        }
    }
}
