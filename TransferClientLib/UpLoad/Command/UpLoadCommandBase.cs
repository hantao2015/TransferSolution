using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransferClientLib.UpLoad.Command;

namespace TransferClientLib.UpLoad.Command
{
    internal abstract class UpLoadCommandBase : ICommand<UpLoadClientEngine, TransferCommandInfo>, ICommand
    {
        internal UpLoadCommandBase()
        {
        }

        public abstract void ExecuteCommand(UpLoadClientEngine session, TransferCommandInfo commandInfo);

        public abstract string Name { get; }
    }
}
