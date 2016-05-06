using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Comm;
using MALClient.Models.Favourites;
using MALClient.Pages;

namespace MALClient.Models
{
    public class ProfileData
    {
        //Anime
        public int AnimeWatching { get; set; }
        public int AnimeCompleted { get; set; }
        public int AnimeOnHold { get; set; }
        public int AnimeDropped { get; set; }
        public int AnimePlanned { get; set; }
        //Manga
        public int MangaReading { get; set; }
        public int MangaCompleted { get; set; }
        public int MangaOnHold { get; set; }
        public int MangaDropped { get; set; }
        public int MangaPlanned { get; set; }
        //Days
        public float AnimeDays;
        public string AnimeDaysBind { get { return $"Days: {AnimeDays}"; } }
        public float MangaDays;
        public string MangaDaysBind { get { return $"Days: {MangaDays}"; } }      
        //Fav Anime
        public List<int> FavouriteAnime { get; set; } = new List<int>();
        //Fav Manga
        public List<int> FavouriteManga { get; set; } = new List<int>();
        //Fav Characters
        public List<FavCharacter> FavouriteCharacters { get; set; } = new List<FavCharacter>();
        //Fav Ppl
        public List<FavPerson> FavouritePeople { get; set; } = new List<FavPerson>();
        //Recent Anime
        public List<int> RecentAnime { get; set; } = new List<int>();
        //Recent Manga 
        public List<int> RecentManga { get; set; } = new List<int>();

        public bool WatchStatsDownloaded { get; private set; }
        public async Task PopulateWatchStats()
        {
            if (WatchStatsDownloaded)
                return;
            WatchStatsDownloaded = true;

            var animeStats = await new LibraryListQuery(Credentials.UserName,AnimeListWorkModes.Anime).GetProfileStats();
            var mangaStats = await new LibraryListQuery(Credentials.UserName,AnimeListWorkModes.Manga).GetProfileStats(false);

            if (animeStats != null)
            {
                AnimeWatching = int.Parse(animeStats.Element("user_watching").Value);
                AnimeCompleted = int.Parse(animeStats.Element("user_completed").Value);
                AnimeOnHold = int.Parse(animeStats.Element("user_onhold").Value);
                AnimeDropped = int.Parse(animeStats.Element("user_dropped").Value);
                AnimePlanned = int.Parse(animeStats.Element("user_plantowatch").Value);
                AnimeDays = float.Parse(animeStats.Element("user_days_spent_watching").Value);
            }

            //Manga
            if (mangaStats != null)
            {
                MangaReading = int.Parse(mangaStats.Element("user_reading").Value);
                MangaCompleted = int.Parse(mangaStats.Element("user_completed").Value);
                MangaOnHold = int.Parse(mangaStats.Element("user_onhold").Value);
                MangaDropped = int.Parse(mangaStats.Element("user_dropped").Value);
                MangaPlanned = int.Parse(mangaStats.Element("user_plantoread").Value);
                MangaDays = float.Parse(mangaStats.Element("user_days_spent_watching").Value);
            }
        }
    }
}
