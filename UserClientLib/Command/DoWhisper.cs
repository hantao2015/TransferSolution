using System;
using Tool;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;

namespace UserClientLib.Command
{
    internal class DoWhisper : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var fromUserSessionId = System.Text.Encoding.UTF8.GetString(commandInfo.Data[0]);
                var msg = Tool.SerializeHelp.Deserialize<Message>(commandInfo.Data[1]);
                var fromUser = UserManage.Instance.GetUser(fromUserSessionId);
                session.UserEvents.OnUserWhisper(new UserWhisperEventArgs(fromUser, msg));
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
                return UserOP.DoWhisper.ToString();
            }
        }
    }
}
