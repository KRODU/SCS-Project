using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCS.Surveillance.Monitor;
using System;

namespace SCS_Test
{
    [TestClass]
    public class IEBlockTest
    {
        [TestMethod]
        public void IEBlock()
        {
            IEControl ieBlock = new IEControl();
            ieBlock.BlockList.Add("http://www.naver.com/");
            ieBlock.startIE();
        }
    }
}
