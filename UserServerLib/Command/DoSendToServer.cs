using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;
namespace UserServerLib.ommand
{
    public class DoSendToServer : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var fromUser = UserManage.Instance.GetUser(session.SessionID);
                if (fromUser == null)
                    return;
                var msg = Tool.SerializeHelp.Deserialize<Message>(requestInfo.Parameters[0]);
                var server = session.AppServer as UserServer;
                server.OnSendToServer(fromUser, msg);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
