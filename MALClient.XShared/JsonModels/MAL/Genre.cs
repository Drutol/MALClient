using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class Genre
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
