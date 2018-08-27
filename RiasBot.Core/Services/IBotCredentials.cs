﻿using System;
using System.Collections.Generic;
using System.Text;
using RiasBot.Commons;

namespace RiasBot.Services
{
    public interface IBotCredentials
    {
        ulong ClientId { get; }
        string Prefix { get; }
        string Token { get; }
        string GoogleApiKey { get; }
        string UrbanDictionaryApiKey { get; }
        string PatreonAccessToken { get; }
        string DiscordBotsListApiKey { get; }
        string WeebServicesToken { get; }
        LavalinkConfig LavalinkConfig { get; }
        string HelpDM { get; }
    }
    
    public class LavalinkConfig
    {
        public string RestHost { get; }
        public ushort RestPort { get; }
        public string WebSocketHost { get; }
        public ushort WebSocketPort { get; }
        public string Authorization { get; }

        public LavalinkConfig(string restHost, ushort restPort, string webSocketHost, ushort webSocketPort, string authorization)
        {
            RestHost = restHost;
            RestPort = restPort;
            WebSocketHost = webSocketHost;
            WebSocketPort = webSocketPort;
            Authorization = authorization;
        }
    }
}
