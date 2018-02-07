using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MALClient.Models.Models.AnimeScrapped
{
    public class AnimePersonalizedRecommendationData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("path")]
        public string Url { get; set; }
        [JsonProperty("image")]
        public string ImgUrl { get; set; }
        [JsonProperty("bundle")]
        public string Bundle { get; set; }
    }
}
