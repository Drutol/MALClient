namespace MALClient.Android.CollectionAdapters
{
    public interface IFlingAwareAdapter
    {
        bool FlingScrollActive { get; set; }
        int FlingItemCountThreshold { get; set; }
    }
}