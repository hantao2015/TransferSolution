using System;
using Tool;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;

namespace UserClientLib.Command
{
    internal class DoUserJoin : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var newuser = SerializeHelp.Deserialize<User>(commandInfo.Data[0]);
                UserManage.Instance.AddUser(newuser);
                session.UserEvents.OnUserJoin(new UserJoinEventArgs(newuser));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override string Name
        {
            get
            {
                return UserOP.DoUserJoin.ToString();
            }
        }
    }
}
