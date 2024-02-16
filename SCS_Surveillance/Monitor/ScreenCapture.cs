using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SCS.Surveillance.Monitor
{
    /// <summary>
    /// 일정 주기로 화면을 캡쳐합니다.
    /// </summary>
    public class ScreenCapture : IDisposable
    {
        /// <summary>
        /// 화면이 캡쳐되었을 때 호출됩니다.
        /// </summary>
        public delegate void ScreenCapturedEvent(Bitmap image);

        /// <summary>
        /// 화면이 캡쳐되었을 때 호출될 메소드를 지장합니다.
        /// </summary>
        private ScreenCapturedEvent m_CaptureEventHandler;

        /// <summary>
        /// 주기적인 캡쳐를 실행하는 스레드입니다.
        /// </summary>
        private Thread m_CaptureThread;

        /// <summary>
        /// 현재 화면 캡쳐가 진행중인지를 반환합니다.
        /// </summary>
        public bool Capture { get { lock (this) return captureInterval > 0; } }

        /// <summary>
        /// 초단위 캡쳐 주기입니다.
        /// </summary>
        public int captureInterval { get; private set; } = -1;

        public ScreenCapture()
        {
            m_CaptureThread = new Thread(ThreadCapture);
        }

        /// <summary>
        /// 스레드에서 지정된 주기만큼 캡쳐를 반복합니다.
        /// </summary>
        private void ThreadCapture()
        {
            Debug.Assert(captureInterval > 0);

            int curWait;

            // 캡쳐가 진행중인 동안 반복합니다.
            while (Capture)
            {
                // 지정된 간격만큼 대기합니다. 중간에 캡쳐가 중단된 경우 for문을 빠져나갑니다.
                for (curWait = 0; curWait < captureInterval && Capture; curWait++)
                {
                    Thread.Sleep(1000);
                }

                lock (this)
                {
                    // 중간에 캡쳐가 중단된 경우에 스레드를 종료합니다.
                    if (!Capture)
                        return;

                    // 화면을 캡쳐해 대리자에 보냅니다.
                    m_CaptureEventHandler(ScreenShot());
                }
            }
        }

        /// <summary>
        /// 호출하면 즉시 화면을 캡쳐해 대리자를 호출합니다. 화면 캡쳐가 시작되지 않은 경우 무시됩니다.
        /// </summary>
        public void InstantCapture()
        {
            if (Capture)
                m_CaptureEventHandler(ScreenShot());
        }

        /// <summary>
        /// 화면을 캡쳐합니다.
        /// </summary>
        private Bitmap ScreenShot()
        {
            Size sz = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap bmp = new Bitmap(sz.Width, sz.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(0, 0, 0, 0, sz);
            return bmp;
        }

        /// <summary>
        /// 화면 캡쳐를 시작합니다. 이미 시작한 경우 무시됩니다.
        /// </summary>
        /// <param name="captureInterval">캡쳐할 초단위 간격입니다. 반드시 0보다 커야합니다.</param>
        /// <param name="capturedEvent">화면이 캡쳐될 때마다 호출될 대리자입니다. null일 수 없습니다.</param>
        public void StartCapture(int captureInterval, ScreenCapturedEvent capturedEvent)
        {
            if (captureInterval <= 0)
                throw new ArgumentOutOfRangeException();

            if (capturedEvent == null)
                throw new ArgumentNullException();

            lock (this)
            {
                // 캡쳐가 이미 시작된 경우 무시
                if (Capture)
                    return;

                // 캡쳐 간격을 설정하고 캡쳐를 시작합니다.
                this.captureInterval = captureInterval;
                m_CaptureEventHandler = capturedEvent;
                m_CaptureThread.Start();
            }
        }

        /// <summary>
        /// 화면 캡쳐를 중단합니다.
        /// </summary>
        public void StopCapture()
        {
            lock (this)
            {
                // 캡쳐가 진행중이지 않은 경우 무시
                if (!Capture)
                    return;
                captureInterval = -1;
                m_CaptureEventHandler = null;
            }
        }

        /// <summary>
        /// 이 객체를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            StopCapture();
            m_CaptureThread = null;
            GC.SuppressFinalize(this);
        }

        ~ScreenCapture()
        {
            Dispose();
        }
    }
}
