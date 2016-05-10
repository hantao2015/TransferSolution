using SuperSocket.ClientEngine.Protocol;
using System;
using UserCommon;

namespace UserClientLib
{
    internal class UserReader : IClientCommandReader<BaseCommandInfo>
    {
        private readonly ArraySegmentList m_BufferSegments;
        private int m_OPTypeLength;
        private int m_OPCodeLength;
        private int m_ParaCountLength;
        private int m_PerParaCountLength;
        internal UserReader(UserClientEngine engine)
        {
            m_OPTypeLength = 1;
            m_OPCodeLength = 1;
            m_ParaCountLength = 1;
            m_PerParaCountLength = 2;
            this.Engine = engine;
            this.m_BufferSegments = new ArraySegmentList();
        }

        public UserReader(UserReader previousCommandReader)
        {
            m_OPTypeLength = 1;
            m_OPCodeLength = 1;
            m_ParaCountLength = 1;
            m_PerParaCountLength = 2;
            this.m_BufferSegments = previousCommandReader.BufferSegments;
        }

        protected void AddArraySegment(byte[] buffer, int offset, int length)
        {
            this.BufferSegments.AddSegment(buffer, offset, length, true);
        }

        protected void ClearBufferSegments()
        {
            this.BufferSegments.ClearSegements();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="rest">剩余部分</param>
        /// <returns></returns>
        public BaseCommandInfo GetCommandInfo(byte[] readBuffer, int offset, int length, out int rest)
        {
            rest = 0;
            if ((readBuffer.Length - length) >= (m_OPTypeLength + m_OPCodeLength + m_ParaCountLength + m_PerParaCountLength))
            {
                offset = offset + m_OPTypeLength;
                UserOP dop;
                if (m_OPCodeLength == 1)
                {
                    dop = (UserOP)(readBuffer[offset] & 0xFF);
                }
                else if (m_OPCodeLength == 2)
                {
                    dop = (UserOP)((readBuffer[offset] & 0xFF)
                            | ((readBuffer[offset + 1] & 0xFF) << 8));
                }
                else if (m_OPCodeLength == 3)
                {
                    dop = (UserOP)((readBuffer[offset] & 0xFF)
                            | ((readBuffer[offset + 1] & 0xFF) << 8)
                            | ((readBuffer[offset + 2] & 0xFF) << 16));
                }
                else
                {
                    dop = (UserOP)((readBuffer[offset] & 0xFF)
                            | ((readBuffer[offset + 1] & 0xFF) << 8)
                            | ((readBuffer[offset + 2] & 0xFF) << 16)
                            | ((readBuffer[offset + 3] & 0xFF) << 24));
                }
                if (!Enum.IsDefined(typeof(UserOP), dop))
                {
                    return null;
                }
                offset = offset + m_OPCodeLength;
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
                rest = length - offset;
                return new BaseCommandInfo(dop.ToString(), bodyBuffer);
            }
            return null;
        }

        protected ArraySegmentList BufferSegments
        {
            get
            {
                return this.m_BufferSegments;
            }
        }

        public IClientCommandReader<BaseCommandInfo> NextCommandReader { get; internal set; }

        internal UserClientEngine Engine { get; private set; }
    }
}
