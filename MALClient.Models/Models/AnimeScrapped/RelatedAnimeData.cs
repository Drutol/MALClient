using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Models.AnimeScrapped
{
    public class RelatedAnimeData : IDetailsPageArgs
    {
        public string WholeRelation { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public RelatedItemType Type { get; set; }
    }
}