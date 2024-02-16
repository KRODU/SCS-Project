using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Net.Common
{
    /// <summary>
    /// 인터넷 또는 프로그램 제한을 추가합니다.
    /// </summary>
    [Serializable]
    public class RestrictionModifyTag
    {
        /// <summary>
        /// 인터넷 제한 또는 프로그램 제한을 의미합니다.
        /// </summary>
        public ResType RestrictionFlag { get; private set; }

        /// <summary>
        /// 제한 추가 또는 삭제를 의미합니다.
        /// </summary>
        public ResMod AddOrMinus { get; private set; }

        /// <summary>
        /// 인터넷 제한일 경우 URL, 프로그램 제한일 경우 해시값입니다.
        /// </summary>
        public string RestrictionData { get; private set; }

        public RestrictionModifyTag(ResType restrictionFlag, ResMod addOrMinus, string restrictionData)
        {
            if (restrictionData == null)
                throw new ArgumentNullException();

            RestrictionFlag = restrictionFlag;
            RestrictionData = restrictionData;
            AddOrMinus = addOrMinus;
        }
    }

    public enum ResType
    {
        Program, Uri
    }

    public enum ResMod
    {
        Add, Minus
    }
}
