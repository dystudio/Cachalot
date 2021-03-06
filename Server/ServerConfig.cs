#region

using System.Collections.Generic;
using System.Xml;
using Client.Interface;

#endregion

namespace Server
{
    
    /// <summary>
    ///   Server configuration; Contains global configuration and by type configuration
    /// </summary>
    public class ServerConfig
    {
        private readonly Dictionary<string, ServerDatastoreConfig> _configByType =
            new Dictionary<string, ServerDatastoreConfig>();


        public IDictionary<string, ServerDatastoreConfig> ConfigByType => _configByType;

        public int TcpPort { get; set; } = 4488;

        /// <summary>
        ///   Load a server config from an XML file
        /// </summary>
        /// <param name="fileName"> </param>
        /// <returns> </returns>
        public static ServerConfig LoadFromFile(string fileName)
        {
            ServerConfig config = new ServerConfig();
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            string tcpPort = StringFromXpath(doc.DocumentElement, "//tcp/port");
            config.TcpPort = int.Parse(tcpPort);

            XmlNodeList dataStoreConfigs = doc.SelectNodes("//datastore");
            foreach (XmlNode node in dataStoreConfigs)
            {
                ServerDatastoreConfig cfg = new ServerDatastoreConfig();
                cfg.FullTypeName = StringFromXpath(node, "@typename");

                string threads = StringFromXpath(node, "threads");
                cfg.Threads = int.Parse(threads);

                string eviction = StringFromXpath(node, "eviction/@type");
                if (eviction == "LRU")
                {
                    cfg.Eviction.Type = EvictionType.LessRecentlyUsed;
                    string limit = StringFromXpath(node, "eviction/lruLimit");
                    cfg.Eviction.LruMaxItems = int.Parse(limit);
                    string count = StringFromXpath(node, "eviction/lruEvictionCount");
                    cfg.Eviction.LruEvictionCount = int.Parse(count);
                }

                config.ConfigByType.Add(cfg.FullTypeName, cfg);
            }

            return config;
        }

        private static string StringFromXpath(XmlNode element, string xpath)
        {
            XmlNode node = element.SelectSingleNode(xpath);
            if (node != null) return node.InnerText;
            return string.Empty;
        }
    }
}