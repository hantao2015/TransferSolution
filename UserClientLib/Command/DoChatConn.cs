using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserCommon;

namespace UserClientLib.Command
{
    internal class DoChatConn : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, UserCommandInfo commandInfo)
        {
            try
            {
                //var strfromUser = Encoding.UTF8.GetString(commandInfo.Data[0]);
                //var fromUser = UserManage.Instance.GetUser(strfromUser);
                //if (fromUser == null)
                //    return;
                //var msg = Tool.SerializeHelp.Deserialize<Message>(commandInfo.Data[1]);
                //session.UserEvents.OnReceiveMsg(new ReceiveMsgEventArgs(fromUser, msg));
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
                return UserOP.DoChatConn.ToString();
            }
        }
    }
}
