using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;

namespace UserServerLib.ommand
{
    public class DoLogOut : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var server = session.AppServer as UserServer;
                var sessions = server.GetSessions(t => t.SessionID != session.SessionID);
                foreach (var item in sessions)
                {
                    item.UserLeave(session.SessionID);
                }
                var user = UserManage.Instance.GetUser(session.SessionID);
                if (user == null)
                    return;
                server.OnUserLeave(user);
                UserManage.Instance.DelUser(session.SessionID);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
