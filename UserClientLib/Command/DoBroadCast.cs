using System;
using System.Text;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;
namespace UserClientLib.Command
{
    internal class DoBroadCast : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var strfromUser = Encoding.UTF8.GetString(commandInfo.Data[0]);
                var fromUser = UserManage.Instance.GetUser(strfromUser);
                if (fromUser == null)
                    return;
                var msg = Tool.SerializeHelp.Deserialize<Message>(commandInfo.Data[1]);
                session.UserEvents.OnReceiveMsg(new ReceiveMsgEventArgs(fromUser, msg));
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
                return UserOP.DoBroadCast.ToString();
            }
        }
    }
}

