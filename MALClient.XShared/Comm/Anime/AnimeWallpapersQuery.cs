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
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeWallpapersQuery : Query
    {
        private readonly string _subreddit;
        private readonly int _page;
        private readonly WallpaperSources _source;
        private static int _baseItemsToPull = Settings.WallpapersBaseAmount;

        private static readonly Dictionary<int,List<AnimeWallpaperData>> Cache = new Dictionary<int, List<AnimeWallpaperData>>();
        private static List<WallpaperSources> _previousSourceSet;
        private static int _itemsToPull;

        public static int BaseItemsToPull
        {
            get { return _baseItemsToPull; }
            set
            {
                _baseItemsToPull = value;
                Reset();
            }
        }

        private static readonly Dictionary<string,List<string>> LastThings = new Dictionary<string, List<string>>(); //as reddit calls them "things"


        public static async Task<List<AnimeWallpaperData>> GetAllWallpapers(int page)
        {
            //if we have different set we are clearing previously fetched data
            var allSubsCount = Enum.GetValues(typeof(WallpaperSources)).Length;
            var currentSubs = Settings.EnabledWallpaperSources.OrderBy(source => (int)source).ToList();
            if (!_previousSourceSet?.SequenceEqual(currentSubs) ?? false)
            {
                Cache.Clear();
                LastThings.Clear();
                page = 0;
                ViewModelLocator.Wallpapers.CurrentPage = 0;                           
            }
            if (page > LastThings.Count)
            {
                page = 0;
                ViewModelLocator.Wallpapers.CurrentPage = 0;
            }
            _itemsToPull = BaseItemsToPull + (allSubsCount - currentSubs.Count);
            _previousSourceSet = currentSubs;
            if (Cache.ContainsKey(page))
                return Cache[page];

            var tasks = new List<Task<List<AnimeWallpaperData>>>();
            lock (LastThings)
            {
                currentSubs.ForEach(
                    s =>
                        tasks.Add(
                            new AnimeWallpapersQuery(s.ToString(), page == 0 ? null : LastThings[s.ToString()][page - 1], page,
                                s).GetWallpapers()));
            }
            await Task.WhenAll(tasks);
            var output = new List<AnimeWallpaperData>();
            foreach (var task in tasks)
            {
                output.AddRange(task.Result);
            }
            output = output.Distinct().OrderByDescending(data => data.DateTime).ToList();
            Cache.Add(page, output);
            return output;
        }

        public static void Reset()
        {
            Cache.Clear();
            LastThings.Clear();
            _previousSourceSet = null;
            _itemsToPull = 0;
        }

        private AnimeWallpapersQuery(string subreddit,string after,int page,WallpaperSources source)
        {
            _subreddit = subreddit;
            _page = page;
            _source = source;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"https://www.reddit.com/r/{subreddit}/hot.json?limit={_itemsToPull}&count={page*_itemsToPull}{(string.IsNullOrEmpty(after) ? "" : $"&after={after}")}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        private async Task<List<AnimeWallpaperData>> GetWallpapers()
        {
            var raw = await GetRequestResponse(false);
            if (string.IsNullOrEmpty(raw))
                return null;

            var data = JsonConvert.DeserializeObject<RedditSearchRoot>(raw);

            lock (LastThings)
            {
                if(!LastThings.ContainsKey(_subreddit))
                    LastThings[_subreddit] = new List<string>();
                LastThings[_subreddit].Insert(_page, data.data.after as string);
            }


            return
                data.data.children.Where(
                        child =>
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
                        DateTime = Utilities.ConvertFromUnixTimestamp(child.data.created_utc)
                    }).ToList();
        }
    }
}
