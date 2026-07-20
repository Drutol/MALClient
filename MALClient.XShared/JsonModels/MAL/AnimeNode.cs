using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class AnimeNode<TResponse>
    {
        [JsonPropertyName("node")]
        public TResponse Node { get; set; }
    }
}
