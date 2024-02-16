using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SCS.Common
{

    public class ListSegmentStream : Stream
    {
        private IList<ArraySegment<byte>> m_buffers;

        private int m_listCurIndex;
        private int m_segmentCurIndex;
        private int m_listCount;

        public ListSegmentStream(IList<ArraySegment<byte>> buffers)
        {
            m_buffers = buffers;
            m_listCurIndex = 0;
            m_segmentCurIndex = 0;
            m_listCount = m_buffers.Count;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                int ret = 0;
                foreach (ArraySegment<byte> item in m_buffers)
                    ret += item.Count;

                return ret;
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException();

            if (offset < 0)
                throw new ArgumentOutOfRangeException();

            if (count < 0)
                throw new ArgumentOutOfRangeException();

            if (buffer.Length < count + offset)
                throw new ArgumentException();

            int totalWrite = 0; // 버퍼에 쓴 총 바이트 수 입니다.
            int bufIndex = offset; // buffer에 몇번 인덱스부터 써야하는지를 나타냅니다.
            int curWriteBytes; // 이번에 버퍼에 쓸 바이트 수 입니다.
            int curRestInSegment; // 현재 segment에서의 남은 바이트 수입니다.
            ArraySegment<byte> curArraySegment; // 현재 복사중인 ArraySegment 입니다.
            int restBuffer = buffer.Length - offset; // 버퍼에 얼마나 더 쓸 수 있는지를 나타냅니다.

            while (m_listCurIndex < m_listCount && totalWrite < count)
            {
                // 현재의 ArraySegment를 가져옵니다.
                curArraySegment = m_buffers[m_listCurIndex];

                // 현재 segment에서 더 쓸 데이터가 없는 경우 다음 segment로 넘어갑니다.
                if (m_segmentCurIndex >= curArraySegment.Count)
                {
                    Debug.Assert(m_segmentCurIndex > curArraySegment.Count);
                    m_listCurIndex++;
                    m_segmentCurIndex = 0;
                    continue;
                }

                // 현재의 segment에서 몇개의 바이트를 더 넣을 수 있는지 계산합니다.
                curRestInSegment = curArraySegment.Count - m_segmentCurIndex;

                // 현재 리스트를 모두 복사할 충분한 공간이 있는 경우 모두 복사
                if (restBuffer >= curRestInSegment)
                    curWriteBytes = curRestInSegment;
                // 부족할 경우엔 남은 버퍼 바이트 수에 맞춥니다.
                else
                    curWriteBytes = restBuffer;

                // 바이트 배열을 복사합니다.
                Array.Copy(curArraySegment.Array, curArraySegment.Offset + m_segmentCurIndex, buffer, bufIndex, curWriteBytes);
                
                bufIndex += curWriteBytes;
                restBuffer -= curWriteBytes;
                totalWrite += curWriteBytes;
                m_segmentCurIndex += curWriteBytes;
            }

            return totalWrite;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
