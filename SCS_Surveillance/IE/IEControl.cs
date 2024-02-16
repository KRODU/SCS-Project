using SCS.Common;
using SHDocVw;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using static SCS.Common.URLTidy;

/*  
    2015-2-24 부천대학교 이동원

    startIE() 실행.

    1. 차단 URL 가져오기
    BlockList()
    2. 실행중인 WebBrowser 가져오기
    getWebBrowser()
    3. WebBrowser 확인(차단 URL, IE), 종료
    checkInternetExplorer(), checkURL()
*/

namespace SCS.Surveillance.Monitor
{
    /// <summary>
    /// 특정 URL로의 접속을 차단하는 클래스입니다.
    /// </summary>
    public class IEControl : IDisposable
    {
        /// <summary>
        /// 특정 URL에 접속했을 때 발생합니다.
        /// true가 전달될 경우 해당 URL이 제한되었음을, false일 경우 허가되었음을 의미합니다.
        /// </summary>
        public event Action<bool, string> URLAccessEvent;

        /// <summary>
        /// 무한반복 종료를 위한 변수
        /// </summary>
        private bool m_isRunning = true;

        /// <summary>
        /// 차단된 URL 목록을 가져오거나 설정합니다.
        /// </summary>
        public SyncHashSet<string> BlockList { get; set; } = new SyncHashSet<string>();

        /// <summary>
        /// 메시지 닫기 정의
        /// </summary>
        private const int WM_CLOSE = 0x0010;

        /// Win32 API 종료 메시지 보내함수.
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int uMsg, int wParam, string lParam);

        /// <summary>
        /// 주기적으로 브라우저를 체크하는 스레드입니다.
        /// </summary>
        private Thread m_IE_Thread;

        /// <summary>
        /// 웹브라우져를 가져오는 함수
        /// </summary>
        private WebBrowser[] getWebBrowser()
        {
            ShellWindowsClass wb = new ShellWindowsClass();
            WebBrowser[] wbList = new WebBrowser[wb.Count];

            for (int i = 0; i < wb.Count; i++)
                wbList[i] = (WebBrowser)wb.Item(i);

            return wbList;
        }

        /// <summary>
        /// URL이 제한된 URL인지 확인하는 함수
        /// </summary>
        private void checkURL()
        {
            try
            {
                WebBrowser[] m_wb = getWebBrowser();

                for (int i = 0; i < m_wb.Length; i++)
                {
                    if (m_wb[i] != null)
                    {
                        string tidyUrl = UrlTidy(m_wb[i].LocationURL);

                        if (BlockList.Contains(tidyUrl))
                        {
                            CloseMessage((InternetExplorer)m_wb[i]); // 프로그램 종료
                            URLAccessEvent?.Invoke(true, UrlTidy(tidyUrl)); // 프로그램이 제한될 때 logEvent발생
                            break;
                        }
                        //현재 접속한 URL 제한 URL이 아닐시 이벤트 발생.
                        else
                        {
                            //if (URLAccessEvent != null)
                            //    URLAccessEvent(false, tidyUrl); // 이벤트 발생
                        }
                    }
                }
            }
            catch
            {
                Debug.Fail("checkURL ERROR");
            }
        }

        /// <summary>
        /// 종료메시지 보내는 함수
        /// </summary>
        private void CloseMessage(InternetExplorer ie)
        {
            try
            {
                SendMessage((int)ie.HWND, WM_CLOSE, 0, null);
            }
            catch
            {
                Debug.Fail("CloseMessage ERROR");
            }
        }
        /// <summary>
        /// 클래스 종료를 위한 메서드
        /// </summary>
        public void Dispose()
        {
            m_isRunning = false;
            m_IE_Thread.Interrupt();
            m_IE_Thread = null;
            BlockList = null;
        }
        /// <summary>
        /// startIE 스레드에서 사용될 메서드
        /// </summary>
        private void thread_play()
        {
            try
            {
                while (m_isRunning)
                {
                    checkURL();
                    try { Thread.Sleep(1000); }
                    catch (ThreadInterruptedException) { } // 인터럽트로 인한 오류 발생시 무시
                }
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.ToString());
            }
        }
        /// <summary>
        /// 브라우저 감시를 시작합니다.
        /// </summary>
        public void startIE()
        {
            m_IE_Thread = new Thread(new ThreadStart(thread_play));
            m_IE_Thread.Start();
        }

        /// <summary>
        /// 브라우저 감시를 끝냅니다.
        /// </summary>
        public void stopIE()
        {
            m_isRunning = false;
            m_IE_Thread.Interrupt();
        }
    }
}