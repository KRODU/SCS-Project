using SCS.Net.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace SCS.Net.Client
{
    /// <summary>
    /// 서버에서 보낸 데이터를 받아들이는 클래스입니다.
    /// </summary>
    public class NetClient_Receiver
    {
        /// <summary>
        /// 서버에서 예외를 전달한 경우 호출되는 이벤트입니다.
        /// </summary>
        public event ExceptionHandler ExceptionEvent;

        /// <summary>
        /// <see cref="SendingType.SpreadMessageToClient"/> 데이터를 처리하는 이벤트입니다.
        /// </summary>
        public event Action<string> SpreadMessageToClientEvent;

        /// <summary>
        /// <see cref="SendingType.RestrictionInitDataToClinet"/> 데이터를 처리하는 이벤트입니다.
        /// </summary>
        public event Action<RestrictionInitTag> RestrictionInitEvent;

        /// <summary>
        /// <see cref="SendingType.PasswordResponseToClient"/> 데이터를 처리하는 이벤트입니다.
        /// </summary>
        public event Action<bool> PasswordCheckEvent;

        /// <summary>
        /// 서버와의 연결이 끊어질 경우 호출됩니다.
        /// </summary>
        public event Action<Exception> ReceivingDisconnect;

        /// <summary>
        /// 데이터를 받아들일 때 사용할 TcpClient 객체를 반환합니다.
        /// </summary>
        public TcpClient TcpSocket { get; private set; }

        /// <summary>
        /// 데이터를 받아들일 때 사용되는 스레드입니다.
        /// </summary>
        private Thread m_ReceivingThread;

        /// <summary>
        /// 데이터를 받고있는 중인지 여부를 나타냅니다.
        /// </summary>
        public bool m_isReceiving { get; private set; } = false;

        /// <summary>
        /// 데이터를 받기 시작합니다.
        /// </summary>
        public void StartReceive(TcpClient tcpSocket)
        {
            if (tcpSocket == null || !tcpSocket.Connected)
                throw new ArgumentException();

            TcpSocket = tcpSocket;
            m_isReceiving = true;
            m_ReceivingThread = new Thread(ReceivingData);
            m_ReceivingThread.Start();
        }

        /// <summary>
        /// 스레드에서 실행될 데이터 수신 메서드입니다.
        /// </summary>
        private void ReceivingData()
        {
            SendingObj receivedData;
            while (TcpSocket.Connected && m_isReceiving)
            {
                try
                {
                    receivedData = SendingObj.Deserialize(TcpSocket.GetStream());
                }
                catch (Exception ex)
                {
                    ReceivingDisconnect(ex);
                    return;
                }

                // 예외가 전송되어 들어온 경우
                if (receivedData.Except != null)
                {
                    ExceptionEvent?.Invoke(receivedData.Except);
                    continue;
                }

                switch (receivedData.SendingType)
                {
                    case SendingType.SpreadMessageToClient:
                        SpreadMessageToClientEvent((string)receivedData.TagData);
                        break;
                    case SendingType.RestrictionInitDataToClinet:
                        RestrictionInitEvent((RestrictionInitTag)receivedData.TagData);
                        break;
                    case SendingType.PasswordResponseToClient:
                        PasswordCheckEvent((bool)receivedData.TagData);
                        break;
                }
            }
        }
    }
}
