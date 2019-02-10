using System.Collections.Generic;

namespace MALClient.Models.Models.AnimeScrapped
{
    public class ReviewScore
    {
        public string Field { get; set; }
        public string Score { get; set; }
    }

    public class AnimeReviewData
    {
        public string Id { get; set; }
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