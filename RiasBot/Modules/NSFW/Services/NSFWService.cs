﻿using Discord;
using RiasBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RiasBot.Modules.NSFW.Services
{
    public class NSFWService : IKService
    {
        public NSFWService()
        {

        }

        public async Task<string> GetImage(string tag)
        {
            var rnd = new Random((int)DateTime.UtcNow.Ticks);
            int site = rnd.Next(3);

            switch(site)
            {
                case 0:
                    return await DownloadImages(NSFWSite.Danbooru, tag).ConfigureAwait(false);
                case 1:
                    return await DownloadImages(NSFWSite.Konachan, tag).ConfigureAwait(false);
                case 2:
                    return await DownloadImages(NSFWSite.Yandere, tag).ConfigureAwait(false);
            }
            return null;
        }

        public async Task<string> DownloadImages(NSFWSite site, string tag = null)
        {
            string api = null;
            string images = null;

            var rnd = new Random((int)DateTime.UtcNow.Ticks);
            int ratingRnd = rnd.Next(2);
            string rating = null;

            var data = new List<Hentai>();
            
            switch(ratingRnd)
            {
                case 0:
                    rating = "rating:questionable";
                    break;
                case 1:
                    rating = "rating:explicit";
                    break;
            }
            switch(site)
            {
                case NSFWSite.Danbooru:
                    api = $"http://danbooru.donmai.us/posts.json?limit=100&tags={rating}+{tag}";
                    break;
                case NSFWSite.Konachan:
                    api = $"https://konachan.com/post.json?s=post&q=index&limit=100&tags={rating}+{tag}";
                    break;
                case NSFWSite.Yandere:
                    api = $"https://yande.re/post.json?limit=100&tags={rating}+{tag}";
                    break;
            }
            try
            {
                using (var http = new HttpClient())
                {
                    images = await http.GetStringAsync(api).ConfigureAwait(false);

                    data = JsonConvert.DeserializeObject<List<Hentai>>(images);
                }
            }
            catch
            {

            }

            if (data.Count > 0)
            {
                int random = rnd.Next(data.Count);
                string imageUrl = data[random].File_Url;
                if (site == NSFWSite.Danbooru)
                    return "http://danbooru.donmai.us" + imageUrl;
                else
                    return imageUrl;
            }
            else
                return null;
        }

        public enum NSFWSite
        {
            Danbooru = 0,
            Konachan = 1,
            Yandere = 2
        }

        public class Hentai
        {
            public string File_Url { get; set; }
            public string Tags { get; set; }
            public string Tag_String { get; set; }
            public string Rating { get; set; }
        }
    }
}