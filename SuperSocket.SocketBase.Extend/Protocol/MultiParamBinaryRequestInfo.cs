using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSocket.SocketBase.Protocol
{
    public class MultiParamBinaryRequestInfo : RequestInfo<byte[]>
    {
        public MultiParamBinaryRequestInfo(string key, byte[] body, byte[][] parameters)
            : base(key, body)
        {
            Parameters = parameters;
        }
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public byte[][] Parameters { get; private set; }
        public byte[] this[int index]
        {
            get { return Parameters[index]; }
        }
    }
}
