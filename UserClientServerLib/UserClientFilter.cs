using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Text;
using UserCommon;


namespace UserClientServerLib
{
    internal class UserClientFilter : MultiParamReceiveFilter<MultiParamBinaryRequestInfo>
    {
        public UserClientFilter()
            : base()
        {

        }

        protected override MultiParamBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[][] paraBuffer)
        {
            int num = header.Array[0];
            if (num == 0)
            {
                UserOP dop = (UserOP)header.Array[1];
                return new MultiParamBinaryRequestInfo(dop.ToString(), null, paraBuffer);
            }
            return null;
        }
    }
}
