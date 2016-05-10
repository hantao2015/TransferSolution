using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransferClientLib
{
    public interface ITransferEngine
    {
        /// <summary>
        /// 该事件接口暴露了所有正在上传的文件（夹）的实时状态。
        /// </summary>
        IFileUploadingEvents FileUploadingEvents { get; }
        void StartUpLoad(string fileName, string saveName, ref string fileProjectId);

        void StopUpLoad();
    }
    public interface IFileUploadingEvents
    {
        event BTransferEventHandler<EventArgs> FileExist;
        event TransferEventHandler<EventArgs> TransferComplete;
        event TransferEventHandler<TransferErrorEventArgs> TransferError;
        event TransferEventHandler<TransferStepEventArgs> TransferStep;
        event TransferEventHandler<TransferStartEventArgs> TransferStart;
    }
}
