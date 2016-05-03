using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;
using MALClient.Models.Favourites;
using Newtonsoft.Json;

namespace MALClient.Comm
{
    public class ProfileQuery : Query
    {
        public ProfileQuery()
        {
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/profile/{Credentials.UserName}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    Request = WebRequest.Create(Uri.EscapeUriString($"https://hummingbird.me/api/v1/users/{Credentials.UserName}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<ProfileData> GetProfileData(bool force = false)
        {
            var raw = await GetRequestResponse();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var current = new ProfileData();

                int i = 1;
                foreach (var recentNode in doc.DocumentNode.Descendants("div").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Profile:recentUpdateNode:class"]))
                {
                    if (i <= 3)
                    {
                        current.RecentAnime.Add(int.Parse(recentNode.Descendants("a").First().Attributes["href"].Value.Substring(8).Split('/')[2]));
                    }
                    else
                    {
                        current.RecentManga.Add(int.Parse(recentNode.Descendants("a").First().Attributes["href"].Value.Substring(8).Split('/')[2]));
                    }
                    i++;
                }
                try
                {
                    foreach (var favCharNode in doc.DocumentNode.Descendants("ul").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Profile:favCharacterNode:class"]).Descendants("li"))
                    {
                        var curr = new FavCharacter();
                        var imgNode = favCharNode.Descendants("a").First();
                        var styleString = imgNode.Attributes["style"].Value.Substring(22);
                        curr.ImgUrl = styleString.Replace("/r/80x120", "");
                        curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));
                        var infoNode = favCharNode.Descendants("div").Skip(1).First();
                        var nameNode = infoNode.Descendants("a").First();
                        curr.Name = nameNode.InnerText.Trim();
                        curr.Id = nameNode.Attributes["href"].Value.Substring(9).Split('/')[2];
                        var originNode = infoNode.Descendants("a").Skip(1).First();
                        curr.OriginatingShowName = originNode.InnerText.Trim();
                        curr.ShowId = originNode.Attributes["href"].Value.Split('/')[2];
                        curr.FromAnime = originNode.Attributes["href"].Value.Split('/')[1] == "anime";
                        current.FavouriteCharacters.Add(curr);
                    }
                }
                catch (Exception)
                {
                    //no favs
                }
                try
                {
                    foreach (var favMangaNode in doc.DocumentNode.Descendants("ul").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Profile:favMangaNode:class"]).Descendants("li"))
                    {
                        current.FavouriteManga.Add(int.Parse(favMangaNode.Descendants("a").First().Attributes["href"].Value.Substring(9).Split('/')[2]));
                    }
                }
                catch (Exception)
                {
                    //no favs
                }

                try
                {
                    foreach (var favAnimeNode in doc.DocumentNode.Descendants("ul").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Profile:favAnimeNode:class"]).Descendants("li"))
                    {
                        current.FavouriteAnime.Add(int.Parse(favAnimeNode.Descendants("a").First().Attributes["href"].Value.Substring(9).Split('/')[2]));
                    }
                }
                catch (Exception)
                {
                    //no favs
                }

                try
                {
                    foreach (var favPersonNode in doc.DocumentNode.Descendants("ul").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == HtmlClassMgr.ClassDefs["#Profile:favPeopleNode:class"]).Descendants("li"))
                    {
                        var curr = new FavPerson();
                        var aElems = favPersonNode.Descendants("a");
                        var styleString = aElems.First().Attributes["style"].Value.Substring(22);
                        curr.ImgUrl = styleString.Replace("/r/80x120", "");
                        curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));

                        curr.Name = aElems.Skip(1).First().InnerText.Trim();
                        curr.Id = aElems.Skip(1).First().Attributes["href"].Value.Substring(9).Split('/')[2];

                        current.FavouritePeople.Add(curr);
                    }
                }
                catch (Exception)
                {
                    //no favs
                }


                return current;
            }
            catch (Exception)
            {
                //hatml
            }

            return null;
        }

        public async Task<string> GetHummingBirdAvatarUrl()
        {
            string raw = await GetRequestResponse();
            return ((dynamic)JsonConvert.DeserializeObject(raw)).avatar.ToString();
        }
    }
}