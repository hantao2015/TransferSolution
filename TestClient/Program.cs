using System;
using System.IO;
using System.Net;
using TransferClientLib;
using TransferClientLib.UpLoad;
namespace TestClient
{
    class Program
    {
        private static string fileName1 = @"D:\Soft\OPSystem\cn_win_srv_2003_enterprise_vl[NRMEVOL_CN].iso";
        private static string fileName2 = @"D:\Soft\OPSystem\GhostXP_SP3.iso";
        private static string fileName3 = @"D:\Soft\OPSystem\YLMF_GHOSTWIN7SP1_X64_YN2014.iso";
        private static void Main(string[] args)
        {
            ITransferEngine engine = TransferClientLib.TransferEngineFactory.CreateTransferEngine();
            engine.FileUploadingEvents.FileExist += FileUploadingEvents_FileExist;
            engine.FileUploadingEvents.TransferComplete += FileUploadingEvents_TransferComplete;
            engine.FileUploadingEvents.TransferError += FileUploadingEvents_TransferError;
            engine.FileUploadingEvents.TransferStep += FileUploadingEvents_TransferStep;
            while (true)
            {
                string read = Console.ReadLine();
                if (read == "q")
                {
                    return;
                }
                else if (read == "1")
                {
                    engine.BeginUpLoad(fileName1, Path.GetFileName(fileName1));
                }
                else if (read == "2")
                {
                    engine.BeginUpLoad(fileName2, Path.GetFileName(fileName2));
                }
                else if (read == "3")
                {
                    engine.BeginUpLoad(fileName3, Path.GetFileName(fileName3));
                }
            }
        }

        static void FileUploadingEvents_TransferStep(TransferStepEventArgs e)
        {
            Console.WriteLine(string.Format("已上传大小{0},本次上传{1},总计大小{2}", e.TransferLen, e.CurrentPacket, e.TotalLen));
        }

        static void FileUploadingEvents_TransferError(TransferErrorEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
        }

        static void FileUploadingEvents_TransferComplete(EventArgs e)
        {
            Console.WriteLine("上传完成");
        }

        static bool FileUploadingEvents_FileExist(EventArgs e)
        {
            Console.WriteLine("存在相同的文件，是否覆盖？");
            string str = Console.ReadLine();
            return (str == "y");
        }
    }
}
