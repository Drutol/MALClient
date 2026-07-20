using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class AlternativeTitles
    {
        [JsonPropertyName("synonyms")]
        public ICollection<String> Synonyms { get; set; }

        [JsonPropertyName("en")]
        public string English { get; set; }

        [JsonPropertyName("ja")]
        public string Japanese { get; set; }
    }
}
