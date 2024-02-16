using SCS.Net.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SCS.Net.Client
{
    /// <summary>
    /// 서버와의 연결을 관리하는 클래스입니다.
    /// </summary>
    public class NetClient_Connector : IDisposable
    {
        /// <summary>
        /// 연결 상태에 변화가 있을 때 호출되는 대리자입니다.
        /// </summary>
        public delegate void ConnectStateChange(ConnectState conState, int curRetryCount, Exception exception = null);

        /// <summary>
        /// 서버와 통신하는 TcpClient 객체 필드입니다.
        /// </summary>
        private volatile TcpClient m_TcpSocket;

        /// <summary>
        /// 서버와의 통신에 사용되는 TcpClient 객체를 반환합니다. 연결되지 않았을 경우 null입니다.
        /// </summary>
        public TcpClient TcpSocket
        {
            get
            {
                if (ConnectionState == ConnectState.Connected)
                    return m_TcpSocket;
                else
                    return null;
            }
        }

        /// <summary>
        /// 서버와 연결을 시도하는 스레드입니다.
        /// </summary>
        private Thread m_ConnectThread;

        /// <summary>
        /// 현재 연결 상태를 반환합니다.
        /// </summary>
        public ConnectState ConnectionState { get; private set; } = ConnectState.NotConnect;

        /// <summary>
        /// 연결 재시도를 위해 저장해놓은 서버 IP입니다.
        /// </summary>
        private IPAddress m_serverIP;

        /// <summary>
        /// 연결 재시도를 위해 저장해놓은 서버 포트번호입니다.
        /// </summary>
        private ushort m_port;

        /// <summary>
        /// 연결이 완료된 경우 이를 알리기 위한 객체입니다.
        /// </summary>
        private ManualResetEvent m_connectDone = new ManualResetEvent(false);

        /// <summary>
        /// 서버와 연결을 시도합니다. 이미 서버와 연결된 경우 무시됩니다.
        /// </summary>
        /// <param name="serverIP">연결할 IP주소입니다.</param>
        /// <param name="port">연결할 포트번호입니다.</param>
        /// <param name="maxRetryCount">연결 재시도 횟수입니다.</param>
        /// <param name="retryWait">재시도 시 대기할 시간입니다.</param>
        /// <param name="waitTime">연결 시도시 대기할 시간입니다.</param>
        /// <param name="conStateChange">연결 상태에 변화가 발생할 때 호출되는 대리자입니다. null일 경우 호출되지 않습니다.</param>
        public void ConnectServer(IPAddress serverIP, ushort port, int maxRetryCount, int retryWait, int waitTime, ConnectStateChange conStateChange = null)
        {
            if (serverIP == null)
                throw new ArgumentNullException();
            if (maxRetryCount < 0 || retryWait < 0)
                throw new ArgumentOutOfRangeException();

            lock (this)
            {
                this.m_serverIP = serverIP;
                this.m_port = port;
                // 서버와의 연결이 성공했는지 여부를 나타냅니다. (서버에서 연결을 거부한 경우, 이후 PingTest는 실패할 수도 있음)
                bool connected = false;

                // 이미 연결된 경우 무시
                if (m_TcpSocket != null && m_TcpSocket.Connected)
                    return;

                // 연결을 시도하는 스레드가 이미 동작중인 경우 무시
                if (m_ConnectThread != null && m_ConnectThread.IsAlive)
                    throw new NotImplementedException();

                // 스레드 객체 생성 및 스레드에서 실행할 코드 ----------
                m_ConnectThread = new Thread(delegate ()
                {
                    int curRetryCount = 0;
                    do
                    {
                        try
                        {
                            m_TcpSocket?.Close();
                            m_TcpSocket = new TcpClient();

                            // 연결중 상태로 설정하고 이를 알립니다.
                            ConnectionState = ConnectState.Connecting;
                            InvokeStateChangeEvent(conStateChange, ConnectState.Connecting, curRetryCount);

                            // 연결을 시도합니다.
                            m_connectDone.Reset();
                            m_TcpSocket.BeginConnect(serverIP, port, Async_ConnectCallback, m_TcpSocket);

                            // 재한 시간만큼 대기합니다.
                            if (!m_connectDone.WaitOne(waitTime))
                            {
                                // 재한 시간 초과시까지 연결되지 않은 경우 예외를 발생시켜 시간 초과 사실을 알립니다.
                                throw new TimeoutException("지정된 시간 안에 연결을 완료하지 못하였습니다.");
                            }
                            connected = true;
                        }
                        catch (Exception ex)
                        {
                            // 에러 발생 사실을 알립니다.
                            InvokeStateChangeEvent(conStateChange, ConnectState.ConnectError, curRetryCount, ex);

                            // 아직 재시도 횟수가 남은 경우, 연결 대기 상태로 설정하고 이를 알립니다.
                            if (curRetryCount < maxRetryCount)
                            {
                                ConnectionState = ConnectState.RetryWait;
                                InvokeStateChangeEvent(conStateChange, ConnectState.RetryWait, curRetryCount);

                                // 지정된 시간만큼 대기합니다.
                                if (retryWait > 0)
                                    Thread.Sleep(retryWait);
                            }
                        }
                    } while (!m_TcpSocket.Connected && curRetryCount++ < maxRetryCount);

                    // 연결 시도가 모두 끝난 이후의 처리입니다. -----------

                    // 재시도 횟수 내에서 성공적으로 연결된 경우
                    if (connected)
                    {
                        // 서버에서 연결을 수락한 경우 PingTest가 성공합니다.
                        if (RequestToServer(3))
                        {
                            ConnectionState = ConnectState.Connected;
                            InvokeStateChangeEvent(conStateChange, ConnectState.Connected, curRetryCount);
                        }
                        // 서버에서 연결을 거부한 경우
                        else
                        {
                            ConnectionState = ConnectState.ConnectRefuse;
                            InvokeStateChangeEvent(conStateChange, ConnectState.ConnectRefuse, curRetryCount);
                        }
                    }
                    // 연결에 오류가 발생한 경우, 연결 상태를 설정하고 이를 알립니다.
                    else
                    {
                        m_TcpSocket = null;
                        ConnectionState = ConnectState.ConnectFail;
                        InvokeStateChangeEvent(conStateChange, ConnectState.ConnectFail, curRetryCount);
                    }
                });
                // 여기까지 스레드에서 실행할 코드 ---------------------

                // 연결 스레드를 시작합니다.
                m_ConnectThread.Start();
            }
        }

        /// <summary>
        /// 연결 상태가 변경될 경우 이를 알립니다.
        /// </summary>
        private void InvokeStateChangeEvent(ConnectStateChange conStateChange, ConnectState conState, int curRetryCount, Exception exception = null)
        {
            if (conStateChange != null)
                new Thread(() => conStateChange(conState, curRetryCount, exception)).Start();
        }

        /// <summary>
        /// 연결 시도 이후 이곳에 콜백됩니다.
        /// </summary>
        private void Async_ConnectCallback(IAsyncResult arg)
        {
            TcpClient tcpObj = (TcpClient)arg.AsyncState;
            if (tcpObj.Client != null && tcpObj.Connected)
                m_TcpSocket.EndConnect(arg);
            m_connectDone.Set();
        }

        /// <summary>
        /// 서버와의 정상적인 연결을 확인하기 위해 서버의 응답을 요청합니다.
        /// </summary>
        /// <param name="timeOutSecond">서버의 응답이 없을 경우 대기할 최대 시간입니다.</param>
        private bool RequestToServer(int timeOutSecond)
        {
            bool succeed = false; // pingTest가 성공했는지를 나타냅니다.
            Thread sleepThread = Thread.CurrentThread; // receiveThread에서 인터럽트를 발생시킬 수 있도록 현재 스레드를 저장해놓습니다.

            // 서버의 응답을 받는 스레드입니다.
            Thread receiveThread = new Thread(delegate ()
            {
                try
                {
                    // 응답 요청을 보냅니다.
                    new SendingObj(SendingType.PingToServer).Serialize(m_TcpSocket.GetStream());

                    // 응답을 받습니다.
                    SendingObj receiveObj = SendingObj.Deserialize(m_TcpSocket.GetStream());

                    if (receiveObj.SendingType == SendingType.PingResponseToClient)
                        succeed = true;
                    else
                        succeed = false;

                    // 인터럽트를 발생시켜 Sleep를 중단합니다.
                    sleepThread.Interrupt();
                }
                catch { succeed = false; sleepThread.Interrupt(); }
            });
            receiveThread.Start();

            // 스레드가 완료될 때까지 지정된 대기시간 안에서 기다립니다.
            try { Thread.Sleep(1000 * timeOutSecond); }
            // 인터럽트가 발생할 경우 즉시 대기를 중단하고 발생하는 예외는 무시합니다.
            catch (ThreadInterruptedException) { }

            // 재한시간 안에 응답을 받은 경우 true를 반환합니다.
            if (succeed)
            {
                return true;
            }
            // 재한시간 안까지 응답을 성공적으로 받지 못한 경우 서버와의 연결을 닫아버리고 false를 반환합니다.
            else
            {
                Close();
                return false;
            }
        }

        /// <summary>
        /// 서버와의 연결을 닫습니다. 이미 연결이 닫혀있는 경우 작업은 무시됩니다.
        /// </summary>
        public void Close()
        {
            if (m_TcpSocket != null)
            {
                m_serverIP = null;
                m_TcpSocket.Close();
                m_TcpSocket = null;
            }
        }

        /// <summary>
        /// 서버와의 연결을 닫고 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        ~NetClient_Connector()
        {
            Dispose();
        }

        /// <summary>
        /// 현재 연결 상태를 나타냅니다.
        /// </summary>
        public enum ConnectState
        {
            /// <summary>
            /// 연결되지 않았습니다.
            /// </summary>
            NotConnect,
            /// <summary>
            /// 연결되었습니다.
            /// </summary>
            Connected,
            /// <summary>
            /// 연결 시도 중입니다.
            /// </summary>
            Connecting,
            /// <summary>
            /// 연결 시도 중 오류가 발생했습니다.
            /// </summary>
            ConnectError,
            /// <summary>
            /// 재시도 횟수만큼 연결을 재시도했으나 연결에 실패했습니다.
            /// </summary>
            ConnectFail,
            /// <summary>
            /// 연결 재시도를 기다리는 중입니다.
            /// </summary>
            RetryWait,
            /// <summary>
            /// 서버에서 연결을 거부했습니다.
            /// </summary>
            ConnectRefuse
        }
    }
}
