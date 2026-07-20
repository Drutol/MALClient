using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class Season
    {
        [JsonPropertyName("year")]
        public long? Year { get; set; }

        [JsonPropertyName("season")]
        public string Name { get; set; }
    }
}
