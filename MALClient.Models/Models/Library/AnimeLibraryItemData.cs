using System;
using MALClient.Models.Enums;
using SQLite;

namespace MALClient.Models.Models.Library
{
    public class AnimeLibraryItemData : ILibraryData
    {
        private int _id = -1;
        private string _owner;

        [PrimaryKey]
        public int CompositeKey
        {
            get
            {
                unchecked
                {
                    return (Id * 397) ^ (_owner?.GetHashCode() ?? 0);
                }
            }
            set { }
        }

        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public int Id
        {
            get { return _id == -1 ? MalId : _id; }
            set { _id = value; }
        }


        public string MyStartDate { get; set; }
        public string MyEndDate { get; set; }




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