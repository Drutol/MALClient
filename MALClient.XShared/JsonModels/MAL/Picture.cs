using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class MainPicture
    {
        [JsonPropertyName("medium")]
        public String Medium { get; set; }

        [JsonPropertyName("large")]
        public String Large { get; set; }
    }
}
