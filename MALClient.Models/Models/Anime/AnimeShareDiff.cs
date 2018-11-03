using System;
using System.Collections.Generic;
using System.Text;
using MALClient.Models.Enums;

namespace MALClient.Models.Models.Anime
{
    public class AnimeShareDiff
    {
        public string Title { get; set; }
        public string Url => $"https://myanimelist.net/{(IsAnime ? "anime" : "manga")}/{Id}";
        public AnimeStatus NewStatus { get; set; }
        public bool IsAnime { get; set; }
        public int NewScore { get; set; }
        public int NewEpisodes { get; set; }
        public int TotalEpisodes { get; set; }
        public int Id { get; set; }
    }
}
