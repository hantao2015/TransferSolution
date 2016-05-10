using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;

namespace UserServerLib.ommand
{
    public class DoCreateChat : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                session.SendData(UserOP.DoCreateChat, Encoding.UTF8.GetBytes(session.RemoteEndPoint.Address.ToString()), Encoding.UTF8.GetBytes(session.RemoteEndPoint.Port.ToString()));
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
