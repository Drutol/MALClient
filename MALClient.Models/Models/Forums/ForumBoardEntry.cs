using System.Collections.Generic;

namespace MALClient.Models.Models.Forums
{
    //staticly defined boards 
    public class ForumBoardEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<ForumBoardEntryPeekPost> PeekPosts { get; set; } = new List<ForumBoardEntryPeekPost>(2);
    }

    //these things on the left aka most recent posts
    public class ForumBoardEntryPeekPost
    {
        public MalUser User { get; set; } = new MalUser();
        public string Title { get; set; }
        public string PostTime { get; set; }
        public string Id { get; set; }
    }

    public class ForumIndexContent
    {
        public List<List<ForumBoardEntryPeekPost>> ForumBoardEntryPeekPosts { get; set; } =
            new List<List<ForumBoardEntryPeekPost>>();

        public List<ForumPostEntry> PopularNewTopics { get; set; }
        public List<ForumPostEntry> RecentPosts { get; set; } 
        public List<ForumPostEntry> AnimeSeriesDisc { get; set; } 
        public List<ForumPostEntry> MangaSeriesDisc { get; set; } 
    }
}
