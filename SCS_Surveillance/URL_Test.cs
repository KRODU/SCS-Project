using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using SHDocVw;
using System.Diagnostics;
using System.Runtime.InteropServices;
using mshtml;
using NetFwTypeLib;
//using DW_Test;

namespace SCS.Surveillance
{
    public partial class URL_Test : Form
    {
        int hWnd;
        public const int WM_CLOSE = 0x0010; //닫기

        //win32 API
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowCaption);
        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int hWnd1, int hWnd2, string lpsz1, string lpsz2);
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int uMsg, int wParam, string lParam);

        public URL_Test()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 익스플로러 정보 가져오기.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //실행중인 ie URL 가져오기
            foreach (InternetExplorer ie in new ShellWindowsClass())
            {
                MessageBox.Show(
                    "LocationName : " + ie.LocationName
                    + "\rLocationURL : " + ie.LocationURL
                    + "\rName : " + ie.Name
                    + "\rPath : " + ie.Path
                    + "\rType : " + ie.Type
                    + "\rHWND : " + ie.HWND
                );
                IHTMLDocument2 doc2 = (IHTMLDocument2)ie.Document;
                //MessageBox.Show(doc2.url);
                hWnd = (int)ie.HWND;
            }
        }

        /// <summary>
        /// 핸들값 가져오기
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            //익스플로러
            hWnd = FindWindow("IEFrame", null);

            //크롬
            //int hIe1 = FindWindow(null, "NAVER - Chrome");
            MessageBox.Show(hWnd.ToString());
            //MessageBox.Show(hIe1.ToString());
        }

        /// <summary>
        /// 핸들값 가지고 종료
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            SendMessage(hWnd, WM_CLOSE, 0, null);
        }

        /// <summary>
        /// 추가
        /// </summary>
        private void bt_Insert_Click(object sender, EventArgs e)
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Description = "방화벽 규칙에 대한 설명을 입력합니다";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.ApplicationName = Application.ExecutablePath + @"\app.exe";
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = "Rule name"; // 방화벽 규칙을 구분하는 이름, 삭제시에도 사용됩니다
            firewallRule.Enabled = true;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);
        }
        /// <summary>
        /// 삭제
        /// </summary>
        private void bt_Delete_Click(object sender, EventArgs e)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Remove("Rule name");
        }
        /// <summary>
        /// 확인
        /// </summary>
        private void bt_Check_Click(object sender, EventArgs e)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            foreach (INetFwRule rule in firewallPolicy.Rules)
            {
                Console.WriteLine(rule.Name);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //실행중인 ie URL 가져오기
            foreach (InternetExplorer ie in new ShellWindowsClass())
            {
                if (ie.Name == "Internet Explorer")
                { 
                    if (string.Compare("http://www.naver.com/", ie.LocationURL) == 0)
                    {
                        SendMessage((int)ie.HWND, WM_CLOSE, 0, null);
                        MessageBox.Show("ie Close");
                    }
                }

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //DW_Test2.Program.Main(null);
        }
    }
}

/*
    "NAVER - Windows Internet Explorer" Chrome_WidgetWin_1 Refresh(); // Navigate
   private void getWin()
        {
            //[0] 현재의 윈두우 핸들 얻기
            int handle = GetForegroundWindow();


            //[1] SHDocVw 의 브라우저에서 현재부라우져들 검출 
            foreach (SHDocVw.WebBrowser wb in new SHDocVw.ShellWindowsClass())
            {
                //[2] 각각의 브라우져 핸들과 현제Top의 핸들 검출
                if (wb.HWND.Equals(handle))
                {
                    //[3] 검출된 브라우져의 타입캐스팅
                    InternetExplorer ie = wb as InternetExplorer;
                    _name = ie.Name;
                    _resultURL = ie.LocationURL;

                }
            }
        }
*/

/* 프로세스 죽이기 MFC

BOOL KillProcess(CString sExeName)
{
    // Eocs_2007_0823 : Kill Process by Exe Name
    sExeName.MakeUpper();
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

    if ((int)hSnapshot != -1)
    {
        PROCESSENTRY32 pe32;
        pe32.dwSize = sizeof(PROCESSENTRY32);
        BOOL bContinue;
        CString strProcessName;

        if (Process32First(hSnapshot, &pe32))
        {
            do
            {
                strProcessName = pe32.szExeFile; //strProcessName이 프로세스 이름;
                strProcessName.MakeUpper();
                if ((strProcessName.Find(sExeName, 0) != -1))
                {

                    HANDLE hProcess = OpenProcess(SYNCHRONIZE | PROCESS_TERMINATE, TRUE, pe32.th32ProcessID);
                    if (hProcess)
                    {
                        DWORD dwExitCode;
                        GetExitCodeProcess(hProcess, &dwExitCode);
                        TerminateProcess(hProcess, dwExitCode);
                        CloseHandle(hProcess);
                        CloseHandle(hSnapshot);
                        return TRUE;
                    }
                    return FALSE;
                }
                //AfxMessageBox( strProcessName );

                bContinue = Process32Next(hSnapshot, &pe32);
            } while (bContinue);
        }

        CloseHandle(hSnapshot);
    }
    return FALSE;
}
*/
