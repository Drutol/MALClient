using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    internal class MALProfileQuery : Query
    {
        public MALProfileQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/profile/{Creditentials.UserName}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<ProfileData> GetProfileData(bool force = false)
        {
            var raw = await GetRequestResponse();


            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                //AnimeStats
                var watching = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Watching") && li.ChildNodes.Count == 2).ToList();
                var plantowatch = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Plan to Watch") && li.ChildNodes.Count == 2).ToList();
                var rewatched = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Rewatched") && li.ChildNodes.Count == 2).ToList();
                var episodes = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Episodes") && li.ChildNodes.Count == 2).ToList();
                //MangaStats
                var reading = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Reading") && li.ChildNodes.Count == 2).ToList();
                var plantoread = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Plan to Read") && li.ChildNodes.Count == 2).ToList();
                var reread = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Reread") && li.ChildNodes.Count == 2).ToList();
                var chapters = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Chapters") && li.ChildNodes.Count == 2).ToList();
                var volumes = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Volumes") && li.ChildNodes.Count == 2).ToList();
                //Shared
                var total = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Total Entries") && li.ChildNodes.Count == 2).ToList();
                var completed = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Completed") && li.ChildNodes.Count == 2).ToList();
                var onhold = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("On-Hold") && li.ChildNodes.Count == 2).ToList();
                var dropped = doc.DocumentNode.Descendants("li")
                    .Where(li => li.InnerText.Contains("Dropped") && li.ChildNodes.Count == 2).ToList();
                //Days
                var days = doc.DocumentNode.Descendants("div")
                    .Where(div => div.InnerText.Contains("Days:") && div.ChildNodes.Count == 2).ToList();
                var currernt = new ProfileData
                {
                    //Anime
                    AnimeWatching = int.Parse(watching[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimeCompleted = int.Parse(completed[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimeOnHold = int.Parse(onhold[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimeDropped = int.Parse(dropped[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimePlanned = int.Parse(plantowatch[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimeTotal = int.Parse(total[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimeEpisodes = int.Parse(episodes[0].ChildNodes[1].InnerText.Replace(",", "")),
                    AnimeRewatched = int.Parse(rewatched[0].ChildNodes[1].InnerText.Replace(",", "")),
                    //Manga
                    MangaReading = int.Parse(reading[0].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaCompleted = int.Parse(completed[1].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaOnHold = int.Parse(onhold[1].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaDropped = int.Parse(dropped[1].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaPlanned = int.Parse(plantoread[0].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaTotal = int.Parse(total[1].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaReread = int.Parse(reread[0].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaChapters = int.Parse(chapters[0].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaVolumes = int.Parse(volumes[0].ChildNodes[1].InnerText.Replace(",", "")),
                    //Days
                    AnimeDays = float.Parse(days[0].ChildNodes[1].InnerText.Replace(",", "")),
                    MangaDays = float.Parse(days[1].ChildNodes[1].InnerText.Replace(",", ""))
                };
                return currernt;
            }
            catch (Exception)
            {
                //hatml
            }

            return null;
        }
    }
}