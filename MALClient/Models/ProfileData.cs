using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Favourites;

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
        public int MangaChapters { get; set; }
        //Days
        public float AnimeDays { get; set; }
        public float MangaDays { get; set; }      
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
    }
}
