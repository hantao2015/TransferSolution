using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransferServerLib
{
    public delegate void TransferEventHandler<TEventArgs>(TEventArgs e);
    public class TransferStepEventArgs
    {
        public TransferStepEventArgs(string _ProjectId, long _TotalLen, long _TransferLen, int _CurrentPacket)
        {
            this.ProjectId = _ProjectId;
            this.TotalLen = _TotalLen;
            this.TransferLen = _TransferLen;
            this.CurrentPacket = _CurrentPacket;
        }

        public string ProjectId { get; private set; }

        public int CurrentPacket { get; private set; }

        public long TotalLen { get; private set; }

        public long TransferLen { get; private set; }
    }
    public class StopTransferEventArgs
    {
        public string ProjectId { get; private set; }
        public StopTransferEventArgs(string _ProjectId)
        {
            this.ProjectId = _ProjectId;
        }
    }
    public class TransferCompleteEventArgs
    {
        public string ProjectId { get; private set; }
        public TransferCompleteEventArgs(string _ProjectId)
        {
            this.ProjectId = _ProjectId;
        }
    }
    public class StartTransferEventArgs : EventArgs
    {
        public StartTransferEventArgs(string _ProjectId, string _FileName, string _SaveName, long _FileSize, long _TransferPos, long _TransferLength)
        {
            this.ProjectId = _ProjectId;
            this.FileName = _FileName;
            this.SaveName = _SaveName;
            this.FileSize = _FileSize;
            this.TransferPos = _TransferPos;
            this.TransferLength = _TransferLength;
        }

        public string ProjectId { get; private set; }
        public string FileName { get; private set; }
        public string SaveName { get; private set; }
        public long FileSize { get; private set; }
        public long TransferPos { get; private set; }
        public long TransferLength { get; private set; }
    }
}
