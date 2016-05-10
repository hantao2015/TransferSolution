using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransferClientLib
{
    public class TransferEngineFactory
    {
        public static ITransferEngine CreateTransferEngine()
        {
            return new TransferEngine();
        }
    }
}
