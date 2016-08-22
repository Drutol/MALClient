using System;
using System.Collections.Generic;
using MALClient.XShared.ViewModels;

namespace MalClient.Shared.Items
{
    public class AnimeUserCache
    {
        public List<AnimeItemAbstraction> LoadedAnime { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}