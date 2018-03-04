﻿using Discord;
using Discord.Commands;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using RiasBot.Commons.Attributes;
using RiasBot.Extensions;
using RiasBot.Modules.Music.Common;
using RiasBot.Modules.Music.MusicServices;
using RiasBot.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RiasBot.Modules.Music
{
    public class Music : RiasModule<MusicService>
    {
        public IBotCredentials _creds;

        public Music(IBotCredentials creds)
        {
            _creds = creds;
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        [Priority(0)]
        public async Task Play([Remainder] string keywords)
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            try
            {
                var socketGuildUser = await Context.Guild.GetCurrentUserAsync();
                var preconditions = socketGuildUser.GetPermissions(voiceChannel);
                if (!preconditions.Connect)
                {
                    await ReplyAsync($"{Context.User.Mention} I don't have permission to connect in the channel {Format.Bold(voiceChannel.Name)}!");
                    return;
                }
            }
            catch
            {

            }

            if (String.IsNullOrEmpty(_creds.GoogleApiKey))
            {
                await Context.Channel.SendErrorEmbed("The Google Api Key must be seted to use the music module!").ConfigureAwait(false);
                return;
            }

            var mp = _service.GetOrAddMusicPlayer(Context.Guild);
            await mp.JoinAudio(Context.Guild, Context.Channel, voiceChannel).ConfigureAwait(false);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _creds.GoogleApiKey,
                ApplicationName = "Kurumi Bot"
            });

            if (Uri.IsWellFormedUriString(keywords, UriKind.Absolute))
            {
                var regex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]*)(?:.*list=|(?:.*/)?)([a-zA-Z0-9-_]*)");
                var matches = regex.Match(keywords);
                string videoId = matches.Groups[1].Value;
                string listId = matches.Groups[2].Value;

                if (!String.IsNullOrEmpty(listId))
                {
                    if (videoId == "playlist")
                    {
                        await PlayList(mp, youtubeService, listId, index: 0);
                    }
                    else
                    {
                        await PlayList(mp, youtubeService, listId, videoId);
                    }
                }
                else
                {
                    await PlayVideoURL(mp, youtubeService, videoId);
                }
            }
            else
            {
                await PlayVideo(mp, youtubeService, keywords);
            }
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        [Priority(1)]
        public async Task Play(int index)
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.PlayByIndex(index - 1);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Play()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetOrAddMusicPlayer(Context.Guild);
            mp.Unpause();
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Pause()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }
            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.TogglePause().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task NowPlaying()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }
            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.NowPlaying().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Destroy()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.RemoveMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Destroy("Stopping music playback!").ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Skip()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Skip().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Replay()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Replay().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Playlist()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Playlist().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Volume(int volume)
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.SetVolume(volume).ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Shuffle()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Shuffle().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        public async Task Clear()
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Clear().ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        [Priority(1)]
        public async Task Remove(int index)
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Remove(index - 1).ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        [RiasCommand][@Alias]
        [Description][@Remarks]
        [RequireContext(ContextType.Guild)]
        [Priority(1)]
        public async Task Remove([Remainder]string title)
        {
            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            if (voiceChannel is null)
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel!");
                return;
            }

            var mp = _service.GetMusicPlayer(Context.Guild);
            if (mp != null)
                await mp.Remove(title).ConfigureAwait(false);
            else
                await Context.Channel.SendErrorEmbed($"{Context.User.Mention} I'm not in a voice channel");
        }

        private async Task PlayList(MusicPlayer mp, YouTubeService youtubeService, string playlist, string videoId = null, int index = 0)
        {
            string title = null;
            string url = null;
            string thumbnail = null;
            TimeSpan duration = new TimeSpan();
            var user = (IGuildUser)Context.User;

            await mp.Clear().ConfigureAwait(false);

            await ReplyAsync("Adding songs to playlist, please wait").ConfigureAwait(false);
            int items = 0;
            var nextPageToken = "";
            while (nextPageToken != null)
            {
                var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                var videoListRequest = youtubeService.Videos.List("contentDetails");
                playlistItemsListRequest.PlaylistId = playlist;
                playlistItemsListRequest.MaxResults = 50;
                playlistItemsListRequest.PageToken = nextPageToken;

                try
                {
                    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync().ConfigureAwait(false);

                    foreach (var playlistItem in playlistItemsListResponse.Items)
                    {
                        if (items < 50)
                        {
                            try
                            {
                                string id = playlistItem.Snippet.ResourceId.VideoId;
                                if (id == videoId)
                                    index = (int?)playlistItem.Snippet.Position ?? 0;

                                videoListRequest.Id = playlistItem.Snippet.ResourceId.VideoId;
                                var videoListResponse = await videoListRequest.ExecuteAsync().ConfigureAwait(false);
                                title = playlistItem.Snippet.Title;
                                url = "https://youtu.be/" + playlistItem.Snippet.ResourceId.VideoId;
                                duration = System.Xml.XmlConvert.ToTimeSpan(videoListResponse.Items.FirstOrDefault().ContentDetails.Duration);
                                thumbnail = playlistItem.Snippet.Thumbnails.High.Url;

                                if (title != null && url != null && duration != new TimeSpan(0, 0, 0))
                                {
                                    await mp.Playlist((IGuildUser)Context.User, youtubeService, videoListRequest, playlistItem, index);
                                    items++;
                                }
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await ReplyAsync("Please provide a direct and unlisted or public YouTube playlist URL!");
                    return;
                }
            }
            await ReplyAsync($"Added to playlist {items} songs").ConfigureAwait(false);
            await mp.UpdateQueue(index).ConfigureAwait(false);
        }

        private async Task PlayVideoURL(MusicPlayer mp, YouTubeService youtubeService, string videoId)
        {
            string title = null;
            string url = null;
            string channel = null;
            string thumbnail = null;
            TimeSpan duration = new TimeSpan();
            var user = (IGuildUser)Context.User;

            var videoListRequestSnippet = youtubeService.Videos.List("snippet");
            var videoListRequestContentDetails = youtubeService.Videos.List("contentDetails");
            videoListRequestSnippet.Id = videoId;
            videoListRequestContentDetails.Id = videoId;

            try
            {
                var videoListResponseSnippet = await videoListRequestSnippet.ExecuteAsync().ConfigureAwait(false);
                var videoListResponseContentDetails = await videoListRequestContentDetails.ExecuteAsync().ConfigureAwait(false);

                url = "https://youtu.be/" + videoId;
                title = videoListResponseSnippet.Items.FirstOrDefault().Snippet.Title;
                channel = videoListResponseSnippet.Items.FirstOrDefault().Snippet.ChannelTitle;
                thumbnail = videoListResponseSnippet.Items.FirstOrDefault().Snippet.Thumbnails.High.Url;

                duration = System.Xml.XmlConvert.ToTimeSpan(videoListResponseContentDetails.Items.FirstOrDefault().ContentDetails.Duration);

                if (title != null && url != null && thumbnail != null)
                {
                    if (duration == new TimeSpan(0, 0, 0))
                    {
                        await ReplyAsync("I can't play live YouTube videos");
                        return;
                    }
                    else
                    {
                        await mp.Play(title, url, channel, duration, thumbnail, user).ConfigureAwait(false);
                    }
                }
                else
                {
                    await ReplyAsync("Please provide a direct YouTube video URL!");
                    return;
                }
            }
            catch
            {

            }
        }

        private async Task PlayVideo(MusicPlayer mp, YouTubeService youtubeService, string keywords)
        {
            string title = null;
            string url = null;
            string channel = null;
            string thumbnail = null;
            TimeSpan duration = new TimeSpan();
            var user = (IGuildUser)Context.User;

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = keywords;
            searchListRequest.MaxResults = 10;

            var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);
            var videoListRequest = youtubeService.Videos.List("contentDetails");

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.VideoId != null)
                {
                    url = "https://youtu.be/" + searchResult.Id.VideoId;
                    title = searchResult.Snippet.Title;
                    channel = searchResult.Snippet.ChannelTitle;
                    thumbnail = searchResult.Snippet.Thumbnails.High.Url;

                    videoListRequest.Id = searchResult.Id.VideoId;
                    var videoResponse = await videoListRequest.ExecuteAsync().ConfigureAwait(false);
                    duration = System.Xml.XmlConvert.ToTimeSpan(videoResponse.Items.FirstOrDefault().ContentDetails.Duration);

                    if (duration != new TimeSpan(0, 0, 0))
                        break;
                }
            }

            if (title != null && url != null && thumbnail != null)
            {
                if (duration == new TimeSpan(0, 0, 0))
                {
                    await ReplyAsync("I can't play live YouTube videos");
                    return;
                }
                else
                {
                    await mp.Play(title, url, channel, duration, thumbnail, user).ConfigureAwait(false);
                }
            }
            else
            {
                await ReplyAsync("Please provide a direct YouTube video URL!");
                return;
            }
        }
    }
}