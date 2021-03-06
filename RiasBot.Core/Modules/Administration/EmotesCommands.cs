﻿using Discord;
using Discord.Commands;
using RiasBot.Commons.Attributes;
using RiasBot.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RiasBot.Modules.Administration
{
    public partial class Administration
    {
        public class EmotesCommands : RiasSubmodule
        {
            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireUserPermission(GuildPermission.ManageEmojis)]
            [RequireBotPermission(GuildPermission.ManageEmojis)]
            [RequireContext(ContextType.Guild)]
            public async Task AddEmote(string url, [Remainder]string name)
            {
                var isAnimated = false;
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the url is not a well formed uri string.").ConfigureAwait(false);
                    return;
                }
                if (!url.Contains("https"))
                {
                    await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the url must be https").ConfigureAwait(false);
                    return;

                }
                if (!url.Contains(".png") && !url.Contains(".jpg") && !url.Contains(".jpeg"))
                {
                    if (!url.Contains(".gif"))
                    {
                        await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the url is not a direct link for a png, jpg, jpeg or gif image.").ConfigureAwait(false);
                        return;
                    }
                    else
                    {
                        isAnimated = true;
                    }
                }
                
                name = name.Replace(" ", "_");
                using (var http = new HttpClient())
                {
                    var staticEmotes = new List<IEmote>();
                    var animatedEmotes = new List<IEmote>();
                
                    var emotes = Context.Guild.Emotes;
                    foreach (var emote in emotes)
                    {
                        if (emote.Animated)
                            animatedEmotes.Add(emote);
                        else
                            staticEmotes.Add(emote);
                    }

                    if (isAnimated)
                    {
                        if (animatedEmotes.Count == 50)
                        {
                            await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the server already has the limit of 50 animated emotes.");
                            return;
                        }
                    }
                    else
                    {
                        if (staticEmotes.Count == 50)
                        {
                            await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the server already has the limit of 50 non-animated emotes.");
                            return;
                        }
                    }

                    try
                    {
                        var res = await http.GetAsync(new Uri(url)).ConfigureAwait(false);
                        if (res.IsSuccessStatusCode)
                        {
                            using (var emote = await res.Content.ReadAsStreamAsync())
                            {
                                var ms = new MemoryStream();
                                await emote.CopyToAsync(ms);
                                ms.Position = 0;
                                
                                if (ms.Length / 1024 <= 256) //in KB
                                {
                                    var emoteImage = new Image(ms);
                                    await Context.Guild.CreateEmoteAsync(name, emoteImage).ConfigureAwait(false);
                                    await Context.Channel.SendConfirmationMessageAsync($"{Context.User.Mention} emote {Format.Bold(name)} was created successfully.").ConfigureAwait(false);
                                }
                                else
                                {
                                    await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the image is bigger than 256 KB.").ConfigureAwait(false);
                                }
                            }
                        }
                    }
                    catch
                    {
                        await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} the image or the URL are not good.").ConfigureAwait(false);
                    }
                }
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireUserPermission(GuildPermission.ManageEmojis)]
            [RequireBotPermission(GuildPermission.ManageEmojis)]
            [RequireContext(ContextType.Guild)]
            public async Task DeleteEmote([Remainder]string name)
            {
                try
                {
                    var emote = Context.Guild.Emotes.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                    if (emote is null)
                    {
                        await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} I couldn't find the emote.").ConfigureAwait(false);
                    }
                    else
                    {
                        await Context.Guild.DeleteEmoteAsync(emote).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationMessageAsync($"{Context.User.Mention} emote {Format.Bold(emote.Name)} was deleted successfully.");
                    }
                }
                catch
                {
                    await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} I couldn't delete the emote.");
                }
            }

            [RiasCommand][@Alias]
            [Description][@Remarks]
            [RequireUserPermission(GuildPermission.ManageEmojis)]
            [RequireBotPermission(GuildPermission.ManageEmojis)]
            [RequireContext(ContextType.Guild)]
            public async Task RenameEmote([Remainder]string name)
            {
                var emotes = name.Split("->");
                var oldName = emotes[0].TrimEnd().Replace(" ", "_");
                var newName = emotes[1].TrimStart().Replace(" ", "_");
                try
                {
                    var emote = Context.Guild.Emotes.FirstOrDefault(x => string.Equals(x.Name, oldName, StringComparison.InvariantCultureIgnoreCase));
                    if (emote is null)
                    {
                        await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} I couldn't find the emote.").ConfigureAwait(false);
                    }
                    else
                    {
                        await Context.Guild.ModifyEmoteAsync(emote, x => x.Name = newName).ConfigureAwait(false);
                        await Context.Channel.SendConfirmationMessageAsync($"{Context.User.Mention} emote {Format.Bold(emote.Name)} was renamed to {Format.Bold(newName)} successfully.");
                    }
                }
                catch
                {
                    await Context.Channel.SendErrorMessageAsync($"{Context.User.Mention} I couldn't rename the emote.");
                }
            }
        }
    }
}
