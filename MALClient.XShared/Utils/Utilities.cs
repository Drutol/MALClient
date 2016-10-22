using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Profile;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Utils
{
    public static class Utilities
    {
        private static readonly string[] SizeSuffixes = {"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public static string StatusToString(int status, bool manga = false, bool rewatch = false)
        {
            switch (status)
            {
                case 1:
                    return manga ? "Reading" : "Watching";
                case 2:
                    if (!rewatch)
                        return "Completed";
                    return manga ? "Rereading" : "Rewatching";
                case 3:
                    return "On hold";
                case 4:
                    return "Dropped";
                case 6:
                    return manga ? "Plan to read" : "Plan to watch";
                case 7:
                    return "All";
                case 8:
                    return "Airing";
                default:
                    return "Not Set";
            }
        }

        public static string StatusToShortString(int status, bool manga = false, bool rewatch = false)
        {
            switch (status)
            {
                case 1:
                    return manga ? "R" : "W";
                case 2:
                    return rewatch ? "Re" : "C";
                case 3:
                    return "H";
                case 4:
                    return "D";
                case 6:
                    return "P";
                case 7:
                    return "";
                case 8:
                    return "";
                default:
                    return "N/A";
            }
        }

        public static int StatusToInt(string status)
        {
            switch (status)
            {
                case "Reading":
                case "Watching":
                    return 1;
                case "Completed":
                    return 2;
                case "On hold":
                    return 3;
                case "Dropped":
                    return 4;
                case "Plan to read":
                case "Plan to watch":
                    return 6;
                case "All":
                    return 7;
                case "Airing":
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string DayToString(DayOfWeek day, bool ignoreOffset = false)
        {
            if (day < 0)
                return "";
            if (Settings.AirDayOffset != 0 && !ignoreOffset)
            {
                var sum = Settings.AirDayOffset + (int) day;
                if (sum > 6)
                    day = (DayOfWeek) sum - 7;
                else if (sum < 0)
                    day = (DayOfWeek) 7 + sum;
                else
                    day += Settings.AirDayOffset;
            }
            switch (day)
            {
                case DayOfWeek.Friday:
                    return "Fri";
                case DayOfWeek.Monday:
                    return "Mon";
                case DayOfWeek.Saturday:
                    return "Sat";
                case DayOfWeek.Sunday:
                    return "Sun";
                case DayOfWeek.Thursday:
                    return "Thu";
                case DayOfWeek.Tuesday:
                    return "Tue";
                case DayOfWeek.Wednesday:
                    return "Wed";
                default:
                    return "";
            }
        }

        public static DayOfWeek StringToDay(string day)
        {
            switch (day)
            {
                case "Fri":
                    return DayOfWeek.Friday;
                    ;
                case "Mon":
                    return DayOfWeek.Monday;
                    ;
                case "Sat":
                    return DayOfWeek.Saturday;
                    ;
                case "Sun":
                    return DayOfWeek.Sunday;
                    ;
                case "Thu":
                    return DayOfWeek.Thursday;
                    ;
                case "Tue":
                    return DayOfWeek.Tuesday;
                    ;
                case "Wed":
                    return DayOfWeek.Wednesday;
                    ;
            }
            return DayOfWeek.Friday;
        }



        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static int ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = date.ToUniversalTime() - origin;
            return (int) Math.Floor(diff.TotalSeconds);
        }



        /// <summary>
        ///     http://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
        /// </summary>
        public static string SizeSuffix(long value)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }
            if (value == 0)
            {
                return "0.0 bytes";
            }

            var mag = (int) Math.Log(value, 1024);
            var adjustedSize = (decimal) value/(1L << (mag*10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }






        public static string CleanAnimeTitle(string title)
        {
            var index = title.IndexOf('+');
            return index == -1 ? title : title.Substring(0, index);
        }

        public static string FirstCharToUpper(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (var i = 0; i <= n; d[i, 0] = i++) ;
            for (var j = 1; j <= m; d[0, j] = j++) ;

            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public static HamburgerButtons GetButtonForPage(PageIndex page)
        {
            switch (page)
            {
                case PageIndex.PageAnimeList:
                    return HamburgerButtons.AnimeList;
                case PageIndex.PageAnimeDetails:
                    return HamburgerButtons.AnimeList;
                case PageIndex.PageSettings:
                    return HamburgerButtons.Settings;
                case PageIndex.PageSearch:
                    return HamburgerButtons.AnimeSearch;
                case PageIndex.PageLogIn:
                    return HamburgerButtons.LogIn;
                case PageIndex.PageProfile:
                    return HamburgerButtons.Profile;
                case PageIndex.PageAbout:
                    return HamburgerButtons.About;
                case PageIndex.PageRecomendations:
                    return HamburgerButtons.Recommendations;
                case PageIndex.PageSeasonal:
                    return HamburgerButtons.Seasonal;
                case PageIndex.PageMangaList:
                    return HamburgerButtons.MangaList;
                case PageIndex.PageMangaSearch:
                    return HamburgerButtons.MangaSearch;
                case PageIndex.PageTopAnime:
                    return HamburgerButtons.TopAnime;
                case PageIndex.PageTopManga:
                    return HamburgerButtons.TopManga;
                case PageIndex.PageCalendar:
                    return HamburgerButtons.Calendar;
                case PageIndex.PageArticles:
                    return HamburgerButtons.Articles;
                case PageIndex.PageNews:
                    return HamburgerButtons.News;
                case PageIndex.PageMessanging:
                    return HamburgerButtons.Messanging;
                case PageIndex.PageHistory:
                    return HamburgerButtons.History;
                case PageIndex.PageForumIndex:
                    return HamburgerButtons.Forums;
                case PageIndex.PageCharacterSearch:
                    return HamburgerButtons.CharacterSearch;
                    case PageIndex.PageWallpapers:
                    return HamburgerButtons.Wallpapers;
                case PageIndex.PagePopularVideos:
                    return HamburgerButtons.PopularVideos;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }

        public static string DecodeXmlSynopsisDetail(string txt)
        {
            return
                Regex.Replace(txt, @"<[^>]+>|&nbsp;", "")
                    .Trim()
                    .Replace("[i]", "")
                    .Replace("[/i]", "")
                    .Replace("#039;", "'")
                    .Replace("&quot;", "\"")
                    .Replace("mdash;", "—")
                    .Replace("&amp;", "&");
        }

        public static string DecodeXmlSynopsisSearch(string txt)
        {
            return
                Regex.Replace(txt, @"<[^>]+>|&nbsp;", "")
                    .Trim()
                    .Replace("[i]", "")
                    .Replace("[/i]", "")
                    .Replace("#039;", "'")
                    .Replace("&quot;", "\"")
                    .Replace("&mdash;", "—")
                    .Replace("&amp;", "&");
        }

        public static FontAwesomeIcon BoardToIcon(ForumBoards board)
        {
            switch (board)
            {
                case ForumBoards.Updates:
                    return FontAwesomeIcon.Bullhorn;
                case ForumBoards.Guidelines:
                    return FontAwesomeIcon.Gavel;
                case ForumBoards.Support:
                    return FontAwesomeIcon.Support;
                case ForumBoards.Suggestions:
                    return FontAwesomeIcon.LightbulbOutline;
                case ForumBoards.Contests:
                    return FontAwesomeIcon.Trophy;
                case ForumBoards.NewsDisc:
                    return FontAwesomeIcon.NewspaperOutline;
                case ForumBoards.Recomms:
                    return FontAwesomeIcon.Gift;
                case ForumBoards.AnimeSeriesDisc:
                    return FontAwesomeIcon.FolderOutline;
                case ForumBoards.MangaSeriesDisc:
                    return FontAwesomeIcon.FolderOutline;
                case ForumBoards.AnimeDisc:
                    return FontAwesomeIcon.Television;
                case ForumBoards.MangaDisc:
                    return FontAwesomeIcon.Book;
                case ForumBoards.Intro:
                    return FontAwesomeIcon.CommentOutline;
                case ForumBoards.GamesTech:
                    return FontAwesomeIcon.Gamepad;
                case ForumBoards.Music:
                    return FontAwesomeIcon.Music;
                case ForumBoards.Events:
                    return FontAwesomeIcon.Glass;
                case ForumBoards.CasualDisc:
                    return FontAwesomeIcon.Coffee;
                case ForumBoards.Creative:
                    return FontAwesomeIcon.PictureOutline;
                case ForumBoards.ForumsGames:
                    return FontAwesomeIcon.PuzzlePiece;
                default:
                    return FontAwesomeIcon.None;
            }
        }

        public static string ShortDayToFullDay(string sub)
        {
            switch (sub)
            {
                case "Fri":
                    return "Friday";
                case "Mon":
                    return "Monday";
                case "Sat":
                    return "Saturday";
                case "Sun":
                    return "Sunday";
                case "Thu":
                    return "Thursday";
                case "Tue":
                    return "Tuesday";
                case "Wed":
                    return "Wednesday";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Converts yyyy-MM-dd string to season, 1 - winter 4 - fall , 0 - error
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DateToSeason(string date)
        {
            try
            {
                var month = int.Parse(date.Split('-')[1]);
                if (month > 0 && month < 3)
                    return 1;
                if (month >= 3 && month < 6)
                    return 2;
                if (month >= 6 && month < 9)
                    return 3;
                return 4;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string SeasonToCapitalLetterWithYear(string date)
        {
            var season = DateToSeason(date);
            var year = date.Substring(2, 2);
            if (season == 1)
                return "Winter " + year;
            if (season == 2)
                return "Spring " + year;
            if (season == 3)
                return "Summer " + year;
            if (season == 4)
                return "Fall " + year;
            return "";
        }

        public static HtmlNode FirstWithClass(this IEnumerable<HtmlNode> doc, string targettedClass)
        {
            return
                doc.First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == targettedClass);
        }

        public static HtmlNode FirstOfDescendantsWithClass(this HtmlDocument doc, string descendants,
            string targettedClass)
        {
            return
                doc.DocumentNode.Descendants(descendants)
                    .First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == targettedClass);
        }

        public static HtmlNode FirstOfDescendantsWithId(this HtmlDocument doc, string descendants, string targettedId)
        {
            return
                doc.DocumentNode.Descendants(descendants)
                    .First(node => node.Attributes.Contains("id") && node.Attributes["id"].Value == targettedId);
        }

        public static HtmlNode FirstOfDescendantsWithClass(this HtmlNode doc, string descendants, string targettedClass)
        {
            return
                doc.Descendants(descendants)
                    .First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == targettedClass);
        }

        public static IEnumerable<HtmlNode> WhereOfDescendantsWithClass(this HtmlDocument doc, string descendants,
            string targettedClass)
        {
            return
                doc.DocumentNode.Descendants(descendants)
                    .Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == targettedClass);
        }

        public static IEnumerable<HtmlNode> WhereOfDescendantsWithClass(this HtmlNode doc, string descendants,
            string targettedClass)
        {
            return
                doc.Descendants(descendants)
                    .Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == targettedClass);
        }

        public static void ForEach<T>(this IEnumerable<T> source,Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static void IndexedForEach<T>(this IEnumerable<T> source,Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

    }
}