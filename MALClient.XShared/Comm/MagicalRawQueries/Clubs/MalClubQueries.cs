using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.MagicalRawQueries.Clubs
{
    public static class MalClubQueries
    {
        public enum SearchCategory
        {
            All = 0,
            [EnumUtilities.Description("Actors & Artists")]
            ActorsArtists = 3,
            Anime = 1,
            Characters =4,
            [EnumUtilities.Description("Cities & Neighbourhoods")]
            CitiesNeighbourhoods =8,
            Companies = 5,
            Conventions = 2,
            Games = 6,
            Japan = 7,
            Manga = 10,
            Music = 9,
            Other = 12,
            Schools = 11,

        }

        public enum QueryType
        {
            All,
            My
        }

        public static async Task<List<MalClubEntry>> GetClubs(QueryType type, int page, SearchCategory category = SearchCategory.All,
            string searchQuery = null)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();


                var response =
                    await client.GetAsync(
                        type == QueryType.All
                            ? (category == SearchCategory.All
                                ? $"/clubs.php?p={page}"
                                : $"/clubs.php?catid={(int) type}&cn={searchQuery}&action=find")
                            : "/clubs.php?action=myclubs");

                if (!response.IsSuccessStatusCode)
                    return null;

                var output = new List<MalClubEntry>();

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());


                foreach (var clubRow in doc.WhereOfDescendantsWithClass("tr","table-data"))
                {
                    var current  = new MalClubEntry();

                    current.Id = clubRow.Descendants("a").First().Attributes["href"].Value.Split('=').Last();

                    var tds = clubRow.ChildNodes.Where(node => node.Name == "td").ToList();

                    var img = tds[0].Descendants("img").First();
                    try
                    {
                        var set = img.Attributes["data-srcset"].Value.Split(',').Last();
                        var pos = set.IndexOf('?');
                        set = set.Substring(0, pos);
                        set = Regex.Replace(set, @"\/r\/\d+x\d+", "");

                        current.ImgUrl = set;
                    }
                    catch (Exception)
                    {
                        //image scrapping
                    }

                    current.Name = img.Attributes["alt"].Value;

                    current.Description = WebUtility.HtmlDecode(tds[0]
                        .FirstOfDescendantsWithClass("div", "pt4 pb4 word-break").InnerText.Trim());

                    current.Members = tds[1].InnerText.Trim();
                    var comment = WebUtility.HtmlDecode(tds[2].InnerText.Trim());
                    var byPos = comment.LastIndexOf("by ");
                    if (byPos != -1)
                    {
                        current.LastCommentDate = comment.Substring(0, byPos);
                        current.LastCommentAuthor = comment.Substring(byPos+3);
                    }
                    current.LastPost = WebUtility.HtmlDecode(tds[3].InnerText.Trim());

                    if (type == QueryType.All)
                    {
                        if(tds[4].InnerText.Contains("Join"))
                            current.JoinType = MalClubEntry.JoinAction.Join;
                        else if (tds[4].InnerText.Contains("Request"))
                            current.JoinType = MalClubEntry.JoinAction.Request;
                        else if(tds[4].InnerText.Contains("Accept"))
                        {
                            current.JoinType = MalClubEntry.JoinAction.AcceptDeny;
                            current.JoinData = tds[4].Descendants("input")
                                .First(node => node.Attributes.Contains("name") &&
                                               node.Attributes["name"].Value == "id").Attributes["value"].Value;
                        }
                    }
                    else
                    {
                        current.JoinType = MalClubEntry.JoinAction.None;
                    }
                    output.Add(current);

                }

                return output;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> AcceptDenyInvitation(string id,bool accept)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("choice", accept ? "1" : "2"),
                    new KeyValuePair<string, string>("id", id),
                    new KeyValuePair<string, string>("inviter_user_id", "0"),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        "/clubs.php?action=invchoice", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static async Task<bool> JoinClub(string id)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("submitjoin", "Join Club"),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        $"/clubs.php?action=join&id={id}", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }


        }

        public static async Task<bool> RequestJoinClub(string id)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("submitjoin", "Request Access"),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        $"/clubs.php?action=request&id={id}", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
