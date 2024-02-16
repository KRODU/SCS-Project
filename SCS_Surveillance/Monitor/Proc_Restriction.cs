using SCS.Common;
using SCS.Net.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace SCS.Surveillance.Monitor
{
    /// <summary>
    /// 새로운 프로세서를 감시하며, 제한된 프로그램인 경우 종료하는 클래스입니다.
    /// </summary>
    public class Proc_Restriction : IDisposable
    {
        /// <summary>
        /// 프로세스를 감시하는 객체입니다.
        /// </summary>
        private Proc_StartEndMonitor m_moniter = new Proc_StartEndMonitor();

        /// <summary>
        /// 제한할 프로그램의 해시코드 컬렉션 필드입니다.
        /// </summary>
        private SyncHashSet<string> m_RestrictionSet = new SyncHashSet<string>();

        /// <summary>
        /// 제한할 프로그램의 해시코드 컬렉션을 반환합니다.
        /// </summary>
        public SyncHashSet<string> RestrictionSet
        {
            get { return m_RestrictionSet; }
            set
            {
                lock (m_RestrictionSet)
                {
                    if (value == null)
                        throw new ArgumentNullException();
                    m_RestrictionSet = value;
                }
            }
        }

        /// <summary>
        /// 프로세스 시작, 종료, 재한 이벤트 발생시 호출됩니다.
        /// </summary>
        public delegate void ProcDel(ProgramEventType logT, string hash, string title, uint processID, DateTime startTime, DateTime? EndTime);

        /// <summary>
        /// 프로세스 관리자 이벤트 유형입니다.
        /// </summary>
        public enum ProgramEventType
        {
            /// <summary>
            /// 프로세스가 시작되었습니다.
            /// </summary>
            Start,
            /// <summary>
            /// 프로세스가 종료되었습니다.
            /// </summary>
            End,
            /// <summary>
            /// 프로세스가 시작되었으나 제한 프로그램이므로 종료되었습니다.
            /// </summary>
            Restriction
        }

        /// <summary>
        /// 프로세스 시작, 종료, 재한 이벤트 발생시 발생합니다.
        /// </summary>
        public event ProcDel ProcEvent;

        /// <summary>
        /// 프로세스의 해시값 및 시작 시간을 저장해놓습니다.
        /// </summary>
        private Dictionary<uint, ProcessData> m_ProcDataCache = new Dictionary<uint, ProcessData>();

        private struct ProcessData
        {
            public string Hash;

            public DateTime StartTime;

            public ProcessData(string hash, DateTime startTime)
            {
                Hash = hash;
                StartTime = startTime;
            }
        }

        /// <summary>
        /// 프로그램 실행 제한을 시작합니다.
        /// </summary>
        public void StartRestriction()
        {
            lock (m_RestrictionSet)
            {
                m_moniter.StartMonitor(StartProc, EndProc);

                // 현재 실행중인 프로세스들을 체크합니다.
                Process[] procArr = Process.GetProcesses();

                foreach (Process eachProc in procArr)
                {
                    CheckProc((uint)eachProc.Id, eachProc.ProcessName);
                }
            }
        }

        /// <summary>
        /// 프로그램 실행 제한을 종료합니다.
        /// </summary>
        public void EndRestriction()
        {
            lock (m_RestrictionSet)
            {
                m_moniter.StopMonitor();
                m_ProcDataCache.Clear();
            }
        }

        /// <summary>
        /// 프로세스가 새로 시작된 경우 호출됩니다.
        /// </summary>
        private void StartProc(string processName, uint processID, uint parentProcessID)
        {
            lock (m_RestrictionSet)
            {
                CheckProc(processID, processName);
            }
        }

        /// <summary>
        /// 프로세스가 종료된 경우 호출됩니다.
        /// </summary>
        private void EndProc(string processName, uint processID, uint parentProcessID)
        {
            lock (m_RestrictionSet)
            {
                string hash;

                if (m_ProcDataCache.ContainsKey(processID))
                {
                    hash = m_ProcDataCache[processID].Hash;
                    // 프로그램 종료 사실을 알림.
                    // 해당 프로그램의 해시값을 확인할 수 없거나, 차단된 프로세스인 경우엔 종료 사실을 알리지 않음.

                    if (ProcEvent != null && !RestrictionSet.Contains(hash))
                    {
                        // 캐시사전에서 값을 가져와서 이벤트 호출
                        ProcEvent(ProgramEventType.End, hash, processName, processID, m_ProcDataCache[processID].StartTime, DateTime.Now);
                    }
                }

                // 캐시사전에서 값 제거
                m_ProcDataCache.Remove(processID);
            }
        }

        /// <summary>
        /// 프로세스 객체를 반환합니다. 오류가 발생한 경우 null이 반환됩니다.
        /// </summary>
        private Process GetProcess(uint processID)
        {
            try
            {
                return Process.GetProcessById((int)processID);
            }
            catch { return null; }
        }

        /// <summary>
        /// 지정된 프로세스를 검사해 제한된 프로그램인 경우 종료합니다. 자식 프로세스도 모두 종료됩니다.
        /// </summary>
        /// <returns>프로세스가 종료된 경우 true, 허가된 경우 false를 반환합니다.</returns>
        private void CheckProc(uint processID, string name)
        {
            lock (m_RestrictionSet)
            {
                Process thisProc = GetProcess(processID);
                string hash;

                try { hash = FileHash.GetFileSHA256Str(thisProc.MainModule.FileName); }// 확인중인 프로그램의 해시값입니다.
                // 오류가 발생한 경우엔 무시하고 종료합니다.
                catch { return; }


                // 캐시사전에 이미 추가된 경우 삭제하고 추가함.
                if (m_ProcDataCache.ContainsKey(processID))
                    m_ProcDataCache.Remove(processID);
                m_ProcDataCache.Add(processID, new ProcessData(hash, thisProc.StartTime));

                // 제한된 프로그램인 경우
                if (m_RestrictionSet.Contains(hash))
                {
                    // 자식 프로세스 종료
                    KillAllProcessesSpawnedBy(processID);

                    // 즉시 종료시킴
                    thisProc.Kill();

                    // 프로그램 제한 사실을 알림.
                    if (ProcEvent != null)
                    {
                        DateTime startTime = m_ProcDataCache[processID].StartTime;
                        ProcEvent(ProgramEventType.Restriction, hash, name, processID, startTime, startTime);
                    }
                }
                else
                {
                    // 프로그램 실행 사실을 알림.
                    ProcEvent?.Invoke(ProgramEventType.Start, hash, name, processID, m_ProcDataCache[processID].StartTime, null);
                }
            }
        }

        /// <summary>
        /// 자식 프로세스를 모두 종료합니다.
        /// </summary>
        private void KillAllProcessesSpawnedBy(uint parentProcessId)
        {
            // NOTE: Process Ids are reused!
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + parentProcessId);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    uint childProcessId = (uint)item["ProcessId"];
                    if ((int)childProcessId != Process.GetCurrentProcess().Id)
                    {
                        KillAllProcessesSpawnedBy(childProcessId);

                        Process childProcess = Process.GetProcessById((int)childProcessId);
                        childProcess.Kill();
                    }
                }
            }
        }

        /// <summary>
        /// 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            lock (m_RestrictionSet)
            {
                EndRestriction();
                m_moniter.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        ~Proc_Restriction()
        {
            Dispose();
        }
    }
}
