using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.Misc;
using MALClient.XShared.Utils;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeWallpapersQuery : Query
    {
        private readonly string _subreddit;
        private readonly int _page;
        private readonly WallpaperSources _source;
        private static readonly List<Tuple<WallpaperSources,string>> _subreddits = new List<Tuple<WallpaperSources, string>>();
        private static readonly Dictionary<int,List<AnimeWallpaperData>> _cache = new Dictionary<int, List<AnimeWallpaperData>>();

        static AnimeWallpapersQuery()
        {
            foreach (WallpaperSources value in Enum.GetValues(typeof(WallpaperSources)))
                _subreddits.Add(new Tuple<WallpaperSources, string>(value,value.ToString()));
        }

        private static readonly Dictionary<string,List<string>> _lastThings = new Dictionary<string, List<string>>(); //as reddit calls them "things"

        public static async Task<List<AnimeWallpaperData>> GetAllWallpapers(int page)
        {
            if (_cache.ContainsKey(page))
                return _cache[page];

            var tasks = new List<Task<List<AnimeWallpaperData>>>();
            lock (_lastThings)
            {
                _subreddits.ForEach(
                    s =>
                        tasks.Add(
                            new AnimeWallpapersQuery(s.Item2, page == 0 ? null : _lastThings[s.Item2][page - 1], page,
                                s.Item1).GetWallpapers()));
            }
            await Task.WhenAll(tasks);
            var output = new List<AnimeWallpaperData>();
            foreach (var task in tasks)
            {
                output.AddRange(task.Result);
            }
            output = output.OrderByDescending(data => data.Upvotes).ToList();
            _cache.Add(page, output);
            return output;
        }

        private AnimeWallpapersQuery(string subreddit,string after,int page,WallpaperSources source)
        {
            _subreddit = subreddit;
            _page = page;
            _source = source;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"https://www.reddit.com/r/{subreddit}/hot.json?limit=4&count={page*4}{(string.IsNullOrEmpty(after) ? "" : $"&after={after}")}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        private async Task<List<AnimeWallpaperData>> GetWallpapers()
        {
            var raw = await GetRequestResponse(false);
            if (string.IsNullOrEmpty(raw))
                return null;

            var data = JsonConvert.DeserializeObject<RedditSearchRoot>(raw);

            lock (_lastThings)
            {
                if(!_lastThings.ContainsKey(_subreddit))
                    _lastThings[_subreddit] = new List<string>();
                _lastThings[_subreddit].Insert(_page, data.data.after as string);
            }

            var now = Utilities.ConvertToUnixTimestamp(DateTime.UtcNow);

            return
                data.data.children.Where(
                        child =>
                            (_page < 2 || now - child.data.created_utc < 604800) &&
                            Regex.IsMatch(child.data.url,
                                @"(http:|https:)\/\/(i.imgur.com|cdn.awwni.me|i.redd.it)\/(?!a\/).*"))
                    .Select(child => new AnimeWallpaperData
                    {
                        FileUrl = child.data.url,
                        Title = child.data.title,
                        Nsfw = child.data.over_18,
                        Upvotes = child.data.ups,
                        RedditUrl = "https://www.reddit.com" + child.data.permalink,
                        Source = _source,
                    }).ToList();
        }
    }
}
