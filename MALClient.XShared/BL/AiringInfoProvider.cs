using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Runtime;
using MALClient.Adapters;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using Newtonsoft.Json;

namespace MALClient.XShared.BL
{
    public class AiringInfoProvider : IAiringInfoProvider
    {
        private const string UpdateStorakeKey = "AiringInfoProviderLastUpdateDate";
        private const string CacheFileName = "airing_data.json";

        private readonly IDataCache _dataCache;
        private readonly IApplicationDataService _applicationDataService;
        private List<AiringData> _airingData;

        public AiringInfoProvider(IDataCache dataCache,IApplicationDataService applicationDataService)
        {
            _dataCache = dataCache;
            _applicationDataService = applicationDataService;
        }

        public async Task Init(bool cacheOnly)
        {
            try
            {
                List<AiringData> data = null;
                var lastUpdate = _applicationDataService[UpdateStorakeKey];
                if (lastUpdate != null)
                {
                    var date = DateTime.FromBinary((long) lastUpdate);
                    if(DateTime.Now - date < TimeSpan.FromHours(8))
                        data = await _dataCache.RetrieveData<List<AiringData>>(CacheFileName, null, 0);
                }

                
                if ((data == null || !data.Any()) && !cacheOnly)
                {
                    var json = await new AnimeAiringDataQuery().GetRequestResponse();
                    if (!string.IsNullOrEmpty(json))
                    {
                        _applicationDataService[UpdateStorakeKey] = DateTime.Now.ToBinary();
                        data = JsonConvert.DeserializeObject<List<AiringData>>(json);
                        _dataCache.SaveData(json, CacheFileName, null);
                    }
                }

                if (data == null)
                {
                    _airingData = new List<AiringData>();
                    return;
                }

                foreach (var airingData in data)
                {
                    airingData.Episodes = airingData.Episodes.OrderBy(episode => episode.Timestamp).ToList();
                }
                _airingData = data;
            }
            catch (Exception)
            {
                _airingData = new List<AiringData>();
            }
        }

        public bool TryGetCurrentEpisode(int id, out int episode, DateTime? forDay = null)
        {
            episode = 0;
            var currentTimestamp = Utilities.ConvertToUnixTimestamp(DateTime.Now);
            var data = _airingData.FirstOrDefault(airingData => airingData.MalId == id);
            if (data == null)
                return false;

            try
            {
                if (forDay == null)
                    episode = data.Episodes.First(ep => ep.Timestamp >= currentTimestamp).EpisodeNumber - 1;
                else
                {
                    var todaysMatch =
                        data.Episodes.FirstOrDefault(ep => Utilities.ConvertFromUnixTimestamp(ep.Timestamp).DayOfYear ==
                                                  forDay.Value.DayOfYear);
                    if (todaysMatch != null)
                        episode = todaysMatch.EpisodeNumber;
                    else
                        episode = data.Episodes.First(ep => ep.Timestamp >= currentTimestamp).EpisodeNumber;

                }
                if (episode <= 0)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        [Preserve(AllMembers = true)]
        class Episode
        {
            [JsonProperty("t")]
            public int Timestamp { get; set; }
            [JsonProperty("n")]
            public int EpisodeNumber { get; set; }
        }

        [Preserve(AllMembers = true)]
        class AiringData
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("mal_id")]
            public int MalId { get; set; }
            [JsonProperty("airing")]
            public List<Episode> Episodes { get; set; }
        }
    }
}
