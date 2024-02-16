using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Net.Common
{
    /// <summary>
    /// URL 접속에 대한 정보를 저장합니다.
    /// </summary>
    [Serializable]
    public class URLAccessTag
    {
        public DateTime AccessTime { private set; get; }

        public string Url { private set; get; }

        public URLAccessTag(DateTime time, string url)
        {
            AccessTime = time;
            Url = url;
        }
    }
}
