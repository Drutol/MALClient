using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeImageQuery : Query
    {
        private readonly int _id;
        private readonly bool _anime;
        private static Dictionary<int, string> CachedAnimeImages { get; set; } 
        private static Dictionary<int, string> CachedMangaImages { get; set; } 
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(2);

        public static async void Init()
        {
            CachedAnimeImages = await DataCache.RetrieveData<Dictionary<int, string>>("id_to_img", "Anime", -1) ?? new Dictionary<int, string>();
            CachedMangaImages = await DataCache.RetrieveData<Dictionary<int, string>>("id_to_img", "Manga", -1) ?? new Dictionary<int, string>();
        }

        public static async Task SaveData()
        {
            await DataCache.SaveData(CachedAnimeImages,"id_to_img", "Anime");
            await DataCache.SaveData(CachedMangaImages,"id_to_img", "Manga");
        }

        private AnimeImageQuery(int id,bool anime)
        {
            _id = id;
            _anime = anime;
        }

        public static async Task<string> GetImageUrl(int id, bool anime)
        {
            if (anime)
            {
                lock (CachedAnimeImages)
                {
                    if (CachedAnimeImages.ContainsKey(id))
                        return CachedAnimeImages[id];
                }

            }
            else
            {
                lock (CachedMangaImages)
                {
                    if (CachedMangaImages.ContainsKey(id))
                        return CachedMangaImages[id];
                }
            }

            await _semaphore.WaitAsync();
            var link = await new AnimeImageQuery(id, anime).FetchImageUrl();
            _semaphore.Release();

            if (anime)
            {
                lock (CachedAnimeImages)
                {
                    CachedAnimeImages.Add(id, link);
                }
            }
            else
            {
                lock (CachedMangaImages)
                {
                    CachedMangaImages.Add(id, link);
                }
            }

            return link;
        }

        private async Task<string> FetchImageUrl()
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/{(_anime ? "anime" : "manga")}/{_id}/what/pics"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";

            var raw = await GetRequestResponse();
            var doc = new HtmlDocument();
            try
            {
                doc.LoadHtml(raw);
                return doc.FirstOfDescendantsWithClass("img", "ac").Attributes["src"].Value;
            }
            catch (Exception e)
            {
                //well it's html
            }
            return "";
        }


    }
}
