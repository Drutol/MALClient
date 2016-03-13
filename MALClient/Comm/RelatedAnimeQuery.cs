using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    class RelatedAnimeQuery : Query
    {
        private int _animeId;

        public RelatedAnimeQuery(int id)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/anime/{id}/"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeId = id;
        }

        public async Task<List<RelatedAnimeData>> GetRelatedAnime(bool force = false)
        {
            List<RelatedAnimeData> output = force
            ? new List<RelatedAnimeData>()
            : await DataCache.RetrieveRelatedAnimeData(_animeId) ?? new List<RelatedAnimeData>();
            if (output.Count != 0) return output;

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var relationsNode = doc.DocumentNode.Descendants("table")
                .First(
                    node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        "anime_detail_related_anime");

            foreach (var row in relationsNode.Descendants("tr"))
            {
                var current = new RelatedAnimeData();
                current.WholeRelation = WebUtility.HtmlDecode(row.Descendants("td").First().InnerText.Trim()) + " ";
                var linkNode = row.Descendants("a").First();
                var link = linkNode.Attributes["href"].Value.Split('/');
                current.IsAnime = link[1] == "anime";
                current.Id = Convert.ToInt32(link[2]);
                current.Title = WebUtility.HtmlDecode(linkNode.InnerText.Trim());
                current.WholeRelation += current.Title;
                output.Add(current);
            }


            DataCache.SaveRelatedAnimeData(_animeId,output);

            return output;
        }
    }
}
