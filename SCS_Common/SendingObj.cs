using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SCS.Net.Common
{
    /// <summary>
    /// 네트워크를 통해 전송될 객체입니다.
    /// </summary>
    [Serializable]
    public class SendingObj
    {
        /// <summary>
        /// 발생한 예외를 가져옵니다. 없을 경우 null입니다.
        /// </summary>
        public Exception Except { get; private set; }

        /// <summary>
        /// 전송 유형을 나타냅니다.
        /// </summary>
        public SendingType SendingType { get; private set; }

        /// <summary>
        /// 같이 전송되는 추가 정보 객체입니다. 없을 경우 null입니다.
        /// </summary>
        public object TagData { get; private set; }

        /// <summary>
        /// 전송 유형과 같이 전송할 객체, 서버 또는 클라이언트에서 발생한 예외 메시지를 지정하여 초기화합니다.
        /// </summary>
        /// <param name="sendingType">전송 유형입니다.</param>
        /// <param name="tagData">같이 전송할 객체입니다.</param>
        /// <param name="exception">발생한 예외 메시지입니다.</param>
        public SendingObj(SendingType sendingType, object tagData = null, Exception exception = null)
        {
            SendingType = sendingType;
            TagData = tagData;
            Except = exception;
        }

        /// <summary>
        /// 지정된 Stream에 직렬화한 객체를 씁니다.
        /// </summary>
        public void Serialize(Stream serializeStream)
        {
            if (serializeStream == null)
                throw new ArgumentNullException();

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(serializeStream, this); // 직렬화합니다.

        }

        /// <summary>
        /// 지정된 Stream에서 읽어들여 역직렬화합니다.
        /// </summary>
        /// <param name="deserializeStream">읽어들일 스트림입니다.</param>
        public static SendingObj Deserialize(Stream deserializeStream)
        {
            if (deserializeStream == null)
                throw new ArgumentNullException();

            BinaryFormatter bf = new BinaryFormatter();
            return (SendingObj)bf.Deserialize(deserializeStream); // 역직렬화한 뒤 SendingObj로 캐스팅하여 반환합니다.
        }

        /// <summary>
        /// 지정된 바이트 배열을 읽어들여 역직렬화합니다.
        /// </summary>
        /// <param name="deserializeStream">읽어들일 바이트 배열입니다.</param>
        public static SendingObj Deserialize(byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream(byteArray);
            return (SendingObj)bf.Deserialize(memStream); // 역직렬화한 뒤 SendingObj로 캐스팅하여 반환합니다.
        }
    }
}
