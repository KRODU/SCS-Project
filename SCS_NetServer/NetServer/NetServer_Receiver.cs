using MySql.Data.MySqlClient;
using SCS.Common;
using SCS.Net.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using static SCS.Net.Server.DBConnector;

namespace SCS.Net.Server
{
    /// <summary>
    /// 데이터를 받아들여 처리하는 클래스입니다.
    /// </summary>
    public class NetServer_Receiver : IDisposable
    {
        private struct IPAndType
        {
            public IPAddress ip;
            public SendingType type;

            public IPAndType(IPAddress ip, SendingType type)
            {
                this.ip = ip;
                this.type = type;
            }
        }

        /// <summary>
        /// 예외 발생시 호출되는 이벤트입니다.
        /// </summary>
        public event ExceptionHandler ExceptionEvent;

        /// <summary>
        /// <see cref="NetServer_Listener"/> 객체입니다.
        /// </summary>
        private NetServer_Listener m_listenerObj;

        /// <summary>
        /// 클라이언트의 연결이 끊긴 경우 호출됩니다.
        /// </summary>
        public event Action<IPAddress> DisconnectedEvent;

        /// <summary>
        /// 받아들인 연결에 대한 스레드를 저장하는 컬렉션입니다.
        /// </summary>
        private Dictionary<TcpClient, Thread> m_ReceiverThread = new Dictionary<TcpClient, Thread>();

        /// <summary>
        /// SFTP와 연결해 이미지를 업로드합니다.
        /// </summary>
        FTPImageSender m_ftpCon = new FTPImageSender();

        public NetServer_Receiver(NetServer_Listener listenerObj)
        {
            if (listenerObj == null || listenerObj.IsDisposed)
                throw new ArgumentNullException();

            m_listenerObj = listenerObj;
            m_listenerObj.NewConnection += NewConnection;

            m_ftpCon.SetServerInfo("52.196.178.171", "ubuntu", "scs.pem", "/var/www/html/assets/img/userScreen");
        }

        /// <summary>
        /// 새로운 연결이 발생했을 때 이벤트로 호출됩니다.
        /// </summary>
        private void NewConnection(IPAddress ip)
        {
            TcpClient tcpObj = m_listenerObj.GetTcpClient(ip);

            if (tcpObj == null)
                return;

            lock (this)
            {
                if (m_ReceiverThread.ContainsKey(tcpObj))
                    return;

                // 스레드를 만들고 등록합니다.
                Thread receiveThread = new Thread(ThreadingCode);
                m_ReceiverThread.Add(tcpObj, receiveThread);

                // 스레드에 파라미터 전달
                receiveThread.Start(new object[] { tcpObj, ip });
            }
        }

        /// <summary>
        /// 스레드에서 실행되는 코드입니다.
        /// </summary>
        private void ThreadingCode(object obj)
        {
            object[] param = (object[])obj;
            TcpClient tcpObj = (TcpClient)param[0];
            IPAddress ip = (IPAddress)param[1];
            param = null;

            SendingObj receivedData;

            // 연결이 끊긴 이후의 처리를 지정합니다.
            Action disconnectedProc = () =>
            {
                lock (this)
                {
                    Program.m_Logger.WriteLog("[Receiver] Disconnected To " + ip.ToString());
                    m_ReceiverThread.Remove(tcpObj);
                    CapturedScreenSave(Properties.Resources.shutdown, ip);
                    DisconnectedEvent?.Invoke(ip);
                }
            };

            while (tcpObj.Connected)
            {
                try
                {
                    receivedData = SendingObj.Deserialize(tcpObj.GetStream());
                }
                catch (IOException) { disconnectedProc(); return; }
                catch (SerializationException) { disconnectedProc(); return; }

                // 예외가 전송되어 들어온 경우
                if (receivedData.Except != null)
                {
                    ExceptionEvent?.Invoke(receivedData.Except);
                    continue;
                }

                switch (receivedData.SendingType)
                {
                    // 응답 요청에 답합니다.
                    case SendingType.PingToServer:
                        Program.m_Logger.WriteLog("[Receiver] PingToServer Received from" + ip.ToString());
                        Program.m_QueueSender.EnqueueData(tcpObj.GetStream(), new SendingObj(SendingType.PingResponseToClient), SendDataResult, new IPAndType(ip, SendingType.PingResponseToClient));
                        break;

                    // 초기화 데이터 전송 요청이 있는 경우 DB에서 데이터를 꺼내 전송합니다.
                    case SendingType.RestrictionInitReqToServer:
                        Program.m_Logger.WriteLog("[Receiver] RestrictionInitReqToServer Received from " + ip.ToString());
                        // 데이터를 전송합니다.
                        try { Program.m_QueueSender.EnqueueData(tcpObj.GetStream(), new SendingObj(SendingType.RestrictionInitDataToClinet, GetInitData(ip)), SendDataResult, new IPAndType(ip, SendingType.RestrictionInitDataToClinet)); }
                        // 오류가 발생한 경우 오류 발생 사실을 클라이언트에 알립니다.
                        catch (Exception ex)
                        {
                            Program.m_Logger.WriteLog("[Receiver] ERROR while GetInitData(): " + ex.Message);
                            Program.m_QueueSender.EnqueueData(tcpObj.GetStream(), new SendingObj(SendingType.RestrictionInitDataToClinet, exception: ex), SendDataResult, new IPAndType(ip, SendingType.RestrictionInitDataToClinet));
                        }
                        break;

                    // 클라이언트로부터 캡쳐 화면이 전송되어온 경우
                    case SendingType.CapturedScreenToServer:
                        Program.m_Logger.WriteLog("[Receiver] CapturedScreenToServer Received from " + ip.ToString());
                        CapturedScreenSave(((CapturedScreenTag)receivedData.TagData).CapturedImage, ip);
                        break;

                    // 보호 해제 비밀번호 확인 요청이 들어온 경우
                    case SendingType.PasswordCheckToServer:
                        {
                            Program.m_Logger.WriteLog("[Receiver] PasswordCheckToServer Received from " + ip.ToString());
                            var rows = ExecuteQuery("SELECT ProtectPassword FROM DepCode WHERE DepCode_Num=" + GetDepCode(ip));
                            bool check;

                            if (rows.Count != 0)
                            {
                                if ((string)receivedData.TagData == (string)rows[0][0])
                                    check = true;
                                else
                                    check = false;
                            }
                            else
                            {
                                Program.m_Logger.WriteLog("[Receiver] userCode not exist: PasswordCheckToServer, " + ip.ToString());
                                check = true;
                            }

                            Program.m_QueueSender.EnqueueData(tcpObj.GetStream(), new SendingObj(SendingType.PasswordResponseToClient, check), SendDataResult, new IPAndType(ip, SendingType.PasswordResponseToClient));
                        }
                        break;

                    // 프로그램 종료 기록
                    case SendingType.ProgramEndLogToServer:
                        {
                            Program.m_Logger.WriteLog("[Receiver] ProgramEndLogToServer Received from " + ip.ToString());
                            ProgramLogTag logTag = (ProgramLogTag)receivedData.TagData;
                            int appCode = GetAppCode(logTag.HashValue);

                            if (appCode != -1)
                            {
                                MySqlCommand comm = new MySqlCommand();
                                comm.CommandText = "INSERT INTO AppUseLog (AppCode_Name, User_Num, AppUseLog_StartTime, AppUseLog_EndTime) values (@appCode, @userCode, @startTime, @endTime);";
                                comm.Parameters.AddWithValue("@appCode", appCode);
                                comm.Parameters.AddWithValue("@userCode", GetUserCode(ip));
                                comm.Parameters.AddWithValue("@startTime", logTag.StartTime);
                                comm.Parameters.AddWithValue("@endTime", logTag.EndTime);
                                ExecuteNonQuery(comm);
                            }
                        }
                        break;

                    // 프로그램 제한 기록
                    case SendingType.ProgramRestrictionLogToServer:
                        {
                            Program.m_Logger.WriteLog("[Receiver] ProgramRestrictionLogToServer Received from " + ip.ToString());
                            ProgramLogTag logTag = (ProgramLogTag)receivedData.TagData;
                            int appCode = GetAppCode(logTag.HashValue);

                            if (appCode != -1)
                                ExecuteNonQuery("INSERT INTO RestraintAppLog (AppCode_Num, RestraintAppLog_Time, User_Num) values ('" + appCode +
                                    "','" + DateTimeToDB(logTag.StartTime) + "', '" + GetUserCode(ip) + "') ");
                        }
                        break;

                    // 인터넷 URL 접속 기록
                    case SendingType.URLAccessLogToServer:
                        {
                            Program.m_Logger.WriteLog("[Receiver] URLAccessLogToServer Received from " + ip.ToString());
                            URLAccessTag uat = (URLAccessTag)receivedData.TagData;
                            int appCode = GetAppCode(uat.Url);

                            if (appCode != -1)
                                ExecuteNonQuery("INSERT INTO AppUseLog (AppCode_Name, User_Num, AppUseLog_StartTime) values ('" + appCode +
                                    "','" + GetUserCode(ip) + "', '" + DateTimeToDB(uat.AccessTime) + "') ");
                        }
                        break;

                    // 인터넷 URL 제한 기록
                    case SendingType.URLRestrictionLogToServer:
                        {
                            Program.m_Logger.WriteLog("[Receiver] URLRestrictionLogToServer Received from " + ip.ToString());
                            URLAccessTag uat = (URLAccessTag)receivedData.TagData;
                            int appCode = GetAppCode(uat.Url);

                            if (appCode != -1)
                            {
                                MySqlCommand comm = new MySqlCommand();
                                comm.CommandText = "INSERT INTO RestraintAppLog (AppCode_Num, RestraintAppLog_Time, User_Num) values (@appCode, @accessTime, @userCode);";
                                comm.Parameters.AddWithValue("@appCode", appCode);
                                comm.Parameters.AddWithValue("@accessTime", uat.AccessTime);
                                comm.Parameters.AddWithValue("@userCode", GetUserCode(ip));
                                ExecuteNonQuery(comm);
                            }
                        }
                        break;

                    default:
                        Program.m_Logger.WriteLog("ERROR: Not defined message received from " + ip.ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// 데이터 전송 이후 결과를 여기로 받아옵니다.
        /// </summary>
        private void SendDataResult(bool succeed, object tag, Exception exp)
        {
            if (exp == null)
            {
                IPAndType ipType = (IPAndType)tag;
                Program.m_Logger.WriteLog("[Receiver] " + (succeed ? "succeed " : "failed ") + ipType.type.ToString() + " type queue data send " + " to " + ipType.ip);
            }
            // 데이터 전송에서 오류가 발생한 경우
            else
            {
                Program.m_Logger.WriteLog("[Receiver] ERROR while data queue send: " + exp.Message);
            }
        }

        /// <summary>
        /// 프로그램 제한, 인터넷 제한, 화면 캡쳐 간격에 대한 초기화 데이터를 가져옵니다.
        /// </summary>
        private RestrictionInitTag GetInitData(IPAddress ip)
        {
            SyncHashSet<string> RestrictionProgram = new SyncHashSet<string>();
            SyncHashSet<string> URLRestriction = new SyncHashSet<string>();

            int captureInteval; // 화면 캡쳐 간격입니다.
            int depCode = GetDepCode(ip); // 부서 코드번호 입니다.

            // 프로그램 제한 목록을 가져옵니다.
            var ProgramRows = ExecuteQuery("SELECT AppCode_Hash FROM AppCode a INNER JOIN RestraintApp r ON a.AppCode_Num = r.RestraintApp_Num WHERE a.AppCode_Division = 1 AND DepCode_Num = " + depCode);

            foreach (DataRow eachRow in ProgramRows)
            {
                RestrictionProgram.Add((string)eachRow[0]);
            }


            // 인터넷 제한 목록을 가져옵니다.
            var URLRows = ExecuteQuery("SELECT AppCode_Hash FROM AppCode a INNER JOIN RestraintApp r ON a.AppCode_Num = r.RestraintApp_Num WHERE a.AppCode_Division = 2 AND DepCode_Num = " + depCode);

            foreach (DataRow eachRow in URLRows)
            {
                URLRestriction.Add((string)eachRow[0]);
            }


            // 화면 캡쳐 간격을 가져옵니다.
            var captureRow = ExecuteQuery("SELECT CaptureInterval FROM DepCode WHERE DepCode_Num = " + depCode);

            if (captureRow.Count == 0)
                captureInteval = -1;
            else
                captureInteval = (int)captureRow[0][0];

            return new RestrictionInitTag(RestrictionProgram, URLRestriction, captureInteval);
        }

        /// <summary>
        /// 클라이언트로부터 스크린샷을 받은 경우 이를 서버에 저장합니다.
        /// </summary>
        private void CapturedScreenSave(Image img, IPAddress ip)
        {
            int userCode = GetUserCode(ip);

            if (userCode == -1)
            {
                Program.m_Logger.WriteLog("[Receiver] userCode not exist: CapturedScreenSave(), " + ip.ToString());
                return;
            }

            m_ftpCon.UploadImage(img, userCode.ToString() + ".jpg");
        }

        public void Dispose()
        {
            m_ftpCon.Dispose();
        }
    }
}
