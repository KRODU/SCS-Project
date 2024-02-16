using SCS.Net.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SCS.Net.Server
{
    /// <summary>
    /// 연결 수신 및 연결 종료를 관리하는 클래스입니다. <see cref="NetServer_Sender"/> 클래스는 이 클래스에 종속적입니다.
    /// </summary>
    public class NetServer_Listener : IDisposable
    {
        /// <summary>
        /// 연결 시도가 들어올 때 연결을 시도하는 IP가 전달되며, 해당 IP주소로 연결할지 여부를 반환해야 합니다.
        /// 반환값이 true이면 연결하며, false이거나 예외가 throw된 경우 연결하지 않습니다.
        /// </summary>
        public delegate bool ListenerIPFilter(IPAddress ip);

        /// <summary>
        /// 연결 시도가 들어올 때 연결을 받아들일지 여부를 결정하는 대리자를 설정하거나 가져옵니다.
        /// null일 경우 모든 연결 요청을 받아들입니다.
        /// </summary>
        public ListenerIPFilter IPFilter { get; set; }

        /// <summary>
        /// 새로운 연결이 발생할 때 호출됩니다.
        /// </summary>
        public delegate void NewConnectionEvent(IPAddress ip);

        /// <summary>
        /// 새로운 연결이 발생할 때 호출됩니다.
        /// </summary>
        public event NewConnectionEvent NewConnection;

        /// <summary>
        /// 서버로 연결할 때 필요한 포트 번호 필드입니다.
        /// </summary>
        private ushort m_PortNumber;

        /// <summary>
        /// 서버로 연결할 때 필요한 포트 번호를 반환합니다. 포트가 열린 상태가 아닐 경우 -1을 반환합니다.
        /// </summary>
        public int PortNumber { get { lock (this) return IsPortOpened ? m_PortNumber : -1; } }

        /// <summary>
        /// 서버가 연결을 수신중인지 여부를 가져옵니다.
        /// </summary>
        public bool IsPortOpened { get { lock (this) return m_Listener != null; } }

        /// <summary>
        /// 연결을 수신할 TcpListener 입니다.
        /// </summary>
        private TcpListener m_Listener;

        /// <summary>
        /// 받아들인 연결을 저장하는 컬렉션입니다.
        /// </summary>
        private Dictionary<IPAddress, TcpClient> m_AcceptedConnection = new Dictionary<IPAddress, TcpClient>();

        /// <summary>
        /// 현재 연결된 IP주소 목록을 가져옵니다.
        /// </summary>
        public IPAddress[] connectedIP { get { lock (this) { return m_AcceptedConnection.Keys.ToArray(); } } }

        /// <summary>
        /// 현재 객체가 해제되었는지 여부를 반환합니다.
        /// </summary>
        public bool IsDisposed { get { lock (this) { return m_AcceptedConnection == null; } } }

        /// <summary>
        /// 수신 요청을 받아들이는 스레드입니다.
        /// </summary>
        private Thread m_AcceptingConnectionThread;

        /// <summary>
        /// 지정된 IP주소와 현재 연결되었는지 여부를 반환합니다.
        /// </summary>
        /// <param name="ip">연결 여부를 확인할 IP주소입니다.</param>
        public bool IsConnected(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException();

            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NetServer_Listener));

                // 키가 없을 경우 false
                if (!m_AcceptedConnection.ContainsKey(ip))
                    return false;

                return m_AcceptedConnection[ip].Connected;
            }
        }

        /// <summary>
        /// 특정 IP주소에 대한 NetworkStream을 가져옵니다. 해당 IP와 연결되지 않았을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="ip">NetworkStream을 가져올 IPAddress입니다.</param>
        public NetworkStream GetNetworkStream(IPAddress ip)
        {
            lock (this)
            {
                TcpClient tcpObj = GetTcpClient(ip);

                if (tcpObj == null)
                    return null;

                try { return tcpObj.GetStream(); }
                catch (InvalidOperationException) { return null; }
            }
        }

        /// <summary>
        /// 특정 IP주소에 대한 TcpClient 가져옵니다. 해당 IP와 연결되지 않았을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="ip">TcpClient를 가져올 IPAddress입니다.</param>
        public TcpClient GetTcpClient(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException();

            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NetServer_Listener));

                try
                {
                    // ip주소에 해당하는 TcpClient를 반환합니다.
                    return m_AcceptedConnection[ip];
                }
                // 지정된 IP주소를 찾을 수 없는 경우 null을 반환합니다.
                catch (KeyNotFoundException)
                { return null; }
            }
        }

        /// <summary>
        /// 연결 요청을 받아들입니다.
        /// </summary>
        private void AcceptingConnection()
        {
            TcpClient conTcpClient = null;
            IPAddress conIpAddress;
            bool filterPass;

            while (IsPortOpened)
            {
                try
                {
                    // 연결된 컴퓨터와 통신할 수 있는 TcpClient를 가져옵니다.
                    conTcpClient = m_Listener.AcceptTcpClient();
                }
                // 연결 수신 도중에 수신이 닫힌 경우, Interrupted SocketException가 발생하므로 스레드를 종료합니다.
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted && !IsPortOpened) { return; }

                lock (this)
                {
                    try
                    {
                        // 만약 포트가 이미 닫혔거나 이 객체가 해제된 경우
                        if (!IsPortOpened || IsDisposed)
                        {
                            // 새로운 연결이 존재하는 경우 닫고 스레드 종료함.
                            if (conTcpClient != null)
                                conTcpClient.Close();
                            return;
                        }

                        // 연결된 컴퓨터의 IP주소를 가져옵니다.
                        conIpAddress = ((IPEndPoint)conTcpClient.Client.RemoteEndPoint).Address;
                        Program.m_Logger.WriteLog("[Listener] Connection Request from " + conIpAddress.ToString());

                        // IP 필터가 설정되어있는 경우 
                        if (IPFilter != null)
                        {
                            try { filterPass = IPFilter(conIpAddress); }
                            catch { filterPass = false; }

                            // 필터에서 차단되거나 오류가 throw된 경우
                            if (!filterPass)
                            {
                                // 현재 연결을 닫고 무시함.
                                conTcpClient.Close();
                                Program.m_Logger.WriteLog("[Listener] Connection filter refuse: " + conIpAddress.ToString());
                                continue;
                            }
                        }

                        // 받아들인 연결 목록에 이미 ip가 존재할 때 다시 연결요청이 들어온 경우
                        if (m_AcceptedConnection.ContainsKey(conIpAddress))
                        {
                            // 이미 연결된 TcpClient 객체를 가져옵니다.
                            TcpClient alreadyCon = m_AcceptedConnection[conIpAddress];

                            // 기존 연결이 존재하는 상태에서 새로 연결 요청이 들어온 경우, 기존 연결을 닫습니다.
                            if (alreadyCon != null)
                            {
                                Program.m_Logger.WriteLog("[Listener] Close existing connection to a new connection: " + conIpAddress.ToString());
                                alreadyCon.Close();
                                m_AcceptedConnection.Remove(conIpAddress);
                            }
                        }

                        // 클라이언트의 응답 요청을 받고 응답 시도
                        if (ResponseToClient(conTcpClient, 5, conIpAddress))
                        {
                            // 새로운 연결을 컬렉션에 추가
                            m_AcceptedConnection.Add(conIpAddress, conTcpClient);

                            // 새로운 연결에 대한 이벤트 발생
                            NewConnection?.Invoke(conIpAddress);
                            Program.m_Logger.WriteLog("[Listener] New connection: " + conIpAddress.ToString());
                        }

                        conTcpClient = null;
                        conIpAddress = null;
                    }
                    catch (Exception ex)
                    {
                        if (conTcpClient != null && conTcpClient.Connected)
                            conTcpClient.Close();
                        Program.m_Logger.WriteLog("[Listener] ERROR while connecting: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 정상적인 연결을 확인하기 위해 클라이언트의 응답 요청에 응답합니다.
        /// </summary>
        /// <param name="timeOutSecond">클라이언트에서 응답 요청이 없을 경우 대기할 최대 초단위 시간입니다.</param>
        private bool ResponseToClient(TcpClient tcpClient, int timeOutSecond, IPAddress ip)
        {
            bool succeed = false; // pingTest가 성공했는지를 나타냅니다.
            Thread sleepThread = Thread.CurrentThread; // receiveThread에서 인터럽트를 발생시킬 수 있도록 현재 스레드를 저장해놓습니다.

            // 서버의 응답을 받는 스레드입니다.
            Thread receiveThread = new Thread(delegate ()
            {
                try
                {
                    // 클라이언트로부터 요청을 받습니다.
                    SendingObj receiveObj = SendingObj.Deserialize(tcpClient.GetStream());
                    Program.m_Logger.WriteLog("[Listener] received send test req: " + ip.ToString());

                    if (receiveObj.SendingType == SendingType.PingToServer)
                    {
                        new SendingObj(SendingType.PingResponseToClient).Serialize(tcpClient.GetStream());
                        Program.m_Logger.WriteLog("[Listener] replied send test: " + ip.ToString());
                        succeed = true;
                    }
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
                Program.m_Logger.WriteLog("[Listener] send test ERROR: " + ip.ToString());
                tcpClient.Close();
                return false;
            }
        }

        /// <summary>
        /// 지정한 포트번호로 서버를 열어 연결 수신을 시작합니다.
        /// </summary>
        /// <param name="portNumber">열 포트번호입니다.</param>
        public void OpenPort(ushort portNumber)
        {
            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NetServer_Listener));

                m_PortNumber = portNumber;
                // 수신 객체 생성 후 수신 시작
                m_Listener = new TcpListener(IPAddress.Any, portNumber);
                m_Listener.Start();

                Program.m_Logger.WriteLog("[Listener] Port is opened");

                // 스레드 객체 생성 후 실행
                m_AcceptingConnectionThread = new Thread(AcceptingConnection);
                m_AcceptingConnectionThread.Start();
            }
        }

        /// <summary>
        /// 포트를 닫아 연결 수신을 종료합니다. 이미 받아들인 연결은 그대로 남습니다.
        /// </summary>
        public void ClosePort()
        {
            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NetServer_Listener));

                // 이미 포트가 닫힌 경우 종료
                if (!IsPortOpened)
                    return;

                m_Listener.Stop();
                Program.m_Logger.WriteLog("[Listener] Port is closed");
                m_Listener = null;
                m_AcceptingConnectionThread = null;
            }
        }

        /// <summary>
        /// 받아들인 모든 연결을 종료합니다. 연결 수신은 그대로 남습니다.
        /// </summary>
        public void CloseAllConnection()
        {
            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NetServer_Listener));

                // 연결 컬랙션을 순회하며 모든 연결을 닫습니다.
                foreach (TcpClient eachCon in m_AcceptedConnection.Values)
                {
                    eachCon.Close();
                }

                // 컬랙션을 초기화합니다.
                m_AcceptedConnection.Clear();
            }
        }

        /// <summary>
        /// 특정 IP에 대한 연결을 종료합니다. 이미 연결이 종료되었거나 연결되지 않았을 경우 아무일도 일어나지 않습니다.
        /// </summary>
        /// <param name="ip">연결을 종료할 IP 주소입니다.</param>
        public void CloseConnection(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException();

            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NetServer_Listener));

                try
                {
                    m_AcceptedConnection[ip].Close();
                    m_AcceptedConnection.Remove(ip);
                }
                // IP주소에 대한 연결을 찾을 수 없는 경우 무시
                catch (KeyNotFoundException) { }
            }
        }

        /// <summary>
        /// 연결 수신을 종료하고 받아들인 모든 연결을 종료하여 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (IsDisposed)
                    return;

                ClosePort();
                CloseAllConnection();
                m_AcceptedConnection = null;
                GC.SuppressFinalize(this);
            }
        }

        ~NetServer_Listener()
        {
            Dispose();
        }
    }
}
