using System;
using System.Net;

namespace SCS.Net.Common
{
    /// <summary>
    /// 연결되지 않은 상태에서 전송을 시도할 때 발생하는 오류입니다.
    /// </summary>
    public class NotConnectedException : ApplicationException
    {
        public override string Message => "연결되지 않았습니다.";
    }

    /// <summary>
    /// 특정 IP에 대한 전송 결과를 나타냅니다.
    /// </summary>
    public class SendingSpecResult
    {
        /// <summary>
        /// 어떤 IP를 대상으로 한 전송 결과인지를 나타냅니다.
        /// </summary>
        public IPAddress IP { get; private set; }

        /// <summary>
        /// 전송 결과입니다.
        /// </summary>
        public SendResult Result { get; private set; }

        /// <summary>
        /// 발생한 오류입니다. 오류가 발생하지 않은 경우 null입니다.
        /// </summary>
        public Exception Error { get; private set; }

        public SendingSpecResult(IPAddress ip, SendResult result, Exception error = null)
        {
            IP = ip;
            Result = result;
            Error = error;
        }
    }

    /// <summary>
    /// 전송 결과를 나타냅니다.
    /// </summary>
    public enum SendResult
    {
        /// <summary>
        /// 성공적으로 전송되었습니다.
        /// </summary>
        Succeed,

        /// <summary>
        /// 연결되지 않아 전송할 수 없습니다.
        /// </summary>
        NotConnected,

        /// <summary>
        /// 전송오류가 발생했습니다.
        /// </summary>
        SendingError
    }
}
