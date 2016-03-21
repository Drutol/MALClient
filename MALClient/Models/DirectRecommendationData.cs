using MALClient.Comm;
using MALClient.Pages;

namespace MALClient.Models
{
    /// <summary>
    ///     Direct as in recommendation from details page
    /// </summary>
    public class DirectRecommendationData : IDetailsPageArgs
    {
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public RelatedItemType Type { get; set; }
    }
}