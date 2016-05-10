using System;
using System.Text;
using UserCommon;

namespace UserClientLib.Command
{
    internal class DoCreateChat : UserCommandBase
    {
        public override void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo)
        {
            try
            {
                var remoteAddr = Encoding.UTF8.GetString(commandInfo.Data[0]);
                var remotePort = Convert.ToInt32(Encoding.UTF8.GetString(commandInfo.Data[1]));
                session.OnCreateChat(remoteAddr, remotePort);
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
                return UserOP.DoCreateChat.ToString();
            }
        }
    }
}
