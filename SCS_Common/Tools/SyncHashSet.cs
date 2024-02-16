using System;
using System.Collections.Generic;

namespace SCS.Common
{
    /// <summary>
    /// 스레드 동기화를 지원하는 <see cref="HashSet{T}"/> 입니다.
    /// </summary>
    [Serializable]
    public class SyncHashSet<T>
    {
        /// <summary>
        /// 내부적으로 사용하는 <see cref="HashSet{T}"/> 개체입니다.
        /// </summary>
        private HashSet<T> m_HashSet;

        /// <summary>
        /// 기본 같음 비교자를 사용하는 새 인스턴스를 초기화합니다.
        /// </summary>
        public SyncHashSet()
        {
            m_HashSet = new HashSet<T>();
        }

        public SyncHashSet(IEqualityComparer<T> comparer)
        {
            m_HashSet = new HashSet<T>(comparer);
        }

        /// <summary>
        /// 집합에 포함된 요소 수를 가져옵니다.
        /// </summary>
        public int Count { get { lock (this) { return m_HashSet.Count; } } }

        /// <summary>
        /// 지정된 요소를 집합에 추가합니다.
        /// </summary>
        /// <param name="item">집합에 추가할 요소입니다.</param>
        /// <returns>요소가 개체에 추가되었으면 true이고, 요소가 이미 있으면 false입니다.</returns>
        public bool Add(T item) { lock (this) { return m_HashSet.Add(item); } }

        /// <summary>
        /// 개체에서 요소를 모두 제거합니다.
        /// </summary>
        public void Clear() { lock (this) { m_HashSet.Clear(); } }

        /// <summary>
        /// 개체에 지정된 요소가 포함되어 있는지 확인합니다.
        /// </summary>
        /// <param name="item">개체에서 찾을 요소입니다.</param>
        /// <returns>개체에 지정된 요소가 들어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Contains(T item) { lock (this) { return m_HashSet.Contains(item); } }

        /// <summary>
        /// 개체에서 지정된 요소를 제거합니다.
        /// </summary>
        /// <param name="item">제거할 요소입니다.</param>
        /// <returns>item 개체가 없으면 이 메서드는 false을(를) 반환합니다.</returns>
        public bool Remove(T item) { lock (this) { return m_HashSet.Remove(item); } }
    }
}
