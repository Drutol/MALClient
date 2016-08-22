using MalClient.Shared.Comm.Anime;

namespace MalClient.Shared.Utils
{
    public interface IDetailsPageArgs
    {
        int Id { get; set; }
        string Title { get; set; }
        RelatedItemType Type { get; set; }
    }
}