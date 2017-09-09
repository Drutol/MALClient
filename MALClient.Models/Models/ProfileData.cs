using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MALClient.Models.Models.Favourites;
using Newtonsoft.Json;

namespace MALClient.Models.Models
{
    [DataContract]
    public class MalUser
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ImgUrl { get; set; }

        private sealed class NameEqualityComparer : IEqualityComparer<MalUser>
        {
            public bool Equals(MalUser x, MalUser y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Name, y.Name,StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(MalUser obj)
            {
                return obj.Name?.GetHashCode() ?? 0;
            }
        }

        public static IEqualityComparer<MalUser> NameComparer { get; } = new NameEqualityComparer();
    }

    public class MalComment
    {
        public MalUser User { get; set; } = new MalUser();
        public string Content { get; set; }
        public string Date { get; set; }
        public string Id { get; set; }
        public bool CanDelete { get; set; }
        public string ComToCom { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }

    public class ProfileData
    {
        [JsonIgnore]
        public bool Cached { get; set; }
        //
        //User details
        //
        public MalUser User { get; } = new MalUser();
        public string ProfileMemId { get; set; }
        public List<Tuple<string, string>> Details { get; } = new List<Tuple<string, string>>();
        public List<MalUser> Friends { get;} = new List<MalUser>();
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
        public string AnimeDaysBind => $"Days: {AnimeDays}";

        public string AnimeMeanBind => $"Mean: {AnimeMean}";

        public string MangaDaysBind => $"Days: {MangaDays}";

        public string MangaMeanBind => $"Mean: {MangaMean}";

        public string HtmlContent { get; set; }

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

        public bool IsFriend { get; set; }
        public bool CanAddFriend { get; set; }
    }
}