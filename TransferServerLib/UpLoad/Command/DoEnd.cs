using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.IO;
using TransferCommon;
namespace TransferServerLib.UpLoad.Command
{
    public class DoEnd : CommandBase<TransferSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(TransferSession session, BinaryRequestInfo requestInfo)
        {
            session.UpLoadEngine.m_EndEvent.Set();
        }
    }
}
