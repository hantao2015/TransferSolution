using System;
using System.Text;
using UserClientLib;
using UserClientLib.Command;
using UserCommon;
namespace UserClientLib.Command
{
    internal class DoUserChat : ChatCommandBase
    {
        public override void ExecuteCommand(ChatEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var msg = Tool.SerializeHelp.Deserialize<Message>(commandInfo.Data[0]);
                session.ChatEvents.OnUserChat(msg);
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
                return UserOP.DoUserChat.ToString();
            }
        }
    }
}

