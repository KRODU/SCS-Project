using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCS.Common;
using SCS.Net.Server;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SCS_Test
{
    [TestClass]
    public class SFTPTest
    {
        [TestMethod]
        public void SFTPT()
        {
            FTPImageSender ftpObj = new FTPImageSender();
            ftpObj.SetServerInfo("54.64.67.69", "ubuntu", @"C:\Users\NSC\Desktop\SH\SCS\scs.pem", "/var/www/html/assets/img/userScreen");
            ftpObj.UploadImage(ScreenShot(), "uploadTest.jpg");
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
    }
}
