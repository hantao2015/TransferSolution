using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
namespace TransferServerLib.UpLoad.Command
{
    public class DoData : CommandBase<TransferSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(TransferSession session, BinaryRequestInfo requestInfo)
        {
            try
            {
                if (session.UpLoadEngine != null)
                {
                    lock (session.UpLoadEngine.lockobj)
                    {
                        session.UpLoadEngine.m_fileStream.Write(requestInfo.Body, 0, requestInfo.Body.Length);
                        session.UpLoadEngine.UpLoadInfo.TransferedLength += requestInfo.Body.Length;
                        session.UpLoadEngine.OnTransferStep(
                            session.UpLoadEngine.UpLoadInfo.TransferLength,
                            session.UpLoadEngine.UpLoadInfo.TransferedLength,
                            requestInfo.Body.Length);
                        if (session.UpLoadEngine.UpLoadInfo.TransferLength <= session.UpLoadEngine.UpLoadInfo.TransferedLength)
                        {
                            new Action(() =>
                            {
                                session.UpLoadEngine.m_EndEvent.WaitOne(5000);
                                session.UpLoadEngine.DoEnd();
                            }).BeginInvoke(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("DoData").Error(ex.Message);
            }
        }
    }
}
