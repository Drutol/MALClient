using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MALClient.Models.Models.ApiResponses
{
    [Preserve(AllMembers = true)]
    public class SearchEverywhereResponse
    {
        [JsonProperty("categories")] public List<Category> Categories { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Payload
    {
        [JsonProperty("media_type")] public string MediaType { get; set; }
        [JsonProperty("start_year")] public int StartYear { get; set; }
        [JsonProperty("aired")] public string Aired { get; set; }
        [JsonProperty("score")] public string Score { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("published")] public string Published { get; set; }
        [JsonProperty("related_works")] public List<string> RelatedWorks { get; set; }
        [JsonProperty("favorites")] public int Favorites { get; set; }
        [JsonProperty("alternative_name")] public string AlternativeName { get; set; }
        [JsonProperty("birthday")] public string Birthday { get; set; }

    }
    [Preserve(AllMembers = true)]
    public class Item
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("image_url")] public string ImageUrl { get; set; }
        [JsonProperty("thumbnail_url")] public string ThumbnailUrl { get; set; }
        [JsonProperty("payload")] public Payload Payload { get; set; }
        [JsonProperty("es_score")] public double EsScore { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Category
    {
        public string Type { get; set; }
        public List<Item> Items { get; set; }
    }
}
