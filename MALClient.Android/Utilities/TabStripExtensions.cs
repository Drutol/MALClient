using Android.Views;

namespace MALClient.Android
{
    public static class TabStripExtensions
    {
        public static void CenterTabs(this com.refractored.PagerSlidingTabStrip strip)
        {
            //Yeah... so , well...
            strip.TabsContainer.SetGravity(GravityFlags.CenterHorizontal);
        }
    }
}