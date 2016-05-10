using UserClientLib;
using UserClientLib.Command;
using UserCommon;
namespace UserClientLib.Command
{
    internal class DoGetAllUser : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            UserManage.Instance.CleareUsers();
            var users = Tool.SerializeHelp.DeserializeList<User>(commandInfo.Data[0]);
            foreach (var user in users)
            {
                UserManage.Instance.AddUser(user);
            }
            session.OnGetAllUser(users);
        }

        public override string Name
        {
            get
            {
                return UserOP.DoGetAllUser.ToString();
            }
        }
    }
}

