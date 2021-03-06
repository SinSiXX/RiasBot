﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RiasBot.Commons;

namespace RiasBot.Services.Implementation
{
    public class BotCredentials : IBotCredentials
    {
        public string Prefix { get; }
        public string Token { get; }
        public string GoogleApiKey { get; }
        public string UrbanDictionaryApiKey { get; }
        public string PatreonAccessToken { get; }
        public string DiscordBotsListApiKey { get; }
        public string WeebServicesToken { get; }
        public DatabaseConfig DatabaseConfig { get; }
        public LavalinkConfig LavalinkConfig { get; }
        public VotesManagerConfig VotesManagerConfig { get; }
        public bool IsBeta { get; }    //beta bool is too protect things to run only on the public version, like apis

        private readonly string _credsFileName = Path.Combine(Environment.CurrentDirectory, "data/credentials.json");
        public BotCredentials()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(_credsFileName);

            var config = configBuilder.Build();

            Prefix = config[nameof(Prefix)];
            Token = config[nameof(Token)];
            GoogleApiKey = config[nameof(GoogleApiKey)];
            UrbanDictionaryApiKey = config[nameof(UrbanDictionaryApiKey)];
            PatreonAccessToken = config[nameof(PatreonAccessToken)];
            DiscordBotsListApiKey = config[nameof(DiscordBotsListApiKey)];
            WeebServicesToken = config[nameof(WeebServicesToken)];
            
            var databaseConfig = config.GetSection(nameof(DatabaseConfig));
            DatabaseConfig = new DatabaseConfig
            {
                Host = databaseConfig.GetValue<string>("Host"),
                Port = databaseConfig.GetValue<ushort>("Port"),
                Database = databaseConfig.GetValue<string>("Database"),
                Username = databaseConfig.GetValue<string>("Username"),
                Password = databaseConfig.GetValue<string>("Password")
            };

            var lavalinkConfig = config.GetSection(nameof(LavalinkConfig));
            LavalinkConfig = new LavalinkConfig
            {
                Host = lavalinkConfig["host"],
                Port = ushort.Parse(lavalinkConfig["port"]),
                Authorization = lavalinkConfig["Authorization"]
            };
            
            var votesManagerConfig = config.GetSection(nameof(VotesManagerConfig));
            VotesManagerConfig = new VotesManagerConfig(votesManagerConfig.GetValue<string>("WebSocketHost"), votesManagerConfig.GetValue<ushort>("WebSocketPort"),
                votesManagerConfig.GetValue<bool>("IsSecureConnection"), votesManagerConfig.GetValue<string>("UrlParameters"),
                votesManagerConfig.GetValue<string>("Authorization"));
            IsBeta = config.GetValue<bool>(nameof(IsBeta));
        }
    }
}
