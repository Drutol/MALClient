using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.ScrappedDetails;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Details
{
    public class CharacterDetailsQuery : Query
    {
        private readonly int _id;

        public CharacterDetailsQuery(int id)
        {
            _id = id;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/character/{id}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<CharacterDetailsData> GetCharacterDetails(bool force = false)
        {
            var possibleData = force ? null : await DataCache.RetrieveData<CharacterDetailsData>(_id.ToString(), "character_details", 30);
            if (possibleData != null)
                return possibleData;

            var output = new CharacterDetailsData();
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            
            output.Id = _id;
            try
            {
                var columns = doc.DocumentNode.Descendants("table").First().ChildNodes[1].ChildNodes.Where(node => node.Name == "td").ToList();
                var leftColumn = columns[0];
                var tables = leftColumn.Descendants("table");
                foreach (var table in tables)
                {
                    foreach (var descendant in table.Descendants("tr"))
                    {
                        var links = descendant.Descendants("a").ToList();
                        if (links[0].Attributes["href"].Value.StartsWith("/anime"))
                        {
                            var curr = new AnimeLightEntry { IsAnime = true };
                            curr.Id = int.Parse(links[0].Attributes["href"].Value.Split('/')[2]);
                            var img = links[0].Descendants("img").First().Attributes["src"].Value;
                            if (!img.Contains("questionmark"))
                            {
                                img = Regex.Replace(img, @"\/r\/\d+x\d+", "");
                                curr.ImgUrl = img.Substring(0, img.IndexOf('?'));
                            }
                            curr.Title = WebUtility.HtmlDecode(links[1].InnerText.Trim());
                            output.Animeography.Add(curr);
                        }
                        else
                        {
                            var curr = new AnimeLightEntry { IsAnime = false };
                            curr.Id = int.Parse(links[0].Attributes["href"].Value.Split('/')[2]);
                            var img = links[0].Descendants("img").First().Attributes["src"].Value;
                            if (!img.Contains("questionmark"))
                            {
                                img = Regex.Replace(img, @"\/r\/\d+x\d+", "");
                                curr.ImgUrl = img.Substring(0, img.IndexOf('?'));
                            }
                            curr.Title = WebUtility.HtmlDecode(links[1].InnerText.Trim());
                            output.Mangaography.Add(curr);
                        }
                    }
                }               
                var image = leftColumn.Descendants("img").First();
                if (image.Attributes.Contains("alt"))
                {
                    output.ImgUrl = image.Attributes["src"].Value;
                    output.Name = image.Attributes["alt"].Value;
                }
                else
                {
                    output.Name = WebUtility.HtmlDecode(doc.DocumentNode.Descendants("h1").First().InnerText.Trim());
                }
                output.Content = output.SpoilerContent = "";
                output.Content += WebUtility.HtmlDecode(leftColumn.LastChild.InnerText.Trim()) + "\n\n";
                foreach (var node in columns[1].ChildNodes)
                {
                    if (node.Name == "#text")
                        output.Content += WebUtility.HtmlDecode(node.InnerText.Trim());
                    else if (node.Name == "br")
                        output.Content += "\n";
                    else if (node.Name == "div" && node.Attributes.Contains("class") && node.Attributes["class"].Value == "spoiler")
                        output.SpoilerContent += WebUtility.HtmlDecode(node.InnerText.Trim());
                    else if (node.Name == "table")
                    {
                        foreach (var descendant in node.Descendants("tr"))
                        {
                            var current = new AnimeStaffPerson();
                            var img = descendant.Descendants("img").First();
                            var imgUrl = img.Attributes["src"].Value;
                            if (!imgUrl.Contains("questionmark"))
                            {
                                var pos = imgUrl.LastIndexOf("v");
                                if (pos != -1)
                                    imgUrl = imgUrl.Remove(pos, 1);

                            }
                            current.ImgUrl = imgUrl;
                            var info = descendant.Descendants("td").Last();
                            current.Id = info.ChildNodes[0].Attributes["href"].Value.Split('/')[2];
                            current.Name = WebUtility.HtmlDecode(info.ChildNodes[0].InnerText.Trim());
                            current.Notes = info.ChildNodes[2].InnerText;
                            output.VoiceActors.Add(current);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //html
            }
            

            DataCache.SaveData(output, _id.ToString(), "character_details");

            return output;
        }
    }
}
