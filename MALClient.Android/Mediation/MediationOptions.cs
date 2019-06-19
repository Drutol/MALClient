using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace MALClient.Android.Mediation
{
    public class MediationOptions
    {
        [JsonProperty("I")]
        public string ImageUrl { get; set; }
        [JsonProperty("IA")]
        public string[] ImageUrls { get; set; }
        [JsonProperty("T")]
        public int AdDisplayTime { get; set; }
        [JsonProperty("L")]
        public string Link { get; set; }
        [JsonProperty("LA")]
        public string Label { get; set; }
    }
}