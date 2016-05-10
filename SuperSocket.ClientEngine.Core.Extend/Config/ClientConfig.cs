using System;
using System.Collections.Specialized;
using SuperSocket.ClientEngine.Common;

namespace SuperSocket.ClientEngine.Core.Config
{
    [Serializable]
    public partial class ClientConfig : IClientConfig
    {
        public const string DefaultIp = "127.0.0.1";
        public const int DefaultPort = 2020;
        public const int DefaultMaxReceiveLength = 1024 * 3;
        public ClientConfig()
        {
            Ip = DefaultIp;
            Port = DefaultPort;
            MaxReceiveLength = DefaultMaxReceiveLength;
        }
        public ClientConfig(IClientConfig clientConfig)
        {
            clientConfig.CopyPropertiesTo(this);
            this.Options = clientConfig.Options;
            this.OptionElements = clientConfig.OptionElements;
        }
        public string Name { get; set; }
        public string ClientTypeName { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public int MaxReceiveLength { get; set; }
        public NameValueCollection Options { get; set; }
        public NameValueCollection OptionElements { get; set; }
    }
}
