using System;
using Tool;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;

namespace UserClientLib.Command
{
    internal class DoUserLeave : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var leaveUserSessionId = System.Text.Encoding.UTF8.GetString(commandInfo.Data[0]);
                var leaveUser = UserManage.Instance.GetUser(leaveUserSessionId);
                session.UserEvents.OnUserLeave(new UserLeaveEventArgs(leaveUser));
                UserManage.Instance.DelUser(leaveUserSessionId);
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
                return UserOP.DoUserLeave.ToString();
            }
        }
    }
}
