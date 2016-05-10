using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;

namespace UserServerLib.ommand
{
    public class DoUserStartCreateChat : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var touserSessionId = Encoding.UTF8.GetString(requestInfo.Parameters[0]);
                var toUserSession = session.AppServer.GetSessionByID(touserSessionId);
                byte[] toUserAddrData = Encoding.UTF8.GetBytes(toUserSession.RemoteEndPoint.Address.ToString());
                byte[] toUserPortData = Encoding.UTF8.GetBytes(toUserSession.RemoteEndPoint.Port.ToString());
                session.SendData(UserOP.DoUserStartCreateChat,requestInfo.Parameters[0], toUserAddrData, toUserPortData);
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
