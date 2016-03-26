using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;

namespace MALClient.Comm
{
    public class AnimeDetailsAnnQuery : Query
    {
        private readonly int _id;
        private readonly string _rootTitle;

        public AnimeDetailsAnnQuery(string title, int id, string rootTitle)
        {
            title = title.Replace('+', ' ');
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://cdn.animenewsnetwork.com/encyclopedia/api.xml?title=~{title}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _id = id;
            _rootTitle = rootTitle;
        }

        public async Task<AnimeDetailsData> GetGeneralDetailsData(bool force = false)
        {
            var possibleData = force ? null : await DataCache.RetrieveAnimeGeneralDetailsData(_id);
            if (possibleData != null)
                return possibleData;

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;

            AnimeDetailsData output;

            try
            {
                var data = XDocument.Parse(raw);
                var nodes = data.Element("ann").Elements("anime");
                //there may be many things , maybe we are lucky enough to grab original title
                var node =
                    nodes.FirstOrDefault(
                        element =>
                            string.Equals(element.Attribute("name").Value, _rootTitle,
                                StringComparison.CurrentCultureIgnoreCase));

                if (node == null)
                    node = nodes.First();

                output = new AnimeDetailsData
                {
                    AnnId = node.Attribute("id").Value,
                    Genres =
                        node.Elements("info")
                            .Where(element => element.Attribute("type").Value == "Genres")
                            .Select(element => element.Value)
                            .ToList(),
                    Episodes = node.Elements("episode").Select(element => element.Value).ToList(),
                    OPs =
                        node.Elements("info")
                            .Where(element => element.Attribute("type").Value == "Opening Theme")
                            .Select(element => element.Value)
                            .ToList(),
                    EDs =
                        node.Elements("info")
                            .Where(element => element.Attribute("type").Value == "Ending Theme")
                            .Select(element => element.Value)
                            .ToList()
                };

                DataCache.SaveAnimeDetails(_id, output);

                return output;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}