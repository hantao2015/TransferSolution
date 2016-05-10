using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserClientLib.Command
{
    internal abstract class UserCommandBase : ICommand<UserClientEngine, BaseCommandInfo>, ICommand
    {
        internal UserCommandBase()
        {
        }

        public abstract void ExecuteCommand(UserClientEngine session, BaseCommandInfo commandInfo);

        public abstract string Name { get; }
    }

    internal abstract class ChatCommandBase : ICommand<ChatEngine, BaseCommandInfo>, ICommand
    {
        internal ChatCommandBase()
        {
        }

        public abstract void ExecuteCommand(ChatEngine session, BaseCommandInfo commandInfo);

        public abstract string Name { get; }
    }
}
