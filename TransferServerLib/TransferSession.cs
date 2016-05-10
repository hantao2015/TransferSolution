using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using TransferServerLib.UpLoad;
namespace TransferServerLib
{
    public class TransferSession : TransferSession<TransferSession>
    {
        private object lockobj = new object();
        private UpLoadServerEngine _UpLoadEngine;
        public UpLoadServerEngine UpLoadEngine
        {
            get
            {
                if (this._UpLoadEngine == null)
                {
                    lock (lockobj)
                    {
                        if (this._UpLoadEngine == null)
                        {
                            this._UpLoadEngine = new UpLoadServerEngine(this);
                        }
                    }
                }
                return this._UpLoadEngine;
            }
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            if (_UpLoadEngine != null)
                _UpLoadEngine.Dispose();
            base.OnSessionClosed(reason);
        }
    }
    public class TransferSession<TWebSocketSession> : AppSession<TWebSocketSession, BinaryRequestInfo>,
        IAppSession, ISessionBase where TWebSocketSession : TransferSession<TWebSocketSession>, new()
    {
    }
}
