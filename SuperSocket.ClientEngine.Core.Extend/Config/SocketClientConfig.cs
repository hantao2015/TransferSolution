using SuperSocket.ClientEngine.Common;
using System.Collections.Generic;
using System.Configuration;

namespace SuperSocket.ClientEngine.Core.Config
{
    public partial class SocketClientConfig : ConfigurationSection, IConfigurationSource
    {
        [ConfigurationProperty("clients")]
        public ClientCollection Clients
        {
            get { return this["clients"] as ClientCollection; }
        }
        /// <summary>
        /// Gets the logfactory name of the bootstrap.
        /// </summary>
        [ConfigurationProperty("logFactory", IsRequired = false, DefaultValue = "")]
        public string LogFactory
        {
            get
            {
                return (string)this["logFactory"];
            }
        }

        IEnumerable<IClientConfig> IConfigurationSource.Clients
        {

            get
            {
                return this.Clients;
            }
        }
    }


    /// <summary>
    /// Server configuration collection
    /// </summary>
    [ConfigurationCollection(typeof(Client), AddItemName = "client")]
    public class ClientCollection : GenericConfigurationElementCollection<Client, IClientConfig>
    {
        /// <summary>
        /// Adds the new server element.
        /// </summary>
        /// <param name="newServer">The new server.</param>
        public void AddNew(Client newClient)
        {
            base.BaseAdd(newClient);
        }

        /// <summary>
        /// Removes the specified server from the configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public partial class Client : ConfigurationElementBase, IClientConfig
    {
        /// <summary>
        /// Gets the ip.
        /// </summary>
        [ConfigurationProperty("ip", IsRequired = false, DefaultValue = ClientConfig.DefaultIp)]
        public string Ip
        {
            get { return this["ip"] as string; }

        }
        /// <summary>
        /// Gets the port.
        /// </summary>
        [ConfigurationProperty("port", IsRequired = false, DefaultValue = ClientConfig.DefaultPort)]
        public int Port
        {
            get { return (int)this["port"]; }
        }
        [ConfigurationProperty("maxReceiveLength", IsRequired = false, DefaultValue = ClientConfig.DefaultMaxReceiveLength)]
        public int MaxReceiveLength
        {
            get { return (int)this["maxReceiveLength"]; }
        }
        [ConfigurationProperty("clientTypeName", IsRequired = false)]
        public string ClientTypeName
        {
            get { return this["clientTypeName"] as string; }
        }
    }
}
