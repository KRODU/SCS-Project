using SCS.Common;
using SCS.Net.Client;
using SCS.Net.Common;
using SCS.Surveillance.Monitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Surveillance
{
    /// <summary>
    /// PC 감시 클래스들을 관리합니다.
    /// </summary>
    public class MonitorManager : IDisposable
    {
        /// <summary>
        /// 서버와 연결하는 TcpClient 객체입니다.
        /// </summary>
        private TcpClient m_tcpClient;

        /// <summary>
        /// 서버에 데이터를 전송하기 위한 큐입니다.
        /// </summary>
        private DataQueueSender<SendingObj> m_DataSender;

        /// <summary>
        /// 서버로부터 데이터를 받기위한 객체입니다.
        /// </summary>
        private NetClient_Receiver m_Receiver;

        /// <summary>
        /// 프로세스 제한을 위한 객체입니다.
        /// </summary>
        private Proc_Restriction m_ProcessRestriction;

        /// <summary>
        /// 주기적인 화면 캡쳐를 위한 객체입니다.
        /// </summary>
        private ScreenCapture m_ScreenCapture;

        /// <summary>
        /// 인터넷 제한을 위한 객체입니다.
        /// </summary>
        private IEControl m_IEControl;

        private UsbReceiver usbMoniter;
        private Thread usbMoniter_Thread;

        public MonitorManager(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException();

            m_tcpClient = tcpClient;

            // 데이터 전송 객체 생성
            m_DataSender = new DataQueueSender<SendingObj>();

            // 초기화값을 서버에 요청합니다.
            m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.RestrictionInitReqToServer));

            // 데이터 받을 객체 생성하고 초기화합니다.
            m_Receiver = new NetClient_Receiver();

            usbMoniter = new UsbReceiver();

            m_Receiver.RestrictionInitEvent += M_Receiver_RestrictionInitEvent;
            m_Receiver.SpreadMessageToClientEvent += M_Receiver_SpreadMessageToClientEvent;
            m_Receiver.PasswordCheckEvent += M_Receiver_PasswordCheckEvent;
            m_Receiver.ReceivingDisconnect += M_Receiver_ReceivingException;
            m_Receiver.ExceptionEvent += M_Receiver_ExceptionEvent;
            m_Receiver.StartReceive(m_tcpClient);

            usbMoniter_Thread = new Thread(() => Application.Run(usbMoniter));
            usbMoniter_Thread.Start();
            usbMoniter.newUsbDetect += UsbMoniter_newUsbDetect;
            UsbBlock.BlockUsbMemory();
        }

        private void UsbMoniter_newUsbDetect()
        {
            Program.ProgramTrayIcon.ShowBalloonTip(2000, Application.ProductName, "USB 메모리는 사용할 수 없습니다.",
                ToolTipIcon.Warning);
        }

        /// <summary>
        /// 서버와의 연결이 끊어지면 호출됩니다.
        /// </summary>
        private void M_Receiver_ReceivingException(Exception obj)
        {
            // TODO: 구현
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 데이터 받기에서 오류가 발생한 경우 호출됩니다.
        /// </summary>
        private void M_Receiver_ExceptionEvent(Exception ex)
        {
            if (Program.connectorObj.TcpSocket.Connected && !Program.m_ProgramExit)
            {
                Debug.Fail(ex.Message);
                m_Receiver.StartReceive(Program.connectorObj.TcpSocket);
            }
            else if (!Program.m_ProgramExit)
            {
                MessageBox.Show("");
            }

        }

        /// <summary>
        /// 비밀번호 확인 요청 서버의 응답이 발생하면 호출됩니다.
        /// </summary>
        private void M_Receiver_PasswordCheckEvent(bool obj)
        {
            if (obj)
            {
                MessageBox.Show("감시 프로그램을 종료합니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Program.EndProgram();
            }
            else
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void M_Receiver_SpreadMessageToClientEvent(string obj)
        {
            MessageBox.Show(obj, "서버로부터의 메시지", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 서버로부터 초기화값을 받은 이후 경우에 호출됩니다.
        /// </summary>
        private void M_Receiver_RestrictionInitEvent(RestrictionInitTag obj)
        {
            if (obj.ProcRestrictionSet != null)
            {
                m_ProcessRestriction = new Proc_Restriction();
                m_ProcessRestriction.RestrictionSet = obj.ProcRestrictionSet;
                m_ProcessRestriction.ProcEvent += ProcessRestriction_ProcEvent;
                m_ProcessRestriction.StartRestriction();
            }

            if (obj.URLRestrictionSet != null)
            {
                m_IEControl = new IEControl();
                m_IEControl.BlockList = obj.URLRestrictionSet;
                m_IEControl.URLAccessEvent += IEControl_URLAccessEvent;
                m_IEControl.startIE();
            }

            if (obj.ScreenCaptureInterval > 0)
            {
                m_ScreenCapture = new ScreenCapture();
                m_ScreenCapture.StartCapture(obj.ScreenCaptureInterval, ScreenCaptureEvent);
            }
        }

        /// <summary>
        /// URL로 접속할 경우 호출됩니다.
        /// </summary>
        private void IEControl_URLAccessEvent(bool restriction, string url)
        {
            // 인터넷에 접속한 경우 서버에 알립니다.
            if (restriction)
                m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.URLRestrictionLogToServer, new URLAccessTag(DateTime.Now, url)));
            else
                m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.URLAccessLogToServer, new URLAccessTag(DateTime.Now, url)));
        }

        /// <summary>
        /// 프로세스 모니터링에서 이벤트가 발생한 경우 호출됩니다.
        /// </summary>
        private void ProcessRestriction_ProcEvent(Proc_Restriction.ProgramEventType logT, string hash, string title, uint processID, DateTime startTime, DateTime? endTime)
        {
            switch (logT)
            {
                // 프로그램이 종료된 경우 서버에 정보를 전송합니다.
                case Proc_Restriction.ProgramEventType.End:
                    {
                        m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.ProgramEndLogToServer,
                            new ProgramLogTag(startTime, (DateTime)endTime, hash, title)));
                    }
                    break;
                // 프로그램이 제한된 경우 서버에 정보를 전송합니다.
                case Proc_Restriction.ProgramEventType.Restriction:
                    {
                        m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.ProgramRestrictionLogToServer,
                            new ProgramLogTag(startTime, (DateTime)endTime, hash, title)));
                    }
                    break;
            }
        }

        /// <summary>
        /// 주기적인 화면 캡쳐시에 호출되어 서버에 전송합니다.
        /// </summary>
        private void ScreenCaptureEvent(Bitmap image)
        {
            m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.CapturedScreenToServer, new CapturedScreenTag(DateTime.Now, image)));
        }

        /// <summary>
        /// 보호해제 비밀번호를 확인합니다.
        /// </summary>
        public void PasswordCheck(string password)
        {
            if (password == null)
                throw new ArgumentNullException();

            m_DataSender.EnqueueData(m_tcpClient.GetStream(), new SendingObj(SendingType.PasswordCheckToServer, password));
        }

        /// <summary>
        /// 화면 캡쳐가 시작된 경우, 호출하면 즉시 화면을 캡쳐해 서버에 전송합니다.
        /// </summary>
        public void InstantCapture()
        {
            if (m_ScreenCapture != null)
                m_ScreenCapture.InstantCapture();
        }

        /// <summary>
        /// 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (m_ProcessRestriction != null)
                    m_ProcessRestriction.Dispose();
                if (m_IEControl != null)
                    m_IEControl.Dispose();
                if (m_ScreenCapture != null)
                    m_ScreenCapture.Dispose();
                m_DataSender.Dispose();
                GC.SuppressFinalize(this);
            }
            UsbBlock.AllowUsbMemory();
        }

        ~MonitorManager()
        {
            Dispose();
        }
    }
}
