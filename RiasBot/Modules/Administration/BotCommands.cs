﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RiasBot.Commons.Attributes;
using RiasBot.Extensions;
using RiasBot.Services;
using RiasBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RiasBot.Modules.Administration
{
    public partial class Administration
    {
        public class BotCommands : RiasSubmodule
        {
            private readonly CommandHandler _ch;
            private readonly CommandService _service;
            private readonly DbService _db;
            private readonly DiscordSocketClient _client;
            private readonly BotService _botService;

            public BotCommands(CommandHandler ch, CommandService service, DbService db, DiscordSocketClient client, BotService botService)
            {
                _ch = ch;
                _service = service;
                _db = db;
                _client = client;
                _botService = botService;
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task LeaveGuild(ulong id)
            {
                var guild = await Context.Client.GetGuildAsync(id).ConfigureAwait(false);
                if (guild != null)
                {
                    var usersGuild = await guild.GetUsersAsync();
                    var embed = new EmbedBuilder().WithColor(RiasBot.color);
                    embed.WithDescription($"Leabing {Format.Bold(guild.Name)}");
                    embed.AddField("Id", guild.Id, true).AddField("Users", usersGuild.Count, true);

                    await ReplyAsync("", embed: embed.Build()).ConfigureAwait(false);

                    await guild.LeaveAsync().ConfigureAwait(false);
                }
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task Status(string type = null, [Remainder]string name = null)
            {
                try
                {
                    _botService.status.Dispose();
                }
                catch
                {

                }

                name = name ?? "";
                type = type?.ToLower();
                if (type is null)
                    await _client.SetActivityAsync(new Game("", ActivityType.Playing)).ConfigureAwait(false);

                switch (type)
                {
                    case "playing":
                        await _client.SetActivityAsync(new Game(name, ActivityType.Playing)).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationEmbed($"Activity status setted to {Format.Bold($"Playing {name}")}");
                        break;
                    case "listening":
                        await _client.SetActivityAsync(new Game(name, ActivityType.Listening)).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationEmbed($"Activity status setted to {Format.Bold($"Listening to {name}")}");
                        break;
                    case "watching":
                        await _client.SetActivityAsync(new Game(name, ActivityType.Watching)).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationEmbed($"Activity status setted to {Format.Bold($"Watching {name}")}");
                        break;
                }
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task StatusRotate(int time, [Remainder]string status)
            {
                var statuses = status.Split('\n');

                _botService.statuses = statuses;
                _botService.status = new Timer(async _ => await _botService.StatusRotate(), null, 0, time * 1000);

                await Context.Channel.SendConfirmationEmbed($"Activity status rotation setted: {time} seconds\n{String.Join("\n", statuses)}");
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task Streaming(string url = null, [Remainder]string name = null)
            {
                try
                {
                    _botService.status.Dispose();
                }
                catch
                {

                }

                name = name ?? "";
                url = url ?? "";

                var game = new Game(name, ActivityType.Streaming);
                game = new StreamingGame(name, url);

                await _client.SetActivityAsync(game).ConfigureAwait(false);
                await Context.Channel.SendConfirmationEmbed($"Activity status setted to {Format.Bold($"Streaming {name}")}");
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task Die()
            {
                await Context.Channel.SendConfirmationEmbed("Shutting down...").ConfigureAwait(false);
                await Context.Client.StopAsync().ConfigureAwait(false);
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task Send(string id, [Remainder]string message)
            {
                IGuild guild;
                IUser user;
                ITextChannel channel;

                //Placeholders
                message = message.Replace("[currency]", RiasBot.currency);
                message = message.Replace("%currency%", RiasBot.currency);

                if (id.Contains("|"))
                {
                    try
                    {
                        var ids = id.Split('|');
                        string guildId = ids[0];
                        string channelId = ids[1];

                        guild = await Context.Client.GetGuildAsync(Convert.ToUInt64(guildId)).ConfigureAwait(false);
                        channel = await guild.GetTextChannelAsync(Convert.ToUInt64(channelId)).ConfigureAwait(false);
                        await channel.SendMessageAsync(message).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationEmbed("Message sent!").ConfigureAwait(false);
                    }
                    catch
                    {
                        await Context.Channel.SendErrorEmbed("I couldn't find the guild or the channel");
                    }
                }
                else
                {
                    try
                    {
                        user = await Context.Client.GetUserAsync(Convert.ToUInt64(id));
                        await user.SendMessageAsync(message).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationEmbed("Message sent!").ConfigureAwait(false);
                    }
                    catch
                    {
                        await Context.Channel.SendErrorEmbed("I couldn't find the user").ConfigureAwait(false);
                    }
                }
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task Name([Remainder]string name)
            {
                try
                {
                    await Context.Client.CurrentUser.ModifyAsync(u => u.Username = name);
                    await ReplyAsync("New name " + name);
                }
                catch
                {
                    await ReplyAsync("You need to wait 2 hours to change your name again.");
                }
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireOwner]
            public async Task Avatar(string url)
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
}