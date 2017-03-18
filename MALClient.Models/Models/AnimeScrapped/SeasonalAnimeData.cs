using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace MALClient.Models.Models.AnimeScrapped
{
    public class SeasonalAnimeData : ISeasonalAnimeBaseData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Ignore]
        public List<string> Genres { get; set; }
        public float Score { get; set; }
        public string ImgUrl { get; set; }
        public string Episodes { get; set; }
        public int Index { get; set; }
        public int AirDay { get; set; }
        public string AirStartDate { get; set; }

        public string TextBlobGenres
        {
            get
            {
                if (Genres?.Any() ?? false) return string.Join("||", Genres);
                return null;
            }
            set
            {
                Genres = value?.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries)?.ToList() ??
                         new List<string>();
            }
        }

    }
}