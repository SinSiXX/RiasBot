﻿using Discord;
using Discord.Commands;
using RiasBot.Commons.Attributes;
using RiasBot.Extensions;
using RiasBot.Services;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RiasBot.Modules.Searches
{
    public partial class Searches : RiasModule
    {
        private readonly CommandHandler _ch;
        private readonly CommandService _service;
        private readonly IBotCredentials _creds;

        public Searches(CommandHandler ch, CommandService service, IBotCredentials creds)
        {
            _ch = ch;
            _service = service;
            _creds = creds;
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        public async Task UrbanDictionary([Remainder]string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(_creds.UrbanDictionaryApiKey))
                {
                    await Context.Channel.SendErrorMessageAsync("The urban dictionary api key needs to be set to use this command!").ConfigureAwait(false);
                    return;
                }

                await Context.Channel.TriggerTypingAsync().ConfigureAwait(false);

                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("X-Mashape-Key", _creds.UrbanDictionaryApiKey);
                    http.DefaultRequestHeaders.Add("Accept", "text/plain");
                    var response = await http.GetStringAsync("https://mashape-community-urban-dictionary.p.mashape.com/define?term=" + Uri.EscapeUriString(keyword));
                    
                    var items = JObject.Parse(response);
                    var item = items["list"][0];
                    var word = item["word"].ToString();
                    var def = item["definition"].ToString();
                    var link = item["permalink"].ToString();

                    var embed = new EmbedBuilder().WithColor(RiasBot.GoodColor);
                    embed.WithUrl(link);
                    embed.WithAuthor(word, "https://i.imgur.com/re5jokL.jpg");
                    embed.WithDescription(def);

                    await Context.Channel.SendMessageAsync("", embed: embed.Build()).ConfigureAwait(false);
                }
            }
            catch
            {
                await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} I couldn't find anything.");
            }
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        public async Task Wikipedia([Remainder]string keyword)
        {
            await Context.Channel.TriggerTypingAsync().ConfigureAwait(false);

            try
            {
                string search = null;
                using (var http = new HttpClient())
                {
                    search = await http.GetStringAsync($"https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=" + Uri.EscapeDataString(keyword));
                }

                var itemX = JObject.Parse(search);
                var item = itemX["query"];
                var page = item["pages"][0];
                var url = page["fullurl"].ToString();

                await Context.Channel.SendMessageAsync(url).ConfigureAwait(false);
            }
            catch
            {
                await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} I couldn't find anything.");
            }
        }
    }
}
