using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCS.Surveillance.Monitor;
using System;
using System.Drawing;
using System.Threading;

namespace SCS_Test
{
    [TestClass]
    public class MonitorTest
    {
        [TestMethod]
        public void ScreenCaptureTest()
        {
            ScreenCapture sc = new ScreenCapture();

            sc.StartCapture(10, ScreenCaptureTest_event);
            Thread.Sleep(1000 * 30);
            sc.Dispose();
        }

        private void ScreenCaptureTest_event(Bitmap image)
        {
            image.Save(@"C:\Users\NSC\Desktop\" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".Jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
