using System;
using System.IO;
using System.Text;
using System.Threading;
using Tool;
using TransferCommon;
using TransferServerLib;
namespace TransferServerLib.UpLoad
{
    public class UpLoadServerEngine : IDisposable
    {
        internal AutoResetEvent m_EndEvent = new AutoResetEvent(false);
        internal object lockobj = new object();
        internal FileStream m_fileStream;
        internal UpLoadInfo UpLoadInfo;
        public event TransferEventHandler<TransferStepEventArgs> TransferStep;
        public event TransferEventHandler<TransferCompleteEventArgs> TransferComplete;
        public event TransferEventHandler<StartTransferEventArgs> StartTransfer;
        public event TransferEventHandler<StopTransferEventArgs> StopTransfer;
        TransferSession Session;
        public UpLoadServerEngine(TransferSession session)
        {
            this.Session = session;
        }

        internal void OnTransferStep(long _TotalLen, long _TransferLen, int _CurrentPacket)
        {
            if (this.TransferStep != null)
            {
                this.TransferStep(new TransferStepEventArgs(UpLoadInfo.ProjectId, _TotalLen, _TransferLen, _CurrentPacket));
            }
        }

        internal void OnTransferComplete(TransferCompleteEventArgs eventArgs)
        {
            if (TransferComplete != null)
                TransferComplete(eventArgs);
        }

        internal void OnStartTransfer(UpLoadInfo upLoadConfigInfo)
        {
            if (this.StartTransfer != null)
            {
                this.StartTransfer(new StartTransferEventArgs(upLoadConfigInfo.ProjectId,
                    upLoadConfigInfo.FileName, upLoadConfigInfo.SaveName, upLoadConfigInfo.FileSize, upLoadConfigInfo.TransferPos,
                    upLoadConfigInfo.TransferLength));
            }
        }

        internal void OnStopTransfer(StopTransferEventArgs eventArgs)
        {
            if (StopTransfer != null)
                StopTransfer(eventArgs);
        }

        internal void SendData(UpLoadOP opCode, byte[] data, int offset, int length)
        {
            byte[] dst = new byte[length + 4];
            dst[0] = (byte)((int)opCode / 256);
            dst[1] = (byte)((int)opCode % 256);
            dst[2] = (byte)(data.Length / 256);
            dst[3] = (byte)(data.Length % 256);
            Buffer.BlockCopy(data, offset, dst, 4, length);
            this.Session.Send(dst, 0, dst.Length);
        }

        internal void DoEnd()
        {
            this.m_fileStream.Close();
            string saveName = Path.GetFullPath(this.UpLoadInfo.SaveName);
            string saveTempName = saveName + TransferCommon.Config.TempSuffix;
            string configName = saveName + TransferCommon.Config.ConfigSuffix;
            File.Move(saveTempName, saveName);
            if (File.Exists(configName))
            {
                File.Delete(configName);
            }
            this.OnTransferComplete(new TransferCompleteEventArgs(this.UpLoadInfo.ProjectId));
            this.Dispose();
        }

        internal void DoUpLoad(UpLoadInfo ClientConfigInfo)
        {
            string saveName = Path.GetFullPath(ClientConfigInfo.SaveName);
            string saveTempName = saveName + TransferCommon.Config.TempSuffix;
            TransferCommon.UpLoadInfo ServerConfigInfo = null;
            string configName = ClientConfigInfo.SaveName + TransferCommon.Config.ConfigSuffix;
            string saveNamePath = Path.GetDirectoryName(saveName);
            if (!Directory.Exists(saveNamePath))
                Directory.CreateDirectory(saveNamePath);
            if (File.Exists(saveTempName))
            {
                this.m_fileStream = new FileStream(saveTempName, FileMode.Open, FileAccess.Write);
                if (File.Exists(configName))
                {
                    ServerConfigInfo = SerializeHelp.DeserializeFromFile<UpLoadInfo>(configName);
                }
            }
            else
            {
                this.m_fileStream = new FileStream(saveTempName, FileMode.Create, FileAccess.Write);
                this.m_fileStream.SetLength(ClientConfigInfo.FileSize);
            }
            if (ServerConfigInfo == null)
            {
                ServerConfigInfo = ClientConfigInfo;
                SerializeHelp.SerializeToFile<UpLoadInfo>(ServerConfigInfo, configName);
            }
            //如果客户端传来的已上传大小与服务端的已上传大小不一致
            //if(ServerConfigInfo.TransferedLength!= ClientConfigInfo.TransferedLength)
            //{

            //}
            //如果客户端传来的文件大小与服务端的文件大小不一致
            //to do something
            this.UpLoadInfo = ServerConfigInfo;
            this.m_fileStream.Position = ServerConfigInfo.TransferPos + ServerConfigInfo.TransferedLength;
            byte[] data = SerializeHelp.Serialize<UpLoadInfo>(ServerConfigInfo);
            this.SendData(UpLoadOP.DoUpLoad, data, 0, data.Length);
        }

        public void Dispose()
        {
            if (m_EndEvent != null)
            {
                m_EndEvent.Reset();
                m_EndEvent = null;
            }
            if (this.m_fileStream != null)
            {
                this.m_fileStream.Close();
                this.m_fileStream = null;
            }
            if (UpLoadInfo != null)
                UpLoadInfo = null;
            if (TransferStep != null)
                TransferStep = null;
            if (TransferComplete != null)
                TransferComplete = null;
            if (StartTransfer != null)
                StartTransfer = null;
            if (StopTransfer != null)
                StopTransfer = null;
            if (this.Session != null)
            {
                this.Session.Close();
                this.Session = null;
            }
        }
    }
}
