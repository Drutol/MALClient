using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MalClient.Shared.Comm.Profile;
using MalClient.Shared.Models.Favourites;

namespace MalClient.Shared.Models
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
        public string Id { get; set; }
        public bool CanDelete { get; set; }
        public string ComToCom { get; set; }
    }

    public class ProfileData
    {
        //
        //User details
        //
        public MalUser User { get; } = new MalUser();
        public string ProfileMemId { get; set; }
        public List<Tuple<string, string>> Details { get; } = new List<Tuple<string, string>>();
        public List<MalUser> Friends { get;} = new List<MalUser>();
        public List<MalComment> Comments { get; private set; } = new List<MalComment>();


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
        public string AnimeDaysBind => $"Days: {AnimeDays}";

        public string AnimeMeanBind => $"Mean: {AnimeMean}";

        public string MangaDaysBind => $"Days: {MangaDays}";

        public string MangaMeanBind => $"Mean: {MangaMean}";

        //Fav Anime
        public List<int> FavouriteAnime { get; } = new List<int>();
        //Fav Manga
        public List<int> FavouriteManga { get; } = new List<int>();
        //Fav Characters
        public List<AnimeCharacter> FavouriteCharacters { get; } = new List<AnimeCharacter>();
        //Fav Ppl
        public List<AnimeStaffPerson> FavouritePeople { get; } = new List<AnimeStaffPerson>();
        //Recent Anime
        public List<int> RecentAnime { get;} = new List<int>();
        //Recent Manga 
        public List<int> RecentManga { get; } = new List<int>();

        public async Task UpdateComments()
        {
            Comments = await new ProfileQuery(false, User.Name).GetComments();
        }
    }
}