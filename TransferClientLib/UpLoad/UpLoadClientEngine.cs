using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Tool;
using TransferClientLib.UpLoad.Command;
using TransferCommon;
namespace TransferClientLib.UpLoad
{
    internal class UpLoadClientEngine : IDisposable
    {
        internal FileUploadingEvents FileUploadingEvents;
        protected IClientCommandReader<TransferCommandInfo> CommandReader { get; private set; }
        private Dictionary<string, ICommand<UpLoadClientEngine, TransferCommandInfo>> m_CommandDict
            = new Dictionary<string, ICommand<UpLoadClientEngine, TransferCommandInfo>>(StringComparer.OrdinalIgnoreCase);
        string _fileName;
        string _saveName;
        internal UpLoadInfo UpLoadInfo;
        internal FileStream m_fileStream;
        internal int PacketSize = 1024 * 30;
        internal byte[] readBuffer;
        internal int StatusCode = 0;
        internal TcpClientSession Client;
        internal AutoResetEvent m_OpenedEvent = new AutoResetEvent(false);
        internal AutoResetEvent m_EndEvent = new AutoResetEvent(false);
        internal AutoResetEvent m_StopEvent = new AutoResetEvent(false);
        internal object lockobj = new object();
        public UpLoadClientEngine(FileUploadingEvents _FileUploadingEvents)
        {
            this.FileUploadingEvents = _FileUploadingEvents;
        }

        public void Init(EndPoint endpoint, string fileName, string saveName)
        {
            string str = ConfigurationManager.AppSettings["PacketSize"];
            if (!string.IsNullOrWhiteSpace(str))
            {
                this.PacketSize = Convert.ToInt32(str);
            }
            this.readBuffer = new byte[this.PacketSize];
            this._fileName = fileName;
            this._saveName = saveName;
            UpLoadCommandBase[] baseArray = new UpLoadCommandBase[] { 
                new TransferClientLib.UpLoad.Command.DoUpLoad(), 
                new TransferClientLib.UpLoad.Command.DoExists(), 
                new TransferClientLib.UpLoad.Command.DoStop() };
            foreach (UpLoadCommandBase base2 in baseArray)
            {
                this.m_CommandDict.Add(base2.Name, base2);
            }
            TcpClientSession session = new AsyncTcpSession(endpoint, 1024 * 3);
            session.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(this.client_Error);
            session.DataReceived += new EventHandler<DataEventArgs>(this.client_DataReceived);
            session.Connected += new EventHandler(this.client_Connected);
            session.Closed += new EventHandler(this.client_Closed);
            this.Client = session;
            this.CommandReader = new UpLoadReader(this);
        }
        public void StartUpLoad(ref string fileProjectId)
        {
            try
            {
                this.Client.Connect();
                if (!this.m_OpenedEvent.WaitOne(5000))
                {
                    throw new Exception("连接失败");
                }
                this.m_fileStream = new FileStream(this._fileName, FileMode.Open, FileAccess.Read);
                UpLoadInfo configInfo = null;
                string configName = _fileName + Config.ConfigSuffix;
                if (File.Exists(configName))
                {
                    try
                    {
                        configInfo = SerializeHelp.DeserializeFromFile<UpLoadInfo>(configName);
                        //验证配置文件正确性
                        //TODO:
                    }
                    catch (Exception ex)
                    {
                        configInfo = null;
                        ex.ToString();
                        //logxxx
                        File.Delete(configName);
                    }
                }
                if (configInfo == null)
                {
                    configInfo = new UpLoadInfo();
                    configInfo.FileName = _fileName;
                    configInfo.FileSize = this.m_fileStream.Length;
                    configInfo.ProjectId = Guid.NewGuid().ToString();
                    configInfo.SaveName = _saveName;
                    configInfo.TransferedLength = 0;
                    configInfo.TransferLength = this.m_fileStream.Length;
                    configInfo.TransferPos = 0;
                    SerializeHelp.SerializeToFile<UpLoadInfo>(configInfo, configName);
                }
                UpLoadInfo = configInfo;
                fileProjectId = UpLoadInfo.ProjectId;
                byte[] data = SerializeHelp.Serialize<UpLoadInfo>(UpLoadInfo);
                this.SendData(UpLoadOP.DoUpLoad, data, 0, data.Length);
            }
            catch (Exception exception)
            {
                this.OnTransferError(new TransferErrorEventArgs(exception));
            }
        }

        private void ExecuteCommand(TransferCommandInfo commandInfo)
        {
            ICommand<UpLoadClientEngine, TransferCommandInfo> command;
            if (this.m_CommandDict.TryGetValue(commandInfo.Key, out command))
            {
                command.ExecuteCommand(this, commandInfo);
            }
        }

        private void m_OnDataReceived(byte[] data, int offset, int length)
        {
            while (true)
            {
                if (this.CommandReader == null)
                    return;
                int rest;
                TransferCommandInfo commandInfo = this.CommandReader.GetCommandInfo(data, offset, length, out rest);
                if (this.CommandReader.NextCommandReader != null)
                {
                    this.CommandReader = this.CommandReader.NextCommandReader;
                }
                if (commandInfo != null)
                {
                    this.ExecuteCommand(commandInfo);
                }
                if (rest <= 0)
                {
                    return;
                }
                offset = (offset + length) - rest;
                length = rest;
            }
        }

        private void client_Closed(object sender, EventArgs e)
        {
            this.m_EndEvent.Set();
        }

        private void client_Connected(object sender, EventArgs e)
        {
            this.m_OpenedEvent.Set();
        }

        private void client_DataReceived(object sender, DataEventArgs e)
        {
            this.m_OnDataReceived(e.Data, e.Offset, e.Length);
        }

        private void client_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
        }

        public void Dispose()
        {
            if (FileUploadingEvents != null)
                FileUploadingEvents = null;
            this.CommandReader = null;
            this.m_CommandDict.Clear();
            this._fileName = string.Empty;
            this._saveName = string.Empty;
            UpLoadInfo = null;
            if (this.m_fileStream != null)
            {
                this.m_fileStream.Close();
                this.m_fileStream = null;
            }
            TcpClientSession client = this.Client;
            if (client != null)
            {
                client.Error -= new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(this.client_Error);
                client.DataReceived -= new EventHandler<DataEventArgs>(this.client_DataReceived);
                client.Connected -= new EventHandler(this.client_Connected);
                client.Closed -= new EventHandler(this.client_Closed);
                if (client.IsConnected)
                {
                    client.Close();
                }
                this.Client = null;
            }
            this.readBuffer = null;
            if (m_OpenedEvent != null)
            {
                this.m_OpenedEvent.Reset();
                m_OpenedEvent = null;
            }
            if (this.m_EndEvent != null)
            {
                this.m_EndEvent.Reset();
                this.m_EndEvent = null;
            }
        }


        internal void OnTransferStart()
        {
            if (this.FileUploadingEvents.OnTransferStart != null)
            {
                this.FileUploadingEvents.OnTransferStart(new TransferStartEventArgs(this.UpLoadInfo.ProjectId));
            }
        }
        internal bool OnFileExist()
        {
            if (this.FileUploadingEvents.OnFileExist != null)
            {
                return this.FileUploadingEvents.OnFileExist(new EventArgs());
            }
            else
                this.OnTransferError(new TransferErrorEventArgs(new Exception("存在相同文件")));
            return false;
        }


        internal void OnTransferError(TransferErrorEventArgs e)
        {
            if (this.FileUploadingEvents.OnTransferError != null)
            {
                this.FileUploadingEvents.OnTransferError(e);
            }
            this.Dispose();
        }

        internal void OnTransferStep(long _TotalLen, long _TransferLen, int _CurrentPacket)
        {
            if (this.FileUploadingEvents.OnTransferStep != null)
            {
                this.FileUploadingEvents.OnTransferStep(new TransferStepEventArgs(_TotalLen, _TransferLen, _CurrentPacket));
            }
        }

        internal void SendData(UpLoadOP opCode, byte[] data, int offset, int length)
        {
            if (this.Client == null)
                return;
            byte[] senddata = new byte[length + 5];
            senddata[0] = 0;
            senddata[1] = (byte)((int)opCode / 256);
            senddata[2] = (byte)((int)opCode % 256);
            senddata[3] = (byte)(length / 256);
            senddata[4] = (byte)(length % 256);
            Buffer.BlockCopy(data, offset, senddata, 5, length);
            this.Client.Send(senddata, 0, senddata.Length);
        }

        internal void Pause()
        {
        }

        internal void Continue()
        {
        }

        internal void DoEnd()
        {
            if (StatusCode != 1 && StatusCode != 2)
            {
                string configName = _fileName + Config.ConfigSuffix;
                if (File.Exists(configName))
                {
                    File.Delete(configName);
                }
            }
            if (this.FileUploadingEvents.OnTransferComplete != null)
            {
                this.FileUploadingEvents.OnTransferComplete(new EventArgs());
            }
            this.Dispose();
        }

        internal void StopUpLoad()
        {
            try
            {
                this.StatusCode = 1;
                new Action(() =>
                {
                    this.m_StopEvent.WaitOne();
                    this.SendData(UpLoadOP.DoStop, new byte[] { 1 }, 0, 1);
                    this.m_StopEvent.WaitOne(5000);
                    lock (this.lockobj)
                    {
                        string configName = _fileName + Config.ConfigSuffix;
                        SerializeHelp.SerializeToFile<UpLoadInfo>(this.UpLoadInfo, configName);
                        this.DoEnd();
                    }
                }).BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                this.OnTransferError(new TransferErrorEventArgs(ex));
            }
        }
    }


    internal class FileUploadingEvents : IFileUploadingEvents
    {
        internal TransferEventHandler<TransferStartEventArgs> OnTransferStart;

        internal BTransferEventHandler<EventArgs> OnFileExist;

        internal TransferEventHandler<EventArgs> OnTransferComplete;

        internal TransferEventHandler<TransferErrorEventArgs> OnTransferError;

        internal TransferEventHandler<TransferStepEventArgs> OnTransferStep;
        public event TransferEventHandler<TransferStartEventArgs> TransferStart
        {
            add { OnTransferStart = (TransferEventHandler<TransferStartEventArgs>)System.Delegate.Combine(OnTransferStart, value); }
            remove { OnTransferStart = (TransferEventHandler<TransferStartEventArgs>)System.Delegate.Remove(OnTransferStart, value); }
        }
        public event BTransferEventHandler<EventArgs> FileExist
        {
            add { OnFileExist = (BTransferEventHandler<EventArgs>)System.Delegate.Combine(OnFileExist, value); }
            remove { OnFileExist = (BTransferEventHandler<EventArgs>)System.Delegate.Remove(OnFileExist, value); }
        }
        public event TransferEventHandler<EventArgs> TransferComplete
        {
            add { OnTransferComplete = (TransferEventHandler<EventArgs>)System.Delegate.Combine(OnTransferComplete, value); }
            remove { OnTransferComplete = (TransferEventHandler<EventArgs>)System.Delegate.Remove(OnTransferComplete, value); }
        }

        public event TransferEventHandler<TransferErrorEventArgs> TransferError
        {
            add { OnTransferError = (TransferEventHandler<TransferErrorEventArgs>)System.Delegate.Combine(OnTransferError, value); }
            remove { OnTransferError = (TransferEventHandler<TransferErrorEventArgs>)System.Delegate.Remove(OnTransferError, value); }
        }

        public event TransferEventHandler<TransferStepEventArgs> TransferStep
        {
            add { OnTransferStep = (TransferEventHandler<TransferStepEventArgs>)System.Delegate.Combine(OnTransferStep, value); }
            remove { OnTransferStep = (TransferEventHandler<TransferStepEventArgs>)System.Delegate.Remove(OnTransferStep, value); }
        }
    }

}
