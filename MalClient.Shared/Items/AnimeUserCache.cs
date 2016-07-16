using System;
using System.Collections.Generic;
using MalClient.Shared.ViewModels;

namespace MalClient.Shared.Items
{
    public class AnimeUserCache
    {
        public List<AnimeItemAbstraction> LoadedAnime { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}