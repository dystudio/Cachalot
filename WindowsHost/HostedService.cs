﻿using System;
using System.IO;
using Server;
using Server.Persistence;
using Client.Core;
using Channel;
using Newtonsoft.Json;
using Topshelf;

namespace Host
{
    public class HostedService
    {
        private Server.Server _cacheServer;
        private TcpServerChannel _listener;
        private ILog Log { get; }

        public HostedService(ILog log)
        {
            Log = log;

            ServerLog.ExternalLog = log;
        }


        public bool Start(HostControl hostControl)
        {
            HostServices.HostServices.Start();

            Log.LogInfo("----------------------------------------------------------");
            


            try
            {

                int port = Constants.DefaultPort;
                bool persistent = true;
                string dataPath = null;

                if (File.Exists(Constants.NodeConfigFileName))
                {
                    try
                    {
                        var nodeConfig = SerializationHelper.FormattedSerializer.Deserialize<NodeConfig>(new JsonTextReader(new StringReader(File.ReadAllText(Constants.NodeConfigFileName))));

                        port = nodeConfig.TcpPort;
                        persistent = nodeConfig.IsPersistent;
                        dataPath = nodeConfig.DataPath;

                    }
                    catch (Exception e)
                    {
                        Log.LogError($"Error reading configuration file {Constants.NodeConfigFileName} : {e.Message}");
                    }
                }
                else
                {
                    Log.LogWarning($"Configuration file {Constants.NodeConfigFileName} not found. Using defaults");
                }
                _cacheServer = new Server.Server(new ServerConfig(), persistent, dataPath);

                _listener = new TcpServerChannel();
                _cacheServer.Channel = _listener;
                _listener.Init(port); 
                _listener.Start();

                var fullDataPath = Path.GetFullPath(dataPath ?? Constants.DataPath);

                var persistentDescription = persistent ? fullDataPath : " NO";
                
                Log.LogInfo($"Starting Cachalot server on port {port}  persistent {persistentDescription}");

                Console.Title = $"Cachalot server on port {port} persistent = {persistentDescription}";
                
                
                _cacheServer.Start();

            }
            catch (Exception e)
            {
                Log.LogError($"Failed to start host: {e}");

                return false;
            }

            Log.LogInfo("Host started successfully");

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Log.LogInfo("Stopping service");

            _listener.Stop();

           _cacheServer.Stop();

            Log.LogInfo("Service stopped successfully");

            // stop this after last log
            HostServices.HostServices.Stop();


            return true;
        }
    }
}