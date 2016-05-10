using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using TransferCommon;

namespace TransferClientLib.DownLoad
{
    public class DownLoadEngine : IDisposable
    {
        protected IClientCommandReader<TransferCommandInfo> CommandReader { get; private set; }
        public delegate void TransferCompleteEventHandler(EventArgs e);
        public delegate void TransferErrorEventHandler(TransferErrorEventArgs e);
        public delegate void TransferStepEventHandler(TransferStepEventArgs e);
        public event TransferCompleteEventHandler TransferComplete;
        public event TransferErrorEventHandler TransferError;
        public event TransferStepEventHandler TransferStep;
        private string _fileName;
        private string _saveName;
        private TcpClientSession Client;
        private Dictionary<string, ICommand<DownLoadEngine, TransferCommandInfo>> m_CommandDict = new Dictionary<string, ICommand<DownLoadEngine, TransferCommandInfo>>(StringComparer.OrdinalIgnoreCase);
        private FileStream m_fileStream;
        private AutoResetEvent m_OpenedEvent = new AutoResetEvent(false);
        private AutoResetEvent m_EndEvent = new AutoResetEvent(false);
        private int PacketSize = 1024 * 30;
        private byte[] readBuffer;
        private long TransferedLength = 0L;
        private int TransferStatus = 0;
        //??0:初始化状态，1：CheckExist,3,OnUpLoad,开始上传，4，OnDoData，数据传输中，5，数据传输完成,6,已结束的标志
        private long TransferTotalLength = 0L;
        public void BeginDownLoad()
        {
            try
            {
                if (this.TransferStatus != 0)
                {
                    throw new Exception("状态错误");
                }
                this.Client.Connect();
                if (!this.m_OpenedEvent.WaitOne(5000))
                {
                    throw new Exception("连接失败");
                }
                //this.SendMessage(UpLoadOP.CheckExist, this._saveName);
            }
            catch (Exception exception)
            {
                //this.OnTransferError(new TransferErrorEventArgs(exception));
            }
        }
    }
}
