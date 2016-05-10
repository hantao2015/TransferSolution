using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.IO;
using System.Text;
using Tool;
using TransferCommon;
namespace TransferServerLib.UpLoad.Command
{
    public class DoCover : CommandBase<TransferSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(TransferSession session, BinaryRequestInfo requestInfo)
        {
            try
            {
                UpLoadInfo ClientConfigInfo = SerializeHelp.Deserialize<UpLoadInfo>(requestInfo.Body);
                string saveName = Path.GetFullPath(ClientConfigInfo.SaveName);
                if (File.Exists(saveName))
                {
                    File.Delete(saveName);
                }
                session.UpLoadEngine.OnStartTransfer(ClientConfigInfo);
                session.UpLoadEngine.DoUpLoad(ClientConfigInfo);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger("DoCover").Error(ex.Message);
            }
        }
    }
}
