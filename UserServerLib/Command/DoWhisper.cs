using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;

namespace UserServerLib.ommand
{
    public class DoWhisper : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var fromUserSessionId = session.SessionID;
                var touserSessionId = Encoding.UTF8.GetString(requestInfo.Parameters[0]);
                var msg = Tool.SerializeHelp.Deserialize<Message>(requestInfo.Parameters[1]);

                var toSession = session.AppServer.GetSessionByID(touserSessionId) as UserSession;
                byte[] msgdata = requestInfo.Parameters[1];
                byte[] fromSessionIdData = Encoding.UTF8.GetBytes(fromUserSessionId);
                toSession.SendData(UserOP.DoWhisper, fromSessionIdData, msgdata);

                var server = session.AppServer as UserServer;
                var fromUser = UserManage.Instance.GetUser(fromUserSessionId);
                var toUser = UserManage.Instance.GetUser(touserSessionId);
                server.OnUserWhisper(fromUser, toUser, msg);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
