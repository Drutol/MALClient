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

        private static readonly NullDictionary<int, Task<string>> AnimeImageTasks = new NullDictionary<int, Task<string>>();
        private static readonly NullDictionary<int, Task<string>> MangaImageTasks = new NullDictionary<int, Task<string>>();

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


            var task = anime ? AnimeImageTasks[id] : MangaImageTasks[id];
            if (task == null)
            {
                await _semaphore.WaitAsync();
                task = new AnimeImageQuery(id, anime).FetchImageUrl();
                _semaphore.Release();
                if (anime)
                {
                    AnimeImageTasks[id] = task;
                }
                else
                {
                    MangaImageTasks[id] = task;
                }
            }

            var link = await task;

            
            if (anime)
            {
                lock (CachedAnimeImages)
                {
                    if (!CachedAnimeImages.ContainsKey(id))
                        CachedAnimeImages.Add(id, link);
                    if (AnimeImageTasks.ContainsKey(id))
                        AnimeImageTasks.Remove(id);
                }
            }
            else
            {
                lock (CachedMangaImages)
                {
                    if (!CachedMangaImages.ContainsKey(id))
                        CachedMangaImages.Add(id, link);
                    if (MangaImageTasks.ContainsKey(id))
                        MangaImageTasks.Remove(id);
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

        class NullDictionary<TKey,TVal> : Dictionary<TKey, TVal>
        {
            public new TVal this[TKey key]
            {
                get
                {
                    try
                    {
                        return base[key];
                    }
                    catch (Exception e)
                    {
                        return default(TVal);
                    }
                }
                set { base[key] = value; }
            }
        }

        public static bool IsCached(int id, bool anime,ref string url)
        {
            if (anime)
            {
                if (CachedAnimeImages.ContainsKey(id))
                {
                    url = CachedAnimeImages[id];
                    return true;
                }
            }
            else
            {
                if (CachedAnimeImages.ContainsKey(id))
                {
                    url = CachedAnimeImages[id];
                    return true;
                }
            }
            return false;

        }
    }
}
