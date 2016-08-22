using System.Collections.Generic;

namespace MalClient.Shared.Models.ApiResponses
{
    public class HumGenre
    {
        public string name { get; set; }
    }

    public class HumAnime
    {
        public int id { get; set; }
        public int mal_id { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string alternate_title { get; set; }
        public int episode_count { get; set; }
        public int episode_length { get; set; }
        public string cover_image { get; set; }
        public string synopsis { get; set; }
        public string show_type { get; set; }
        public string started_airing { get; set; }
        public string finished_airing { get; set; }
        public double community_rating { get; set; }
        public string age_rating { get; set; }
        public List<HumGenre> genres { get; set; }
    }

    public class HumRating
    {
        public string type { get; set; }
        public object value { get; set; }
    }

    public class HumRootObject
    {
        public int id { get; set; }
        public int episodes_watched { get; set; }
        public string last_watched { get; set; }
        public string updated_at { get; set; }
        public int rewatched_times { get; set; }
        public object notes { get; set; }
        public object notes_present { get; set; }
        public string status { get; set; }
        public bool @private { get; set; }
        public bool rewatching { get; set; }
        public HumAnime anime { get; set; }
        public HumRating rating { get; set; }
    }
}