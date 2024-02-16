using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS.Net.Server
{
    public partial class LogViewer : Form
    {
        private const string returnLine = "\r\n";

        public LogViewer()
        {
            InitializeComponent();
            Icon = Properties.Resources.server_connection;
        }

        private void LogViewer_Load(object sender, EventArgs e)
        {
            foreach (string logT in Program.m_Logger.Log)
            {
                logTextBox.AppendText(logT + returnLine);
            }

            Program.m_Logger.WriteLogEvent += Logger_WriteLogEvent;
            Program.ConEvent += Program_ConEvent;
            Program.DisconEvent += Program_ConEvent;
            Program_ConEvent(null);
        }

        private void Program_ConEvent(IPAddress obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(Program_ConEvent_Raise));
            }
            else
            {
                Program_ConEvent_Raise();
            }
        }

        private void Program_ConEvent_Raise()
        {
            userListBox.Items.Clear();

            foreach (IPAddress eachAdd in Program.ConnectionList)
            {
                userListBox.Items.Add(eachAdd.ToString());
            }
        }

        private void Logger_WriteLogEvent(string obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    logTextBox.AppendText(obj + returnLine);
                    logTextBox.SelectionStart = logTextBox.TextLength;
                    logTextBox.ScrollToCaret();
                }));
            }
            else
            {
                logTextBox.AppendText(obj + returnLine);
                logTextBox.SelectionStart = logTextBox.TextLength;
                logTextBox.ScrollToCaret();
            }
        }

        private void LogViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.m_Logger.WriteLogEvent -= Logger_WriteLogEvent;
            Program.ConEvent -= Program_ConEvent;
            Program.DisconEvent -= Program_ConEvent;
        }
    }
}
