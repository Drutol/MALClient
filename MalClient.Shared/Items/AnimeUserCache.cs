using System;
using System.Collections.Generic;

namespace MALClient.Items
{
    public class AnimeUserCache
    {
        public List<AnimeItemAbstraction> LoadedAnime { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}