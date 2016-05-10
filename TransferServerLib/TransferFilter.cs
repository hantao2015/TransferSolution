using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using TransferCommon;

namespace TransferServerLib
{
    internal class TransferFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public TransferFilter()
            : base(5)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return ((header[offset + 3] * 256) + header[offset + 4]);
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            int num = header.Array[0];
            if (num == 0)
            {
                UpLoadOP dop = (UpLoadOP)(header.Array[1] * 256) + header.Array[2];
                return new BinaryRequestInfo(dop.ToString(), bodyBuffer.CloneRange<byte>(offset, length));
            }
            return null;
        }
    }
}
