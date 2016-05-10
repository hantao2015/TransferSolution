using SuperSocket.ClientEngine.Protocol;
namespace TransferClientLib
{
    public class TransferCommandInfo : ICommandInfo
    {
        public TransferCommandInfo()
        {
        }

        public TransferCommandInfo(string key)
        {
            this.Key = key;
        }

        public TransferCommandInfo(string key, string text)
        {
            this.Key = key;
            this.Text = text;
        }

        public TransferCommandInfo(string key, byte[] data)
        {
            this.Key = key;
            this.Data = data;
        }

        public byte[] Data { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }
    }
}
