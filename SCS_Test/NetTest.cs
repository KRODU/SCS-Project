using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCS.Net.Client;
using SCS.Net.Server;
using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SCS_Test
{
    [TestClass]
    public class NetTest
    {
        [TestMethod]
        [STAThread]
        public void NetworkTest()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NetTestForm());
        }
    }
}
