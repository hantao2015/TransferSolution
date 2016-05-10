using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;
using System.Threading;
using Tool;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;

namespace UserClientLib.Command
{
    internal class DoUserStartCreateChat : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var toUserSessionId = Encoding.UTF8.GetString(commandInfo.Data[0]);
                var toUserAddr = System.Text.Encoding.UTF8.GetString(commandInfo.Data[1]);
                var toUserPort = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(commandInfo.Data[2]));
                session.OnUserStartCreateChat(toUserSessionId, toUserAddr, toUserPort);
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
                return UserOP.DoUserStartCreateChat.ToString();
            }
        }
    }
}
