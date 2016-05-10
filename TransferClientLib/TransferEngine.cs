using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TransferClientLib.UpLoad;

namespace TransferClientLib
{
    internal class TransferEngine : ITransferEngine
    {
        private FileUploadingEvents _FileUploadingEvents;
        public IFileUploadingEvents FileUploadingEvents { get { return _FileUploadingEvents; } }
        UpLoadClientEngine upLoadEngine;
        public TransferEngine()
        {
            _FileUploadingEvents = new FileUploadingEvents();
        }
        public void StartUpLoad(string fileName, string saveName, ref string fileProjectId)
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Parse("139.196.188.134"), 2021);
            upLoadEngine = new UpLoadClientEngine(_FileUploadingEvents);
            upLoadEngine.Init(endpoint, fileName, saveName);
            upLoadEngine.StartUpLoad(ref fileProjectId);
        }

        public void StopUpLoad()
        {
            upLoadEngine.StopUpLoad();
        }
    }
}
