using System;
using System.Drawing;

namespace SCS.Net.Common
{
    /// <summary>
    /// 캡쳐한 스크린샷에 대한 데이터입니다.
    /// </summary>
    [Serializable]
    public class CapturedScreenTag
    {
        /// <summary>
        /// 캡쳐 시간입니다.
        /// </summary>
        public DateTime CapturedTime { get; private set; }

        /// <summary>
        /// 캡쳐된 이미지입니다.
        /// </summary>
        public Image CapturedImage { get; private set; }

        /// <summary>
        /// 캡쳐 데이터 전송을 위한 객체를 생성합니다.
        /// </summary>
        /// <param name="time">캡쳐된 시간입니다.</param>
        /// <param name="image">캡쳐된 이미지입니다.</param>
        public CapturedScreenTag(DateTime time, Image image)
        {
            if (time == null || image == null)
                throw new ArgumentNullException();

            CapturedTime = time;
            CapturedImage = image;
        }
    }
}
