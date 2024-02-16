using SCS.Net.Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace SCS.Net.Server
{
    /// <summary>
    /// 클라이언트로 데이터를 전송하는 클래스입니다.
    /// </summary>
    public class NetServer_Sender
    {
        /// <summary>
        /// 연결 목록을 참조하기 위해 사용되는 <see cref="NetServer_Listener"/> 객체입니다.
        /// </summary>
        private NetServer_Listener m_listenerObj;

        public NetServer_Sender(NetServer_Listener listenerObj)
        {
            if (listenerObj == null)
                throw new ArgumentNullException();

            m_listenerObj = listenerObj;
        }

        /// <summary>
        /// 지정된 IP주소로 메시지를 보냅니다. 지정된 IP주소 중에 연결되지 않은 IP가 있을 경우 무시됩니다.
        /// </summary>
        /// <param name="desIP">메시지를 보낼 IP주소 배열입니다.</param>
        /// <param name="message">보낼 메시지입니다.</param>
        public SendingSpecResult[] SendMessage(IPAddress[] desIP, string message)
        {
            if (desIP == null || desIP.Length == 0 || message == null)
                throw new ArgumentNullException();

            IPAddress eachIP;
            NetworkStream eachStream;
            SendingSpecResult[] ret = new SendingSpecResult[desIP.Length];

            lock (m_listenerObj)
            {
                for (int i = 0; i < desIP.Length; i++)
                {
                    eachIP = desIP[i];

                    // 지정된 IP와 연결된 경우에만 통신
                    if (m_listenerObj.IsConnected(eachIP))
                    {
                        try
                        {
                            // 네트워크 스트림을 가져옵니다.
                            eachStream = m_listenerObj.GetNetworkStream(eachIP);
                            // 메시지를 씁니다.
                            new SendingObj(SendingType.SpreadMessageToClient, message).Serialize(eachStream);
                            // 성공으로 표기합니다.
                            ret[i] = new SendingSpecResult(eachIP, SendResult.Succeed);
                        }
                        catch (Exception ex)
                        {
                            // 전송오류를 표기합니다.
                            ret[i] = new SendingSpecResult(eachIP, SendResult.SendingError, ex);
                        }
                    }
                    // 연결되지 않은 경우
                    else
                    {
                        // 연결되지 않았음을 표기합니다.
                        ret[i] = new SendingSpecResult(eachIP, SendResult.NotConnected);
                    }
                }
            }

            return ret;
        }
    }
}
