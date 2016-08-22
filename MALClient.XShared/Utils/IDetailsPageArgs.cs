using MALClient.XShared.Comm.Anime;

namespace MALClient.XShared.Utils
{
    public interface IDetailsPageArgs
    {
        int Id { get; set; }
        string Title { get; set; }
        RelatedItemType Type { get; set; }
    }
}