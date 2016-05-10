using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserClientLib
{
    public class BaseCommandInfo : ICommandInfo
    {
        public BaseCommandInfo()
        {
        }

        public BaseCommandInfo(string key)
        {
            this.Key = key;
        }

        public BaseCommandInfo(string key, string text)
        {
            this.Key = key;
            this.Text = text;
        }

        public BaseCommandInfo(string key, byte[][] data)
        {
            this.Key = key;
            this.Data = data;
        }

        public byte[][] Data { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }
    }
}
