using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SuperSocket.ClientEngine.Core.Config
{
    public interface IRootConfig
    {
        string LogFactory { get; }
    }
    public interface IClientConfig
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the name of the server type this appServer want to use.
        /// </summary>
        /// <value>
        /// The name of the server type.
        /// </value>
        string ClientTypeName { get; }
        /// <summary>
        /// Gets the ip.
        /// </summary>
        string Ip { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        int Port { get; }

        int MaxReceiveLength { get; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        NameValueCollection Options { get; }


        /// <summary>
        /// Gets the option elements.
        /// </summary>
        NameValueCollection OptionElements { get; }
    }
    public interface IConfigurationSource : IRootConfig
    {
        /// <summary>
        /// Gets the servers definitions.
        /// </summary>
        IEnumerable<IClientConfig> Clients { get; }
    }
}
