using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace SCS.Net.Server
{
    /// <summary>
    /// 서버의 활동에 대한 로그를 기록합니다.
    /// </summary>
    public class Logger : IDisposable
    {
        /// <summary>
        /// 로그가 기록되는 이벤트 발생시 호출됩니다.
        /// </summary>
        public event Action<string> WriteLogEvent;

        /// <summary>
        /// 로그기록 목록 입니다.
        /// </summary>
        private List<string> LogList = new List<string>();

        /// <summary>
        /// 로그기록 배열을 가져옵니다.
        /// </summary>
        public string[] Log
        {
            get { lock (LogList) { return LogList.ToArray(); } }
        }


        /// <summary>
        /// 로그는 먼저 이곳에 기록된 이후 <see cref="LogList"/>에 쓰여집니다.
        /// </summary>
        private ConcurrentQueue<string> m_LogBuffer = new ConcurrentQueue<string>();

        /// <summary>
        /// <see cref="m_LogBuffer"/>의 로그를 버퍼에 쓰는 스레드입니다.
        /// </summary>
        private Thread m_BufferToListThread;

        /// <summary>
        /// 리스트를 통해 로그를 저장할 최대 개수입니다.
        /// </summary>
        private int maxLogCount = 100;

        private bool m_isRunning = true;

        public Logger()
        {
            m_BufferToListThread = new Thread(BufferToList);
            m_BufferToListThread.Start();
        }

        /// <summary>
        /// <see cref="m_BufferToListThread"/> 스레드에서 실행되는 메서드입니다.
        /// </summary>
        private void BufferToList()
        {
            string eachLog;

            while (m_isRunning)
            {
                while (m_LogBuffer.TryDequeue(out eachLog))
                {
                    WriteLogEvent?.Invoke(eachLog); // 로그 기록 이벤트 발생

                    lock (LogList)
                    {
                        LogList.Add(eachLog);
                        if (LogList.Count > maxLogCount)
                            cutLog();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 로그를 기록합니다.
        /// </summary>
        public void WriteLog(string message)
        {
            m_LogBuffer.Enqueue("[" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + "] " + message);
        }

        /// <summary>
        /// 가장 오래된 로그 하나를 리스트에서 삭제합니다.
        /// </summary>
        private void cutLog()
        {
            lock (LogList)
            {
                if (LogList.Count == 0)
                    return;

                LogList.RemoveAt(0);
            }
        }

        public void Dispose()
        {
            m_isRunning = false;
        }
    }
}
