using SuperSocket.ClientEngine.Protocol;
using System;
using TransferCommon;
namespace TransferClientLib.UpLoad
{
    internal class UpLoadReader : IClientCommandReader<TransferCommandInfo>
    {
        private readonly ArraySegmentList m_BufferSegments;

        internal UpLoadReader(UpLoadClientEngine upLoadEngine)
        {
            this.UpLoadEngine = upLoadEngine;
            this.m_BufferSegments = new ArraySegmentList();
        }

        public UpLoadReader(UpLoadReader previousCommandReader)
        {
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

        public TransferCommandInfo GetCommandInfo(byte[] readBuffer, int offset, int length, out int left)
        {
            left = 0;
            if ((readBuffer.Length - length) >= 4)
            {
                UpLoadOP dop = (UpLoadOP)((readBuffer[offset] * 0x100) + readBuffer[offset + 1]);
                if (!Enum.IsDefined(typeof(UpLoadOP), dop))
                {
                    return null;
                }
                left = 4;
                int count = (readBuffer[offset + 2] * 0x100) + readBuffer[offset + 3];
                byte[] dst = new byte[count];
                Buffer.BlockCopy(readBuffer, left, dst, 0, count);
                switch (dop)
                {
                    case UpLoadOP.DoUpLoad:
                        return new TransferCommandInfo(UpLoadOP.DoUpLoad.ToString(), dst);

                    case UpLoadOP.DoExists:
                        return new TransferCommandInfo(UpLoadOP.DoExists.ToString(), dst);

                    //case UpLoadOP.DoEnd:
                    //    return new TransferCommandInfo(UpLoadOP.DoEnd.ToString(), dst);

                    case UpLoadOP.DoStop:
                        return new TransferCommandInfo(UpLoadOP.DoStop.ToString(), dst);
                }
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

        public IClientCommandReader<TransferCommandInfo> NextCommandReader { get; internal set; }

        internal UpLoadClientEngine UpLoadEngine { get; private set; }
    }
}
