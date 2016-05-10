using System.Text;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;
namespace UserClientLib.Command
{
    internal class DoLoginResult : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            var result = bool.Parse(Encoding.UTF8.GetString(commandInfo.Data[0]));
            var errormsg = Encoding.UTF8.GetString(commandInfo.Data[1]);
            session.OnLoginComplete(result, errormsg);
            if (!result)//登陆失败
            {
                session.Client.Close();
            }
            else
            {
            }
        }

        public override string Name
        {
            get
            {
                return UserOP.DoLoginResult.ToString();
            }
        }
    }
}

