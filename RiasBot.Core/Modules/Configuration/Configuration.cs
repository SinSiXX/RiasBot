﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RiasBot.Commons.Attributes;
using RiasBot.Extensions;
using RiasBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RiasBot.Modules.Configuration
{
    public partial class Configuration : RiasModule
    {
        private readonly CommandHandler _ch;
        private readonly CommandService _service;
        private readonly DbService _db;
        private readonly DiscordShardedClient _client;
        private readonly BotService _botService;

        public Configuration(CommandHandler ch, CommandService service, DbService db, DiscordShardedClient client, BotService botService)
        {
            _ch = ch;
            _service = service;
            _db = db;
            _client = client;
            _botService = botService;
        }

        [RiasCommand]
        [@Alias]
        [Description]
        [@Remarks]
        [RequireOwner]
        public async Task SetName([Remainder]string name)
        {
            try
            {
                await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
                await Context.Channel.SendConfirmationMessageAsync("New name " + name);
            }
            catch
            {
                await Context.Channel.SendErrorMessageAsync("You need to wait 2 hours to change your name again.");
            }
        }

        [RiasCommand]
        [@Alias]
        [Description]
        [@Remarks]
        [RequireOwner]
        public async Task SetAvatar(string url)
        {
            try
            {
                var http = new HttpClient();
                var res = await http.GetStreamAsync(new Uri(url));
                var ms = new MemoryStream();
                res.CopyTo(ms);
                ms.Position = 0;
                await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(ms));
            }
            catch
            {

            }
        }
    }
}
