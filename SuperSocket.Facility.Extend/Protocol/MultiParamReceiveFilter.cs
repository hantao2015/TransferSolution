using System;
using System.Text;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace SuperSocket.Facility.Protocol
{
    public abstract class MultiParamReceiveFilter<TRequestInfo> : IReceiveFilter<TRequestInfo>
        where TRequestInfo : IRequestInfo
    {
        private int m_OPTypeLength;
        private int m_OPCodeLength;
        private int m_ParaCountLength;
        private int m_PerParaCountLength;
        private int left;
        private ArraySegment<byte> m_Header;
        public MultiParamReceiveFilter()
        {
            m_OPTypeLength = 1;
            m_OPCodeLength = 1;
            m_ParaCountLength = 1;
            m_PerParaCountLength = 2;
        }
        public MultiParamReceiveFilter(int OPTypeLength, int OPCodeLength
            , int ParaCountLength, int PerParaCountLength)
        {
            m_OPTypeLength = OPTypeLength;
            m_OPCodeLength = OPCodeLength;
            m_ParaCountLength = ParaCountLength;
            m_PerParaCountLength = PerParaCountLength;
        }
        protected readonly static TRequestInfo NullRequestInfo = default(TRequestInfo);
        public virtual TRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;
            if (m_ParaCountLength <= 0 || m_OPTypeLength <= 0 || m_OPCodeLength <= 0 || m_PerParaCountLength <= 0)
            {
                State = FilterState.Error;
                return NullRequestInfo;
            }
            if (length < (m_OPTypeLength + m_OPCodeLength + m_ParaCountLength))
            {
                State = FilterState.Error;
                return NullRequestInfo;
            }
            if (toBeCopied)
                m_Header = new ArraySegment<byte>(readBuffer.CloneRange(offset, m_OPTypeLength + m_OPCodeLength + m_ParaCountLength));
            else
                m_Header = new ArraySegment<byte>(readBuffer, offset, m_OPTypeLength + m_OPCodeLength + m_ParaCountLength);

            offset = offset + m_OPTypeLength + m_OPCodeLength;
            int paraCount = 1;
            if (m_ParaCountLength == 1)
            {
                paraCount = (int)(readBuffer[offset] & 0xFF);
            }
            else if (m_ParaCountLength == 2)
            {
                paraCount = (int)((readBuffer[offset] & 0xFF)
                        | ((readBuffer[offset + 1] & 0xFF) << 8));
            }
            else if (m_ParaCountLength == 3)
            {
                paraCount = (int)((readBuffer[offset] & 0xFF)
                        | ((readBuffer[offset + 1] & 0xFF) << 8)
                        | ((readBuffer[offset + 2] & 0xFF) << 16));
            }
            else
            {
                paraCount = (int)((readBuffer[offset] & 0xFF)
                        | ((readBuffer[offset + 1] & 0xFF) << 8)
                        | ((readBuffer[offset + 2] & 0xFF) << 16)
                        | ((readBuffer[offset + 3] & 0xFF) << 24));
            }
            offset = offset + m_ParaCountLength;
            byte[][] bodyBuffer = new byte[paraCount][];
            for (int i = 0; i < paraCount; i++)
            {
                int PerparaCount = 1;
                if (m_PerParaCountLength == 1)
                {
                    PerparaCount = (int)(readBuffer[offset] & 0xFF);
                }
                else if (m_PerParaCountLength == 2)
                {
                    PerparaCount = (int)((readBuffer[offset] & 0xFF)
                            | ((readBuffer[offset + 1] & 0xFF) << 8));
                }
                else if (m_PerParaCountLength == 3)
                {
                    PerparaCount = (int)((readBuffer[offset] & 0xFF)
                            | ((readBuffer[offset + 1] & 0xFF) << 8)
                            | ((readBuffer[offset + 2] & 0xFF) << 16));
                }
                else
                {
                    PerparaCount = (int)((readBuffer[offset] & 0xFF)
                            | ((readBuffer[offset + 1] & 0xFF) << 8)
                            | ((readBuffer[offset + 2] & 0xFF) << 16)
                            | ((readBuffer[offset + 3] & 0xFF) << 24));
                }
                bodyBuffer[i] = new byte[PerparaCount];
                Buffer.BlockCopy(readBuffer, offset + m_PerParaCountLength, bodyBuffer[i], 0, PerparaCount);
                offset = offset + m_PerParaCountLength + PerparaCount;
            }
            return ResolveRequestInfo(m_Header, bodyBuffer);
        }
        protected abstract TRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[][] paraBuffer);
        public int LeftBufferSize
        {
            get { return left; }
        }

        public IReceiveFilter<TRequestInfo> NextReceiveFilter
        {
            get { return null; }
        }
        private void InternalReset()
        {
            left = 0;
        }
        public void Reset()
        {
            InternalReset();
        }

        public FilterState State { get; protected set; }
    }
}
