using System;

namespace MALClient.Models.Models.Library
{
    public class AnimeLibraryItemData : ILibraryData
    {
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
    }
}