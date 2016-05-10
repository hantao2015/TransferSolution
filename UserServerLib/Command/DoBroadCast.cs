using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;

namespace UserServerLib.ommand
{
    public class DoBroadCast : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var fromUserData = Encoding.UTF8.GetBytes(session.SessionID);
                var msg = Tool.SerializeHelp.Deserialize<Message>(requestInfo.Parameters[0]);
                byte[] msgdata = requestInfo.Parameters[0];
                var server = session.AppServer as UserServer;
                var sessions = server.GetAllSessions();
                foreach (var item in sessions)
                {
                    item.SendData(UserOP.DoBroadCast, fromUserData, msgdata);
                }
                var fromUser = UserManage.Instance.GetUser(session.SessionID);
                server.OnBroadCast(fromUser, msg);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
