using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Net.Common
{
    /// <summary>
    /// 프로그램 사용 로그 데이트입니다.
    /// </summary>
    [Serializable]
    public class ProgramLogTag
    {
        /// <summary>
        /// 프로그램이 시작된 시간입니다.
        /// </summary>
        public DateTime StartTime { private set; get; }

        /// <summary>
        /// 프로그램이 끝난 시간입니다.
        /// </summary>
        public DateTime EndTime { private set; get; }

        /// <summary>
        /// 프로그램에 대한 Hash값입니다.
        /// </summary>
        public string HashValue { private set; get; }

        /// <summary>
        /// 프로그램의 이름입니다.
        /// </summary>
        public string ProgramTitle { private set; get; }

        public ProgramLogTag(DateTime startTime, DateTime endTime, string hashValue, string programTitle)
        {
            StartTime = startTime;
            EndTime = endTime;
            HashValue = hashValue;
            ProgramTitle = programTitle;
        }
    }
}
