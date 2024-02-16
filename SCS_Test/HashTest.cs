using System;
using SCS.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace SCS_Test
{
    [TestClass]
    public class HashTest
    {
        [TestMethod]
        public void Sha256Test()
        {
            //Trace.WriteLine("Minecraft: " + FileHash.GetFileSHA256Str(@"C:\Users\NSC\AppData\Roaming\.minecraft\Minecraft.exe"));
            Trace.WriteLine(URLTidy.UrlTidy("https://search.naver.com/search.naver?sm=tab_hty.top&where=nexearch&ie=utf8&query=%2F"));
        }
    }
}
