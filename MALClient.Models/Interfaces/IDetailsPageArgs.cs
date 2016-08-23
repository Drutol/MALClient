using MALClient.Models.Enums;

namespace MALClient.Models.Interfaces
{
    public interface IDetailsPageArgs
    {
        int Id { get; set; }
        string Title { get; set; }
        RelatedItemType Type { get; set; }
    }
}