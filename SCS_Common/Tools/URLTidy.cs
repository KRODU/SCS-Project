using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Common
{
    /// <summary>
    /// URL을 도메인 주소만 남겨서 정리합니다.
    /// </summary>
    public static class URLTidy
    {
        private const string httpToken = "http://";

        private const string httpsToken = "https://";

        /// <summary>
        /// URL을 도메인 주소만 남겨서 정리합니다.
        /// </summary>
        public static string UrlTidy(string url)
        {
            // http 또는 https를 제거함.
            if (url.StartsWith(httpToken))
                url = url.Remove(0, httpToken.Length);
            else if (url.StartsWith(httpsToken))
                url = url.Remove(0, httpsToken.Length);

            int slash = url.IndexOf('/');

            if (slash >= 0)
                url = url.Remove(slash);

            return url;
        }
    }
}
