using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.IO;
using Tool;
using TransferCommon;
namespace TransferServerLib.UpLoad.Command
{
    public class DoUpLoad : CommandBase<TransferSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(TransferSession session, BinaryRequestInfo requestInfo)
        {
            try
            {
                UpLoadInfo ClientConfigInfo = SerializeHelp.Deserialize<UpLoadInfo>(requestInfo.Body);
                string saveName = Path.GetFullPath(ClientConfigInfo.SaveName);
                if (File.Exists(saveName))
                {
                    session.UpLoadEngine.SendData(UpLoadOP.DoExists, new byte[] { 1 }, 0, 1);
                    return;
                }
                session.UpLoadEngine.OnStartTransfer(ClientConfigInfo);
                session.UpLoadEngine.DoUpLoad(ClientConfigInfo);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger("DoUpLoad").Error(ex.Message);
            }
        }
    }
}
