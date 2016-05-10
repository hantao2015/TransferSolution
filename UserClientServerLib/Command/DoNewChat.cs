using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;

namespace UserClientServerLib.Command
{
    public class DoNewChat : CommandBase<UserClientSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserClientSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var newuserSessionId = Encoding.UTF8.GetString(requestInfo.Parameters[0]);
                var server = session.AppServer as UserClientServer;
                server.OnNewChat(newuserSessionId, session);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
