using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.ApiResponses;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Profile
{
    public class ProfileQuery : Query
    {
        private readonly string _userName;

        public ProfileQuery(bool feed = false, string userName = "")
        {
            if (string.IsNullOrEmpty(userName))
                userName = Credentials.UserName;
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/profile/{userName}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    Request =
                        WebRequest.Create(
                            Uri.EscapeUriString(
                                $"https://hummingbird.me/api/v1/users/{Credentials.UserName}{(feed ? "/feed" : "")}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _userName = userName;
        }

        public async Task<ProfileData> GetProfileData(bool force = false, bool updateFavsOnly = false)
        {
            try
            {


                ProfileData possibleData = null;
                if (!force)
                    possibleData = await DataCache.RetrieveProfileData(_userName);
                if (possibleData != null)
                {
                    possibleData.Cached = true;
                    return possibleData;
                }
                var raw = !updateFavsOnly
                    ? await (await (await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync())
                        .GetAsync($"/profile/{_userName}")).Content.ReadAsStringAsync()
                    : await GetRequestResponse();
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var current = new ProfileData {User = {Name = _userName}};

                #region Recents

                try
                {
                    var i = 1;
                    foreach (
                        var recentNode in
                        doc.DocumentNode.Descendants("div")
                            .Where(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    "statistics-updates di-b w100 mb8"))
                    {
                        if (i <= 3)
                        {
                            current.RecentAnime.Add(
                                int.Parse(
                                    recentNode.Descendants("a").First().Attributes["href"].Value.Substring(8)
                                        .Split('/')[2]));
                        }
                        else
                        {
                            current.RecentManga.Add(
                                int.Parse(
                                    recentNode.Descendants("a").First().Attributes["href"].Value.Substring(8)
                                        .Split('/')[2]));
                        }
                        i++;
                    }
                }
                catch (Exception)
                {
                    //no recents
                }

                #endregion

                #region FavChar

                try
                {
                    foreach (
                        var favCharNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    "favorites-list characters")
                            .Descendants("li"))
                    {
                        var curr = new AnimeCharacter();
                        var imgNode = favCharNode.Descendants("a").First();
                        var styleString = imgNode.Attributes["style"].Value.Substring(22);
                        curr.ImgUrl = styleString.Replace("/r/80x120", "");
                        curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));
                        var infoNode = favCharNode.Descendants("div").Skip(1).First();
                        var nameNode = infoNode.Descendants("a").First();
                        curr.Name = nameNode.InnerText.Trim();
                        curr.Id = nameNode.Attributes["href"].Value.Substring(9).Split('/')[2];
                        var originNode = infoNode.Descendants("a").Skip(1).First();
                        curr.Notes = originNode.InnerText.Trim();
                        curr.ShowId = originNode.Attributes["href"].Value.Split('/')[2];
                        curr.FromAnime = originNode.Attributes["href"].Value.Split('/')[1] == "anime";
                        current.FavouriteCharacters.Add(curr);
                    }
                }
                catch (Exception)
                {
                    //no favs
                }

                #endregion

                #region FavManga

                try
                {
                    foreach (
                        var favMangaNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    "favorites-list manga")
                            .Descendants("li"))
                    {
                        current.FavouriteManga.Add(
                            int.Parse(
                                favMangaNode.Descendants("a").First().Attributes["href"].Value.Substring(9).Split('/')[2
                                ]));
                    }
                }
                catch (Exception)
                {
                    //no favs
                }

                #endregion

                #region FavAnime

                try
                {
                    foreach (
                        var favAnimeNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    "favorites-list anime")
                            .Descendants("li"))
                    {
                        current.FavouriteAnime.Add(
                            int.Parse(
                                favAnimeNode.Descendants("a").First().Attributes["href"].Value.Substring(9).Split('/')[2
                                ]));
                    }
                }
                catch (Exception)
                {
                    //no favs
                }

                #endregion

                #region FavPpl

                try
                {
                    foreach (
                        var favPersonNode in
                        doc.DocumentNode.Descendants("ul")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value ==
                                    "favorites-list people")
                            .Descendants("li"))
                    {
                        var curr = new AnimeStaffPerson();
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

                #endregion

                #region Stats

                if (!updateFavsOnly)
                    try
                    {
                        var animeStats = doc.FirstOfDescendantsWithClass("div", "stats anime");
                        var generalStats = animeStats.Descendants("div").First().Descendants("div");
                        current.AnimeDays = float.Parse(generalStats.First().InnerText.Substring(5).Trim(),
                            NumberStyles.Any, CultureInfo.InvariantCulture);
                        current.AnimeMean = float.Parse(generalStats.Last().InnerText.Substring(11).Trim(),
                            NumberStyles.Any, CultureInfo.InvariantCulture);
                        var i = 0;

                        #region AnimeStats

                        foreach (
                            var htmlNode in
                            animeStats.FirstOfDescendantsWithClass("ul", "stats-status fl-l").Descendants("li"))
                        {
                            switch (i)
                            {
                                case 0:
                                    current.AnimeWatching =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;

                                case 1:
                                    current.AnimeCompleted =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 2:
                                    current.AnimeOnHold =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 3:
                                    current.AnimeDropped =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 4:
                                    current.AnimePlanned =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                            }
                            i++;
                        }
                        //left stats done now right
                        i = 0;
                        foreach (
                            var htmlNode in animeStats.FirstOfDescendantsWithClass("ul", "stats-data fl-r")
                                .Descendants("li"))
                        {
                            switch (i)
                            {
                                case 0:
                                    current.AnimeTotal =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;

                                case 1:
                                    current.AnimeRewatched =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 2:
                                    current.AnimeEpisodes =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                            }
                            i++;
                        }
                        //we are done with anime

                        #endregion

                        i = 0;
                        animeStats = doc.FirstOfDescendantsWithClass("div", "stats manga");
                        generalStats = animeStats.Descendants("div").First().Descendants("div");
                        current.MangaDays = float.Parse(generalStats.First().InnerText.Substring(5).Trim(),
                            NumberStyles.Any, CultureInfo.InvariantCulture);
                        current.MangaMean = float.Parse(generalStats.Last().InnerText.Substring(11).Trim(),
                            NumberStyles.Any, CultureInfo.InvariantCulture);

                        #region MangaStats

                        foreach (
                            var htmlNode in
                            animeStats.FirstOfDescendantsWithClass("ul", "stats-status fl-l").Descendants("li"))
                        {
                            switch (i)
                            {
                                case 0:
                                    current.MangaReading =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;

                                case 1:
                                    current.MangaCompleted =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 2:
                                    current.MangaOnHold =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 3:
                                    current.MangaDropped =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 4:
                                    current.MangaPlanned =
                                        int.Parse(htmlNode.Descendants("span").First().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                            }
                            i++;
                        }
                        //left stats done now right
                        i = 0;
                        foreach (
                            var htmlNode in animeStats.FirstOfDescendantsWithClass("ul", "stats-data fl-r")
                                .Descendants("li"))
                        {
                            switch (i)
                            {
                                case 0:
                                    current.MangaTotal =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;

                                case 1:
                                    current.MangaReread =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 2:
                                    current.MangaChapters =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                                case 3:
                                    current.MangaVolumes =
                                        int.Parse(htmlNode.Descendants("span").Last().InnerText.Trim()
                                            .Replace(",", ""));
                                    break;
                            }
                            i++;
                        }
                        //we are done with manga

                        #endregion
                    }
                    catch (Exception e)
                    {
                        //hatml
                    }

                #endregion

                #region LeftSideBar

                if (!updateFavsOnly)
                    try
                    {
                        var sideInfo =
                            doc.FirstOfDescendantsWithClass("ul", "user-status border-top pb8 mb4").Descendants("li")
                                .ToList();
                        try
                        {
                            foreach (var htmlNode in sideInfo)
                            {
                                var left = WebUtility.HtmlDecode(htmlNode.FirstChild.InnerText);
                                if (left == "Supporter")
                                    continue;
                                current.Details.Add(new Tuple<string, string>(left,
                                    WebUtility.HtmlDecode(htmlNode.LastChild.InnerText)));
                            }
                        }
                        catch (Exception)
                        {

                        }
                        current.User.ImgUrl =
                            doc.FirstOfDescendantsWithClass("div", "user-image mb8").Descendants("img").First()
                                .Attributes["src"].Value;

                    }
                    catch (Exception)
                    {
                        //???
                    }

                #endregion

                #region Friends

                if (!updateFavsOnly)
                    try
                    {
                        var friends = doc.FirstOfDescendantsWithClass("div", "user-friends pt4 pb12").Descendants("a");
                        foreach (var friend in friends)
                        {
                            var curr = new MalUser();
                            var styleString = friend.Attributes["data-bg"].Value;
                            curr.ImgUrl = styleString.Replace("/r/76x120", "");
                            curr.ImgUrl = curr.ImgUrl.Substring(0, curr.ImgUrl.IndexOf('?'));

                            curr.Name = friend.InnerText;
                            current.Friends.Add(curr);
                        }
                    }
                    catch (Exception)
                    {
                        //
                    }

                #endregion

                #region Comments

                if (!updateFavsOnly)
                    try
                    {
                        var commentBox = doc.FirstOfDescendantsWithClass("div", "user-comments mt24 pt24");
                        foreach (var comment in commentBox.WhereOfDescendantsWithClass("div", "comment clearfix"))
                        {
                            var curr = new MalComment();
                            var imgNode = comment.Descendants("img").First();
                            if (imgNode.Attributes.Contains("srcset"))
                                curr.User.ImgUrl = imgNode.Attributes["srcset"].Value.Split(',').Last()
                                    .Replace("2x", "").Trim();
                            else if (imgNode.Attributes.Contains("data-src"))
                                curr.User.ImgUrl = imgNode.Attributes["data-src"].Value;


                            var textBlock = comment.Descendants("div").First();
                            var header = textBlock.Descendants("div").First();
                            curr.User.Name = header.ChildNodes[1].InnerText;
                            curr.Date = header.ChildNodes[3].InnerText;
                            curr.Content =
                                WebUtility.HtmlDecode(textBlock.Descendants("div").Skip(1).First().InnerText.Trim());
                            var postActionNodes = comment.WhereOfDescendantsWithClass("a", "ml8");
                            var convNode =
                                postActionNodes.FirstOrDefault(node => node.InnerText.Trim() == "Conversation");
                            if (convNode != null)
                            {
                                curr.ComToCom =
                                    WebUtility.HtmlDecode(convNode.Attributes["href"].Value.Split('?').Last());
                            }
                            var deleteNode = postActionNodes.FirstOrDefault(node => node.InnerText.Trim() == "Delete");
                            if (deleteNode != null)
                            {
                                curr.CanDelete = true;
                                curr.Id =
                                    deleteNode.Attributes["onclick"].Value.Split(new char[] {'(', ')'},
                                        StringSplitOptions.RemoveEmptyEntries).Last();
                            }
                            foreach (var img in comment.Descendants("img").Skip(1))
                            {
                                if (img.Attributes.Contains("src"))
                                    curr.Images.Add(img.Attributes["src"].Value);
                            }
                            current.Comments.Add(curr);
                        }
                    }
                    catch (Exception e)
                    {
                        //no comments
                    }

                #endregion

                try
                {
                    current.ProfileMemId = doc.DocumentNode.Descendants("input")
                        .First(node => node.Attributes.Contains("name") &&
                                       node.Attributes["name"].Value == "profileMemId")
                        .Attributes["value"].Value;
                }
                catch (Exception)
                {
                    //restricted
                }

                try
                {
                    current.HtmlContent = doc.FirstOfDescendantsWithClass("div", "profile-about-user js-truncate-inner").OuterHtml;
                }
                catch (Exception)
                {
                    //
                }


                if (_userName.Equals(Credentials.UserName,StringComparison.CurrentCultureIgnoreCase)) //umm why do we need someone's favs?
                {
                    FavouritesManager.ForceNewSet(FavouriteType.Anime,
                        current.FavouriteAnime.Select(i => i.ToString()).ToList());
                    FavouritesManager.ForceNewSet(FavouriteType.Manga,
                        current.FavouriteManga.Select(i => i.ToString()).ToList());
                    FavouritesManager.ForceNewSet(FavouriteType.Character,
                        current.FavouriteCharacters.Select(i => i.Id).ToList());
                    FavouritesManager.ForceNewSet(FavouriteType.Person,
                        current.FavouritePeople.Select(i => i.Id).ToList());
                }

                current.IsFriend =
                    doc.FirstOrDefaultOfDescendantsWithClass("a", "icon-user-function icon-remove js-user-function") != null;

                current.CanAddFriend =
                    doc.FirstOrDefaultOfDescendantsWithClass("a", "icon-user-function icon-request js-user-function") != null;

                if (!updateFavsOnly)
                    DataCache.SaveProfileData(_userName, current);
                return current;
            }
            catch (Exception e)
            {
                ResourceLocator.TelemetryProvider.LogEvent($"Profile Query Crash: {_userName}, {e}");
                ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                    "Hmm, you have encountered bug that'm hunting. I've just sent report to myself. If everything goes well it should be gone in next release :). Sorry for inconvenience!",
                    "Ooopsies!");
            }
            return new ProfileData();
        }

        public async Task<List<MalComment>> GetComments()
        {
            var raw = await (await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync()).GetAsync($"/profile/{_userName}");
            var doc = new HtmlDocument();
            doc.LoadHtml(await raw.Content.ReadAsStringAsync());
            var output = new List<MalComment>();
            try
            {
                var commentBox = doc.FirstOfDescendantsWithClass("div", "user-comments mt24 pt24");
                foreach (var comment in commentBox.WhereOfDescendantsWithClass("div", "comment clearfix"))
                {
                    var curr = new MalComment();
                    var imgNode = comment.Descendants("img").First();
                    if(imgNode.Attributes.Contains("srcset"))
                        curr.User.ImgUrl = imgNode.Attributes["srcset"].Value.Split(',').Last().Replace("2x","").Trim();
                    else if (imgNode.Attributes.Contains("data-src"))
                        curr.User.ImgUrl = imgNode.Attributes["data-src"].Value;

                   
                    var textBlock = comment.Descendants("div").First();
                    var header = textBlock.Descendants("div").First();
                    curr.User.Name = header.ChildNodes[1].InnerText;
                    curr.Date = header.ChildNodes[3].InnerText;
                    curr.Content = WebUtility.HtmlDecode(textBlock.Descendants("div").Skip(1).First().InnerText.Trim());
                    var postActionNodes = comment.WhereOfDescendantsWithClass("a", "ml8");
                    var convNode = postActionNodes.FirstOrDefault(node => node.InnerText.Trim() == "Conversation");
                    if (convNode != null)
                    {
                        curr.ComToCom = WebUtility.HtmlDecode(convNode.Attributes["href"].Value.Split('?').Last());
                    }
                    var deleteNode = postActionNodes.FirstOrDefault(node => node.InnerText.Trim() == "Delete");
                    if(deleteNode != null)
                    {
                        curr.CanDelete = true;
                        curr.Id =
                            deleteNode.Attributes["onclick"].Value.Split(new char[] { '(', ')' },
                                StringSplitOptions.RemoveEmptyEntries).Last();
                    }
                    output.Add(curr);
                }
            }
            catch (Exception)
            {
                //no comments
            }

            await Task.Run( async () =>
            {
                var data = await DataCache.RetrieveProfileData(_userName);
                if (data != null)
                {
                    data.Comments = output;
                }
                DataCache.SaveProfileData(_userName, data);
            });

            return output;
        }

        public async Task<string> GetHummingBirdAvatarUrl()
        {
            var raw = await GetRequestResponse();
            return ((dynamic)JsonConvert.DeserializeObject(raw)).avatar.ToString();
        }

        public async Task<HumProfileData> GetHumProfileData(bool force = false)
        {
            var raw = await GetRequestResponse();
            return JsonConvert.DeserializeObject<HumProfileData>(raw,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public async Task<List<HumStoryObject>> GetHumFeedData(bool force = false)
        {
            var raw = await GetRequestResponse();
            return JsonConvert.DeserializeObject<List<HumStoryObject>>(raw,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }

    public static class ProfileDataHelper
    {
        public static async Task UpdateComments(this ProfileData data)
        {
            data.Comments = await new ProfileQuery(false, data.User.Name).GetComments();
        }
    }
}