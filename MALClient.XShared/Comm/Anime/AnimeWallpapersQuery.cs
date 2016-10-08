using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models.Misc;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeWallpapersQuery : Query
    {
        public AnimeWallpapersQuery(string name)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"https://www.reddit.com/r/animewallpaper/search.json?q={name}&type=link&restrict_sr=true"));
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

            foreach (var child in data.data.children)
            {
                output.Add(new AnimeWallpaperData
                {
                    FileUrl = child.data.url,
                    Title = child.data.title,
                    Nsfw = child.data.over_18,
                    RedditUrl = "https://www.reddit.com" + child.data.permalink
                });
            }

            return output;
        }
    }
}
