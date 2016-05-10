using System;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;
namespace UserClientLib.Command
{
    internal class DoGetUser : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var user = Tool.SerializeHelp.Deserialize<User>(commandInfo.Data[0]);
                session.OnGetUser(user);
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
                return UserOP.DoGetUser.ToString();
            }
        }
    }
}

