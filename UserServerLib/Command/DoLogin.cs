using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Text;
using UserCommon;
using System.Linq;

namespace UserServerLib.ommand
{
    public class DoLogin : CommandBase<UserSession, MultiParamBinaryRequestInfo>
    {
        public override void ExecuteCommand(UserSession session, MultiParamBinaryRequestInfo requestInfo)
        {
            try
            {
                var userId = Encoding.UTF8.GetString(requestInfo.Parameters[0]);
                var password = Encoding.UTF8.GetString(requestInfo.Parameters[1]);
                bool result = true;
                string ErrorMessage = "登陆成功";
                if (userId == "zs")
                {
                    result = false;
                    ErrorMessage = "系统不允许zs登陆";
                }
                else
                {
                    //TODO:getUserFromdatabase
                    var newUser = new User();
                    newUser.SessionId = session.SessionID;
                    newUser.UserId = userId;
                    if (userId == "ls")
                    {
                        newUser.UserName = "李四";
                    }
                    else if (userId == "we")
                    {
                        newUser.UserName = "王二";
                    }
                    else if (userId == "mz")
                    {
                        newUser.UserName = "麻子";
                    }
                    else if (userId == "zl")
                    {
                        newUser.UserName = "赵六";
                    }
                    else
                    {
                        newUser.UserName = "用户名不详";
                    }
                    UserManage.Instance.AddUser(newUser);
                    session.SendData(UserOP.DoGetUser, Tool.SerializeHelp.Serialize<User>(newUser));
                    session.SendData(UserOP.DoGetAllUser, Tool.SerializeHelp.SerializeList<User>(UserManage.Instance.GetAllUser().ToList()));

                    var server = session.AppServer as UserServer;
                    var sessions = server.GetSessions(t => t.SessionID != newUser.SessionId).ToList();
                    foreach (var item in sessions)
                    {
                        item.UserJoin(newUser);
                    }
                    server.OnUserJoin(newUser);
                }
                session.SendData(UserOP.DoLoginResult, Encoding.UTF8.GetBytes(result.ToString()), Encoding.UTF8.GetBytes(ErrorMessage));
            }
            catch (System.Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error(ex.Message);
            }
        }
    }
}
