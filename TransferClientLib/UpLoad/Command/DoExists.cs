using System;
using System.Text;
using Tool;
using TransferClientLib.UpLoad;
using TransferCommon;
namespace TransferClientLib.UpLoad.Command
{
    internal class DoExists : UpLoadCommandBase
    {
        public override void ExecuteCommand(UpLoadClientEngine session, TransferCommandInfo commandInfo)
        {
            bool result = session.OnFileExist();
            if (result)
            {
                byte[] data = SerializeHelp.Serialize<UpLoadInfo>(session.UpLoadInfo);
                session.SendData(UpLoadOP.DoCover, data, 0, data.Length);
            }
            else
            {
                session.DoEnd();
            }
        }

        public override string Name
        {
            get
            {
                return UpLoadOP.DoExists.ToString();
            }
        }
    }
}
