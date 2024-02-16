using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;


namespace SCS.Common
{
    /// <summary>
    /// 보낼 데이터를 큐에 저장해 순차적으로 씁니다.
    /// </summary>
    public class DataQueueSender<T> : IDisposable
    {
        /// <summary>
        /// 데이터 쓰기 성공 여부와 태그, 오류가 있을 경우 오류를 반환합니다.
        /// </summary>
        public delegate void QueueSenderResult(bool succeed, object tag, Exception exp);

        /// <summary>
        /// 스트림과 데이터를 같이 넣어 큐에 저장합니다.
        /// </summary>
        private struct QueueData
        {
            public Stream Stream { get; }
            public T Data { get; }
            public object Tag { get; }
            public QueueSenderResult ResultRet { get; }

            public QueueData(Stream stream, T data, QueueSenderResult resultRet, object tag)
            {
                Stream = stream;
                Data = data;
                Tag = tag;
                ResultRet = resultRet;
            }
        }

        /// <summary>
        /// 데이터를 쓸 스레드입니다.
        /// </summary>
        private Thread m_SendingThread;

        /// <summary>
        /// 객체 직렬화에 사용되는 BinaryFormatter 객체입니다.
        /// </summary>
        private BinaryFormatter m_bf = new BinaryFormatter();

        /// <summary>
        /// 쓸 데이터를 저장하는 큐입니다.
        /// </summary>
        private ConcurrentQueue<QueueData> m_DataQueue = new ConcurrentQueue<QueueData>();

        /// <summary>
        /// 재시도 간격 필드입니다.
        /// </summary>
        private int m_RetryInterval;

        /// <summary>
        /// ms 단위의 재시도 간격을 가져오거나 설정합니다.
        /// </summary>
        public int RetryInterval
        {
            get { return m_RetryInterval; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                m_RetryInterval = value;
            }
        }

        /// <summary>
        /// 재시도 횟수 필드입니다.
        /// </summary>
        private int m_RetryCount;

        /// <summary>
        /// 데이터 쓰기 실패시 재시도 횟수를 가져오거나 설정합니다.
        /// </summary>
        public int RetryCount
        {
            get { return m_RetryCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                m_RetryCount = value;
            }
        }

        /// <summary>
        /// 이 객체가 해제되었는지를 나타냅니다.
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// 사용할 Steam을 지정해 객체를 생성합니다.
        /// </summary>
        /// <param name="retryCount">쓰기 실패시 재시도 횟수입니다.</param>
        /// <param name="retryInterval">쓰기 실패 후 재시도할 경우 재시도까지 대기할 ms단위의 시간입니다.</param>
        public DataQueueSender(int retryCount = 5, int retryInterval = 100)
        {
            lock (this)
            {
                RetryCount = retryCount;
                RetryInterval = retryInterval;
                m_SendingThread = new Thread(ThreadDataSend);
                m_SendingThread.Start();
            }
        }

        /// <summary>
        /// 지정된 스트림으로 쓸 데이터를 큐에 등록합니다.
        /// </summary>
        /// <param name="stream">데이터를 쓸 스트림입니다.</param>
        /// <param name="obj">쓸 데이터입니다.</param>
        public void EnqueueData(Stream stream, T obj)
        {
            EnqueueData(stream, obj, null, null);
        }

        /// <summary>
        /// 지정된 스트림으로 쓸 데이터를 큐에 등록합니다.
        /// </summary>
        /// <param name="stream">데이터를 쓸 스트림입니다.</param>
        /// <param name="obj">쓸 데이터입니다.</param>
        /// <param name="resultRet">데이터 쓰기 성공 또는 실패 여부가 전달될 대리자입니다.</param>
        /// <param name="tag">대리자에 함께 전달될 태그입니다.</param>
        public void EnqueueData(Stream stream, T obj, QueueSenderResult resultRet, object tag)
        {
            if (stream == null || obj == null)
                throw new ArgumentNullException();
            if (!stream.CanWrite)
                throw new ArgumentException();

            lock (this)
            {
                // 객체가 해제된 이후에 쓰기 요청이 들어온경우 예외처리
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(DataQueueSender<T>));

                // 큐에 등록합니다.
                m_DataQueue.Enqueue(new QueueData(stream, obj, resultRet, tag));

                // 스레드에 인터럽트를 발생시킵니다.
                m_SendingThread.Interrupt();
            }
        }

        /// <summary>
        /// 스레드에서 실행되며, 큐의 데이터를 씁니다.
        /// </summary>
        private void ThreadDataSend()
        {
            QueueData queueData = new QueueData(); //큐에서 꺼내온 데이터입니다.
            bool prevError = false; // 이전에 데이터 쓰기에서 오류가 발생해 재시도해야하는지 여부를 나타냅니다.
            int curRetryCount; // 현재까지의 재시도 횟수입니다.
            Exception lastEx = null; // 쓰기 실패시 실패 이유를 전달하기 위해 오류 객체를 저장해놓습니다.

            // 무한 반복합니다.
            while (true)
            {
                // 큐가 빌때까지 반복합니다. 만약 중간에 객체가 해제된 경우라도 큐가 빌 때까지 쓰기는 계속됩니다.
                while (m_DataQueue.TryDequeue(out queueData))
                {
                    try
                    {
                        // 데이터 쓰기
                        Serialize(queueData.Stream, queueData.Data);
                        prevError = false;
                        lastEx = null;
                        NotifyResult(queueData.ResultRet, true, queueData.Tag);
                    }
                    catch (SerializationException szExp)
                    {
                        Debug.Fail("직렬화 오류");
                        // 직렬화 오류인 경우엔 재시도하지 않음.
                        prevError = false;
                        lastEx = null;
                        NotifyResult(queueData.ResultRet, false, queueData.Tag, szExp); // 직렬화 오류 발생을 알립니다.
                    }
                    catch (Exception ex1)
                    {
                        Debug.Fail("데이터 전송 오류");
                        // 오류가 발생한 경우 오류 플래그를 설정합니다.
                        lastEx = ex1;
                        prevError = true;
                        Thread.Sleep(RetryInterval);

                        // 오류가 발생한 경우 재시도 횟수 내에서 성공할 때까지 반복합니다.----------
                        for (curRetryCount = 0; prevError; curRetryCount++)
                        {
                            // 아직 재시도 횟수가 끝나지 않은 경우
                            if (curRetryCount < RetryCount)
                            {
                                try
                                {
                                    Serialize(queueData.Stream, queueData.Data);
                                    prevError = false;
                                    lastEx = null;
                                    NotifyResult(queueData.ResultRet, true, queueData.Tag);
                                }
                                catch (Exception ex2)
                                {
                                    lastEx = ex2;
                                    Thread.Sleep(RetryInterval);
                                }
                            }
                            // 재시도 횟수가 끝난 경우
                            else
                            {
                                NotifyResult(queueData.ResultRet, false, queueData.Tag, lastEx);
                                prevError = false;
                                lastEx = null;
                            }
                        }
                        // 오류 처리 for문 끝 --------------------------------------------------------
                    }
                }

                try
                {
                    lock (this)
                    {
                        // 이 객체가 해제되었고 쓸 객체도 남아있지 않은 경우 스레드를 종료합니다.
                        if (m_DataQueue.IsEmpty && IsDisposed)
                            return;
                    }
                }
                catch (ThreadInterruptedException) { }

                // 큐가 비어있는 경우 인터럽트 발생시까지 대기합니다.
                if (m_DataQueue.IsEmpty)
                {
                    try { Thread.Sleep(Timeout.Infinite); }
                    // 인터럽트시 발생하는 예외는 무시합니다.
                    catch (ThreadInterruptedException) { }
                }
            }
        }

        /// <summary>
        /// 객체를 직렬화해 씁니다.
        /// </summary>
        private void Serialize(Stream stream, T obj)
        {
            m_bf.Serialize(stream, obj); // 직렬화합니다.
        }

        /// <summary>
        /// 지정된 대리자에 데이터 쓰기 성공 또는 실패를 알립니다.
        /// </summary>
        private void NotifyResult(QueueSenderResult resultRet, bool succeed, object tag, Exception ex = null)
        {
            if (resultRet != null)
            {
                new Thread(() => resultRet(succeed, tag, ex)).Start();
            }
        }

        /// <summary>
        /// 큐에 남은 데이터를 다 쓴 이후 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                IsDisposed = true;
                m_SendingThread.Interrupt();
                GC.SuppressFinalize(this);
            }
        }

        ~DataQueueSender()
        {
            Dispose();
        }
    }
}
