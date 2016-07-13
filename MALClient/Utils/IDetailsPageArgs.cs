using MALClient.Comm;

namespace MALClient.Utils
{
    public interface IDetailsPageArgs
    {
        int Id { get; set; }
        string Title { get; set; }
        RelatedItemType Type { get; set; }
    }
}