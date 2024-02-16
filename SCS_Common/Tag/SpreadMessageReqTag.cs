using System;
using System.Net;

namespace SCS.Net.Common
{
    /// <summary>
    /// 메시지 전달 요청 데이터입니다.
    /// </summary>
    [Serializable]
    public class SpreadMessageReqTag
    {
        /// <summary>
        /// 메시지를 전달할 IP 주소입니다.
        /// </summary>
        public IPAddress[] spreadingIP { get; private set; }

        /// <summary>
        /// 전달할 메시지입니다.
        /// </summary>
        public string spreadText { get; private set; }

        /// <summary>
        /// 메세지 전달 요청을 위한 객체를 생성합니다.
        /// </summary>
        /// <param name="spreadingIP">메시지를 전달할 IP 주소입니다.</param>
        /// <param name="spreadText">전달할 메시지입니다.</param>
        public SpreadMessageReqTag(IPAddress[] spreadingIP, string spreadText)
        {
            if (spreadingIP == null || spreadingIP.Length == 0 || spreadText == null)
                throw new ArgumentNullException();

            this.spreadingIP = spreadingIP;
            this.spreadText = spreadText;
        }
    }
}
