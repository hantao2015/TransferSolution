using System;
using Tool;
using TransferClientLib.UpLoad;
using TransferCommon;
namespace TransferClientLib.UpLoad.Command
{
    internal class DoUpLoad : UpLoadCommandBase
    {
        public override void ExecuteCommand(UpLoadClientEngine session, TransferCommandInfo commandInfo)
        {
            try
            {
                UpLoadInfo ServerConfigInfo = SerializeHelp.Deserialize<UpLoadInfo>(commandInfo.Data);
                long CurrentPos = ServerConfigInfo.TransferPos + ServerConfigInfo.TransferedLength;
                long TransferedLength = ServerConfigInfo.TransferedLength;
                long TransferLength = ServerConfigInfo.TransferLength;
                session.m_fileStream.Position = CurrentPos;
                session.UpLoadInfo = ServerConfigInfo;
                session.OnTransferStart();
                while (TransferedLength < TransferLength)
                {
                    lock (session.lockobj)
                    {
                        if (session == null)
                            return;
                        if (session.StatusCode == 1)
                        {
                            session.StatusCode = 2;
                            session.m_StopEvent.Set();
                            return;
                        }
                        int length = session.m_fileStream.Read(session.readBuffer, 0, session.PacketSize);
                        session.SendData(UpLoadOP.DoData, session.readBuffer, 0, length);
                        TransferedLength += length;
                        session.UpLoadInfo.TransferedLength = TransferedLength;
                        session.OnTransferStep(TransferLength, TransferedLength, length);
                    }
                }
                session.SendData(UpLoadOP.DoEnd, new byte[] { 1 }, 0, 1);
                new Action(() =>
                {
                    session.m_EndEvent.WaitOne(5000);
                    session.DoEnd();
                }).BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                ex.ToString();
                //error to do;
            }
        }

        public override string Name
        {
            get
            {
                return UpLoadOP.DoUpLoad.ToString();
            }
        }
    }
}
