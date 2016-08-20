using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Items;
using MalClient.Shared.Models.Favourites;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Search
{
    public class CharactersSearchQuery : Query
    {

        public CharactersSearchQuery(string query)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/character.php?q={query}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<AnimeCharacter>> GetSearchResults()
        {
            var output = new List<AnimeCharacter>();

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            try
            {
                foreach (var row in doc.DocumentNode.Descendants("table").First().Descendants("tr").Skip(1))
                {
                    try
                    {
                        var character = new AnimeCharacter();
                        var tds = row.Descendants("td").ToList();
                        var link = tds[1].Descendants("a").First();
                        character.Id = link.Attributes["href"].Value.Split('/')[2];
                        character.Name = WebUtility.HtmlDecode(link.InnerText.Trim());
                        var smalls = tds[1].Descendants("small");
                        if (smalls.Any())
                            character.Notes = WebUtility.HtmlDecode(smalls.Last().InnerText);

                        var img = tds[0].Descendants("img").First().Attributes["src"].Value;
                        if (!img.Contains("questionmark"))
                        {
                            img = Regex.Replace(img, @"\/r\/\d+x\d+", "");
                            character.ImgUrl = img.Substring(0, img.IndexOf('?'));
                        }

                        output.Add(character);
                    }
                    catch (Exception)
                    {
                        //
                    }

                    
                }
            }
            catch (Exception)
            {
                //
            }

            return output;
        }
    }
}
