using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MALClient.Models.Models.Misc;
using MALClient.XShared.Utils;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeWallpapersQuery : Query
    {
        private static readonly List<string> _subreddits = new List<string>
        {
            "animewallpaper",
            "awwnime",
        };

        public static async Task<List<AnimeWallpaperData>> GetAllWallpapers()
        {

            var tasks = new List<Task<List<AnimeWallpaperData>>>();
            _subreddits.ForEach(s => tasks.Add(new AnimeWallpapersQuery(s).GetWallpapers()));
            await Task.WhenAll(tasks);
            var output = new List<AnimeWallpaperData>();
            foreach (var task in tasks)
            {
                output.AddRange(task.Result);
            }
            return output.OrderByDescending(data => data.Upvotes).ToList();

        }

        public AnimeWallpapersQuery(string subreddit)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"https://www.reddit.com/r/{subreddit}/hot.json?limit=30"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<AnimeWallpaperData>> GetWallpapers()
        {
            var output = new List<AnimeWallpaperData>();
            var raw = await GetRequestResponse(false);
            if (string.IsNullOrEmpty(raw))
                return null;

            var data = JsonConvert.DeserializeObject<RedditSearchRoot>(raw);

            foreach (
                var child in
                data.data.children.Where(
                    child => Regex.IsMatch(child.data.url, @"(http:|https:)\/\/(i.imgur.com|cdn.awwni.me)\/(?!a\/).*")))
            {
                output.Add(new AnimeWallpaperData
                {
                    FileUrl = child.data.url,
                    Title = child.data.title,
                    Nsfw = child.data.over_18,
                    Upvotes = child.data.ups,
                    RedditUrl = "https://www.reddit.com" + child.data.permalink
                });
            }

            return output;
        }
    }
}
