using System;
using MALClient.Models.Enums;

namespace MALClient.Models.Models.Library
{
    public class AnimeLibraryItemData : ILibraryData
    {
        public AnimeLibraryItemData()
        {
            
        }

        public AnimeLibraryItemData(ILibraryData other)
        {
            Id = other.Id;
            Title = other.Title;
            AllEpisodes = other.AllEpisodes;
            ImgUrl = other.ImgUrl;
            Type = other.Type;
        }

        private int _id = -1;
        public string MyStartDate { get; set; }
        public string MyEndDate { get; set; }


        public int Id
        {
            get { return _id == -1 ? MalId : _id; }
            set { _id = value; }
        }

        public int MalId { get; set; }
        public string Title { get; set; }
        public AnimeStatus MyStatus { get; set; }
        public float MyScore { get; set; }
        public int MyEpisodes { get; set; }
        public int AllEpisodes { get; set; }
        public string ImgUrl { get; set; }
        public int Type { get; set; }
        public DateTime LastWatched { get; set; } = DateTime.MinValue;
        public string Notes { get; set; }
        public bool IsRewatching { get; set; }
    }
}