using SuperSocket.ClientEngine;
using SuperSocket.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UserClientLib
{
    public class MyAsyncTcpSession : AsyncTcpSession
    {
        public MyAsyncTcpSession(EndPoint remoteEndPoint)
            : base(remoteEndPoint)
        {

        }
        public MyAsyncTcpSession(EndPoint remoteEndPoint, int receiveBufferSize)
            : base(remoteEndPoint, receiveBufferSize)
        {

        }
        protected override void OnGetSocket(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            base.OnGetSocket(e);
            if (e.ConnectSocket == null)
                return;
            var handler = m_GetSocket;
            if (handler == null)
                return;
            handler(new GetSocketEventArgs(e.ConnectSocket.LocalEndPoint));
        }
        private VEventHandler<GetSocketEventArgs> m_GetSocket;
        public event VEventHandler<GetSocketEventArgs> GetSocket
        {
            add { m_GetSocket += value; }
            remove { m_GetSocket -= value; }
        }
    }
    public class GetSocketEventArgs : EventArgs
    {

        public GetSocketEventArgs(EndPoint localEndPoint)
        {
            LocalEndPoint = localEndPoint;
        }
        public EndPoint LocalEndPoint { get; private set; }
    }
}
