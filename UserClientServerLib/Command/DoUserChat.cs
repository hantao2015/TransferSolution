using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using UserCommon;

namespace UserClientServerLib.Command
{
    public class DoUserChat : CommandBase<UserClientSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserClientSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var message = Tool.SerializeHelp.Deserialize<Message>(requestInfo.Parameters[0]);
                session.OnUserChat(message);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
