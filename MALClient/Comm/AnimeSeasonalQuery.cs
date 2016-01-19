using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using HtmlAgilityPack;
using MALClient.Items;

namespace MALClient.Comm
{
    class AnimeSeasonalQuery : Query
    {
        public AnimeSeasonalQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/anime/season"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime()
        {
            List<SeasonalAnimeData> output = await DataCache.RetrieveSeasonalData() ?? new List<SeasonalAnimeData>();
            if (output.Count == 0)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    Utils.GetMainPageInstance().SetStatus("Downloading data...");
                });

                string raw = await GetRequestResponse();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var mainNode =
                    doc.DocumentNode.Descendants("div")
                        .First(
                            node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value ==
                                "seasonal-anime-list js-seasonal-anime-list js-seasonal-anime-list-key-1 clearfix");

                var nodes = mainNode.ChildNodes.Where(node => node.Name == "div");




                int i = 0;
                foreach (var htmlNode in nodes)
                {
                    if (htmlNode.Attributes["class"]?.Value != "seasonal-anime js-seasonal-anime")
                        continue;

                    var imageNode =
                        htmlNode.Descendants("div")
                            .First(
                                node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "image");
                    var link = imageNode.ChildNodes.First(node => node.Name == "a").Attributes["href"].Value;
                    var img = imageNode.Attributes["style"].Value;
                    var scoreTxt =
                        htmlNode.Descendants("span")
                            .First(
                                node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "score")
                            .InnerText;
                    float score;
                    if (!float.TryParse(scoreTxt, out score))
                        score = 0;
                    output.Add(new SeasonalAnimeData
                    {
                        Title = WebUtility.HtmlDecode(imageNode.InnerText.Trim()), //there are some \n that we need to get rid of
                        MalLink = link,
                        Id = int.Parse(link.Substring(7).Split('/')[2]), //extracted from anime link
                        ImgUrl = img.Split('(', ')')[1], // from image style attr it's between ( )
                        Synopsis =
                            htmlNode.Descendants("div")
                                .First(
                                    node =>
                                        node.Attributes.Contains("class") &&
                                        node.Attributes["class"].Value == "synopsis js-synopsis")
                                .InnerHtml,
                        Score = score, //0 for N/A
                        Episodes =
                            htmlNode.Descendants("div")
                                .First(
                                    node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "eps")
                                .Descendants("a")
                                .First()
                                .InnerText.Split(new[] {" ",}, StringSplitOptions.RemoveEmptyEntries)[0],
                        Index = i,
                    });
                    i++;
                    if (i == 30)
                        break;
                }

                DataCache.SaveSeasonalData(output);
            }

            //Search in autheticated list for references
            //var loadedStuff = Utils.GetMainPageInstance().RetrieveLoadedAnime();
            //if (loadedStuff != null)
            //    {
            //        Dictionary<int, XElement> downloadedItems = loadedStuff.DownloadedAnime.ToDictionary(item => int.Parse(item.Element("series_animedb_id").Value));
            //        Dictionary<int,AnimeItem> loadedItems = loadedStuff.LoadedAnime.ToDictionary(item => item.Id);
            //        foreach (var seasonalAnimeData in output)
            //        {
            //            try
            //            {
            //                seasonalAnimeData.AnimeItemRef = loadedItems[seasonalAnimeData.Id];
            //            }
            //            catch (Exception)
            //            {
            //                try
            //                {
            //                    var animeXElement = downloadedItems[seasonalAnimeData.Id];
            //                    var animeItem = Utils.LoadAnimeItemFromXElement(animeXElement, true);
            //                    loadedStuff.AnimeItemLoaded(animeItem);
            //                    seasonalAnimeData.AnimeItemRef = animeItem;
            //                }
            //                catch (Exception)
            //                {
            //                    // no luck , we will have to load this item on the go
            //                }
            //            }
            //        }
            //    }

            //We are done.
            return output;
        }
    }
}
