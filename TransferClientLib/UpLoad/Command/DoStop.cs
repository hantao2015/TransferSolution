using System;
using System.Text;
using TransferClientLib.UpLoad;
using TransferCommon;
namespace TransferClientLib.UpLoad.Command
{
    internal class DoStop : UpLoadCommandBase
    {
        public override void ExecuteCommand(UpLoadClientEngine session, TransferCommandInfo commandInfo)
        {
            try
            {
                session.m_StopEvent.Set();
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("DoData").Error(ex.Message);
            }
        }

        public override string Name
        {
            get
            {
                return UpLoadOP.DoStop.ToString();
            }
        }
    }
}
