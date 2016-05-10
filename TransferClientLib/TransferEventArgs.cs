
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using TransferClientLib.UpLoad;
using TransferCommon;
namespace TransferClientLib
{
    public delegate void TransferEventHandler<TEventArgs>(TEventArgs e);
    public delegate bool BTransferEventHandler<TEventArgs>(TEventArgs e);
    public class TransferStepEventArgs : EventArgs
    {
        public TransferStepEventArgs(long _TotalLen, long _TransferLen, int _CurrentPacket)
        {
            this.TotalLen = _TotalLen;
            this.TransferLen = _TransferLen;
            this.CurrentPacket = _CurrentPacket;
        }

        public int CurrentPacket { get; set; }

        public long TotalLen { get; set; }

        public long TransferLen { get; set; }
    }
    public class TransferErrorEventArgs : EventArgs
    {
        public TransferErrorEventArgs(System.Exception exception)
        {
            this.Exception = exception;
        }

        public System.Exception Exception { get; private set; }
    }
    public class TransferStartEventArgs : EventArgs
    {
        public TransferStartEventArgs(string projectId)
        {
            this.ProjectId = projectId;
        }

        public string ProjectId { get; private set; }
    }
}
