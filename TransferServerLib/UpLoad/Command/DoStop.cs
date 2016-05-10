using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.IO;
using Tool;
using TransferCommon;
namespace TransferServerLib.UpLoad.Command
{
    public class DoStop : CommandBase<TransferSession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(TransferSession session, BinaryRequestInfo requestInfo)
        {
            try
            {
                string configName = Path.GetFullPath(session.UpLoadEngine.UpLoadInfo.SaveName) + Config.ConfigSuffix;
                SerializeHelp.SerializeToFile<UpLoadInfo>(session.UpLoadEngine.UpLoadInfo, configName);
                session.UpLoadEngine.SendData(UpLoadOP.DoStop, new byte[] { 1 }, 0, 1);
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("DoData").Error(ex.Message);
            }
        }
    }
}
