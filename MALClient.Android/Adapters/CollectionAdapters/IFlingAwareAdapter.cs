namespace MALClient.Android.CollectionAdapters
{
    public interface IFlingAwareAdapter
    {
        bool FlingScrollActive { get; set; }
        bool FlingScrollOverride { get; set; }
        int FlingItemCountThreshold { get; set; }
    }
}