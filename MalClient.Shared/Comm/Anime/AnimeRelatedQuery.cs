using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;
using MALClient.Utils;

namespace MALClient.Comm
{
    public enum RelatedItemType
    {
        Anime,
        Manga,
        Unknown
    }

    public class AnimeRelatedQuery : Query
    {
        private readonly int _animeId;
        private readonly bool _animeMode;

        public AnimeRelatedQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeId = id;
            _animeMode = anime;
        }

        public async Task<List<RelatedAnimeData>> GetRelatedAnime(bool force = false)
        {
            var output = force
                ? new List<RelatedAnimeData>()
                : await DataCache.RetrieveRelatedAnimeData(_animeId, _animeMode) ?? new List<RelatedAnimeData>();
            if (output.Count != 0) return output;

            var raw = await GetRequestResponse(false);
            if (string.IsNullOrEmpty(raw))
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            try
            {
                var relationsNode = doc.DocumentNode.Descendants("table")
                    .First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            HtmlClassMgr.ClassDefs["#Related:relationsNode:class"]);

                foreach (var row in relationsNode.Descendants("tr"))
                {
                    try
                    {
                        var current = new RelatedAnimeData();
                        current.WholeRelation = WebUtility.HtmlDecode(row.Descendants("td").First().InnerText.Trim()) +
                                                " ";
                        var linkNode = row.Descendants("a").First();
                        var link = linkNode.Attributes["href"].Value.Split('/');
                        current.Type = link[1] == "anime"
                            ? RelatedItemType.Anime
                            : link[1] == "manga" ? RelatedItemType.Manga : RelatedItemType.Unknown;
                        current.Id = Convert.ToInt32(link[2]);
                        current.Title = WebUtility.HtmlDecode(linkNode.InnerText.Trim());
                        current.WholeRelation += current.Title;
                        output.Add(current);
                    }
                    catch (Exception)
                    {
                        //mystery
                    }
                }
            }
            catch (Exception)
            {
                //no recom
            }
            DataCache.SaveRelatedAnimeData(_animeId, output, _animeMode);

            return output;
        }
    }
}