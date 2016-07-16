using System.Collections.Generic;

namespace MalClient.Shared.Models.AnimeScrapped
{
    public class ReviewScore
    {
        public string Field { get; set; }
        public string Score { get; set; }
    }

    public class AnimeReviewData
    {
        public string Review { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string AuthorAvatar { get; set; }
        public string OverallRating { get; set; }
        public string EpisodesSeen { get; set; }
        public string HelpfulCount { get; set; }
        public List<ReviewScore> Score { get; set; } = new List<ReviewScore>();
    }
}