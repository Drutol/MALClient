namespace MALClient.XShared.NavArgs
{
    public class ProfilePageNavigationArgs
    {
        public string TargetUser { get; set; }
        public int DesiredPivotIndex { get; set; }
        public bool AllowBackNavReset { get; set; } = true;
    }
}
