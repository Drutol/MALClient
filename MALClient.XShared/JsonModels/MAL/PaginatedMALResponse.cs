using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    internal class PaginatedMALResponse<TResponse>
    {
        [JsonPropertyName("data")]
        public TResponse Data { get; set; }
    }
}
