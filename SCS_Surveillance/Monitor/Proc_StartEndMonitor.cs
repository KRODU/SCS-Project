using System;
using System.Management;

namespace SCS.Surveillance.Monitor
{
    /// <summary>
    /// 프로세스의 시작과 종료를 감시하는 클래스입니다.
    /// </summary>
    public class Proc_StartEndMonitor : IDisposable
    {
        /// <summary>
        /// 프로세스의 발생 또는 종료 이벤트를 처리하는 대리자입니다.
        /// </summary>
        public delegate void ProcStartEndEvent(string ProcessName, uint ProcessID, uint ParentProcessID);

        /// <summary>
        /// 새로운 프로세스가 나타날 때 발생합니다.
        /// </summary>
        private ProcStartEndEvent ProcStart;

        /// <summary>
        /// 프로세스가 종료될 때 발생합니다.
        /// </summary>
        private ProcStartEndEvent ProcEnd;

        /// <summary>
        /// 프로세스 시작을 감시합니다.
        /// </summary>
        private ManagementEventWatcher m_ProcStartWatch;

        /// <summary>
        /// 프로세스 종료를 감시합니다.
        /// </summary>
        private ManagementEventWatcher m_ProcStopWatch;

        /// <summary>
        /// 프로세스 감시를 시작합니다.
        /// </summary>
        /// <param name="startEvent">프로세스 시작 이벤트가 전달될 대리자입니다.</param>
        /// <param name="endEvent">프로세스 종료 이벤트가 전달될 대리자입니다.</param>
        public void StartMonitor(ProcStartEndEvent startEvent = null, ProcStartEndEvent endEvent = null)
        {
            lock (this)
            {
                if (startEvent != null)
                {
                    ProcStart = startEvent;
                    m_ProcStartWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                    m_ProcStartWatch.EventArrived += new EventArrivedEventHandler(StartWatch_EventArrived);
                    m_ProcStartWatch.Start();
                }

                if (endEvent != null)
                {
                    ProcEnd = endEvent;
                    m_ProcStopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                    m_ProcStopWatch.EventArrived += new EventArrivedEventHandler(StopWatch_EventArrived);
                    m_ProcStopWatch.Start();
                }
            }
        }

        /// <summary>
        /// 감시를 중단합니다.
        /// </summary>
        public void StopMonitor()
        {
            lock (this)
            {
                if (m_ProcStartWatch != null)
                {
                    m_ProcStartWatch.Stop();
                    m_ProcStartWatch.Dispose();
                    m_ProcStartWatch = null;
                }

                if (m_ProcStopWatch != null)
                {
                    m_ProcStopWatch.Stop();
                    m_ProcStopWatch.Dispose();
                    m_ProcStopWatch = null;
                }
            }
        }

        /// <summary>
        /// 프로세스가 발생했을 때 호출됩니다.
        /// </summary>
        private void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var values = e.NewEvent.Properties;
            ProcStart?.Invoke((string)values["ProcessName"].Value, (uint)values["ProcessID"].Value, (uint)values["ParentProcessID"].Value);
        }

        /// <summary>
        /// 프로세스가 종료됐을 때 호출됩니다.
        /// </summary>
        private void StopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var values = e.NewEvent.Properties;
            ProcEnd?.Invoke((string)values["ProcessName"].Value, (uint)values["ProcessID"].Value, (uint)values["ParentProcessID"].Value);
        }

        /// <summary>
        /// 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                StopMonitor();
                GC.SuppressFinalize(this);
            }
        }

        ~Proc_StartEndMonitor()
        {
            Dispose();
        }
    }
}
