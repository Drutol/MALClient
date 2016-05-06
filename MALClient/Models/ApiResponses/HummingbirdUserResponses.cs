using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.ApiResponses
{
    //Genreated online from hummingbird json responses
    //Basic profile

    public class HumFavouriteAnime
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int item_id { get; set; }
        public string item_type { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int fav_rank { get; set; }
    }

    public class HumProfileData
    {
        public string name { get; set; }
        public object waifu { get; set; }
        public object waifu_or_husbando { get; set; }
        public string waifu_slug { get; set; }
        public string waifu_char_id { get; set; }
        public object location { get; set; }
        public object website { get; set; }
        public string avatar { get; set; }
        public string cover_image { get; set; }
        public string about { get; set; }
        public string bio { get; set; }
        public int karma { get; set; }
        public int life_spent_on_anime { get; set; }
        public bool show_adult_content { get; set; }
        public string title_language_preference { get; set; }
        public string last_library_update { get; set; }
        public bool following { get; set; }
        public List<HumFavouriteAnime> favorites { get; set; }
    }


    //Feed stuff

    public class HumUser
    {
        public string name { get; set; }
        public string url { get; set; }
        public string avatar { get; set; }
        public string avatar_small { get; set; }
        public bool nb { get; set; }
    }

    public class HumStoryMediaElement //aka anime
    {
        public int id { get; set; }
        public int mal_id { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string alternate_title { get; set; }
        public int episode_count { get; set; }
        public int? episode_length { get; set; }
        public string cover_image { get; set; }
        public string synopsis { get; set; }
        public string show_type { get; set; }
        public string started_airing { get; set; }
        public string finished_airing { get; set; }
        public double community_rating { get; set; }
        public string age_rating { get; set; }
        public List<HumGenre> genres { get; set; }
    }

    public class Substory
    {
        public int id { get; set; }
        public string substory_type { get; set; }
        public string created_at { get; set; }
        public string new_status { get; set; }
        public object permissions { get; set; } //ignored
        public string episode_number { get; set; }
        public object service { get; set; } //ignored
        public string comment { get; set; }
    }

    public class HumStoryPoster
    {
        public string name { get; set; }
        public string url { get; set; }
        public string avatar { get; set; }
        public string avatar_small { get; set; }
        public bool nb { get; set; }
    }

    public class HumStoryObject
    {
        public int id { get; set; }
        public string story_type { get; set; }
        public HumUser user { get; set; }
        public string updated_at { get; set; }
        public HumStoryMediaElement media { get; set; }
        public int substories_count { get; set; }
        public List<Substory> substories { get; set; }
        public bool? self_post { get; set; }
        public HumStoryPoster poster { get; set; }
    }
}
