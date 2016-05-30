using System.Collections.Generic;
using System.Threading.Tasks;
using MALClient.Comm;
using MALClient.Models.Favourites;
using MALClient.Pages;

namespace MALClient.Models
{
    public class MalUser
    {
        public string Name { get; set; }
        public string ImgUrl { get; set; }
    }

    public class MalComment
    {
        public MalUser User { get; set; } = new MalUser();
        public string Content { get; set; }
        public string Date { get; set; }
    }

    public class ProfileData
    {
        //
        //User details
        //
        public MalUser User { get; set; } = new MalUser();
        public string LastOnline { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string Location { get; set; }
        public string Joined { get; set; }
        public List<MalUser> Friends { get; set; } = new List<MalUser>();
        public List<MalComment> Comments { get; set; } = new List<MalComment>();


        //Days
        public float AnimeDays;
        public float MangaDays;
        //
        //Anime
        //
        public int AnimeWatching { get; set; }
        public int AnimeCompleted { get; set; }
        public int AnimeOnHold { get; set; }
        public int AnimeDropped { get; set; }
        public int AnimePlanned { get; set; }
        //AnimeAdditional
        public int AnimeTotal { get; set; }
        public int AnimeRewatched { get; set; }
        public int AnimeEpisodes { get; set; }
        public float AnimeMean { get; set; }
        //
        //Manga
        //
        public int MangaReading { get; set; }
        public int MangaCompleted { get; set; }
        public int MangaOnHold { get; set; }
        public int MangaDropped { get; set; }
        public int MangaPlanned { get; set; }
        //MangaAdditional
        public int MangaTotal { get; set; }
        public int MangaReread { get; set; }
        public int MangaChapters { get; set; }
        public int MangaVolumes { get; set; }
        public float MangaMean { get; set; }
        //
        public string AnimeDaysBind
        {
            get { return $"Days: {AnimeDays}"; }
        }

        public string AnimeMeanBind
        {
            get { return $"Mean: {AnimeMean}"; }
        }

        public string MangaDaysBind
        {
            get { return $"Days: {MangaDays}"; }
        }

        public string MangaMeanBind
        {
            get { return $"Mean: {MangaMean}"; }
        }

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