using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using HtmlAgilityPack;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{

    public struct ProfileData
    {
        public string Username { get; set; }
        //Anime
        public int AnimeWatching { get; set; }
        public int AnimeCompleted { get; set; }
        public int AnimeOnHold { get; set; }
        public int AnimeDropped { get; set; }
        public int AnimePlanned { get; set; }
        public int AnimeTotal { get; set; }
        public int AnimeRewatched { get; set; }
        public int AnimeEpisodes { get; set; }
        //Manga
        public int MangaReading { get; set; }
        public int MangaCompleted { get; set; }
        public int MangaOnHold { get; set; }
        public int MangaDropped { get; set; }
        public int MangaPlanned { get; set; }
        public int MangaTotal { get; set; }
        public int MangaReread { get; set; }
        public int MangaVolumes { get; set; }
        public int MangaChapters{ get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {

        //http://cdn.myanimelist.net/images/userimages/{id}.jpg

        public ProfilePage()
        {
            this.InitializeComponent();
            PullData();
        }

        private async void PullData()
        {
            var data = await new MALProfileQuery().GetRequestResponse();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
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

            ProfileData profile = new ProfileData
            {
                AnimeWatching = int.Parse(watching[0].ChildNodes[1].InnerText),
                AnimeCompleted = int.Parse(completed[0].ChildNodes[1].InnerText),
                AnimeOnHold = int.Parse(onhold[0].ChildNodes[1].InnerText),
                AnimeDropped = int.Parse(dropped[0].ChildNodes[1].InnerText),
                AnimePlanned = int.Parse(plantowatch[0].ChildNodes[1].InnerText),

                AnimeTotal = int.Parse(total[0].ChildNodes[1].InnerText),
                AnimeEpisodes = int.Parse(episodes[0].ChildNodes[1].InnerText),
                AnimeRewatched = int.Parse(rewatched[0].ChildNodes[1].InnerText),
                //Manga
                MangaReading = int.Parse(reading[0].ChildNodes[1].InnerText),
                MangaCompleted = int.Parse(completed[1].ChildNodes[1].InnerText),
                MangaOnHold = int.Parse(onhold[1].ChildNodes[1].InnerText),
                MangaDropped = int.Parse(dropped[1].ChildNodes[1].InnerText),
                MangaPlanned = int.Parse(plantoread[0].ChildNodes[1].InnerText),

                MangaTotal = int.Parse(total[1].ChildNodes[1].InnerText),
                MangaReread = int.Parse(reread[0].ChildNodes[1].InnerText),
                MangaChapters = int.Parse(chapters[0].ChildNodes[1].InnerText),
                MangaVolumes = int.Parse(volumes[0].ChildNodes[1].InnerText),
            };
        }

    }
}
