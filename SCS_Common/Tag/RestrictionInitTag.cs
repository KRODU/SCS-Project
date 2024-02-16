using SCS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Net.Common
{
    /// <summary>
    /// 모든 초기 제한 설정 데이터들을 나타냅니다. 피감시자 프로그램 구동시 이 클래스에 제한 설정을 담아 전달됩니다.
    /// </summary>
    [Serializable]
    public class RestrictionInitTag
    {
        /// <summary>
        /// 제한할 프로그램 해시값입니다.
        /// </summary>
        public SyncHashSet<string> ProcRestrictionSet { get; private set; }

        /// <summary>
        /// 제한할 URL 목록입니다.
        /// </summary>
        public SyncHashSet<string> URLRestrictionSet { get; private set; }

        /// <summary>
        /// 화면을 몇초단위로 캡쳐할지 지정합니다. 0이하의 수일 경우 화면을 캡쳐하지 않습니다.
        /// </summary>
        public int ScreenCaptureInterval { get; private set; }

        public RestrictionInitTag(SyncHashSet<string> procRestrictionSet, SyncHashSet<string> urlRestrictionSet, int screenCaptureInterval)
        {
            ProcRestrictionSet = procRestrictionSet;
            URLRestrictionSet = urlRestrictionSet;
            ScreenCaptureInterval = screenCaptureInterval;
        }
    }
}
