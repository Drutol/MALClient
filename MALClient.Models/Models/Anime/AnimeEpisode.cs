using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MALClient.Models.Models.Anime
{
    [Preserve(AllMembers = true)]
    public class AnimeEpisode
    {
        [JsonProperty("episode_id")] public int EpisodeId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("title_japanese")] public string TitleJapanese { get; set; }
        [JsonProperty("title_romanji")] public string TitleRomanji { get; set; }
        [JsonProperty("filler")] public bool Filler { get; set; }
        [JsonProperty("recap")] public bool Recap { get; set; }
        [JsonProperty("video_url")] public string VideoUrl { get; set; }
        [JsonProperty("forum_url")] public string ForumUrl { get; set; }
    }
}
