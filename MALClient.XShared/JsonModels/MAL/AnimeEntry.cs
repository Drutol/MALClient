using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class AnimeEntry
    {
        [JsonPropertyName("id")]
        public long? MalId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("main_picture")]
        public MainPicture Picture { get; set; }

        [JsonPropertyName("num_episodes")]
        public long? Episodes { get; set; }

        [JsonPropertyName("mean")]
        public double? Score { get; set; }

        [JsonPropertyName("genres")]
        public ICollection<Genre> Genres { get; set; }

        [JsonPropertyName("num_list_users")]
        public long? MembersCount { get; set; }

        [JsonPropertyName("start_season")]
        public Season StartSeason { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("media_type")]
        public string Type { get; set; }

        [JsonPropertyName("alternative_titles")]
        public AlternativeTitles AlternativeTitle { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("synopsis")]
        public string Synopsis { get; set; }
    }
}
