using Android.Views;
using Com.Astuetz;

namespace MALClient.Android
{
    public static class TabStripExtensions
    {
        public static void CenterTabs(this PagerSlidingTabStrip strip)
        {
            //Yeah... so , well...
            strip.TabsContainer.SetGravity(GravityFlags.CenterHorizontal);
        }
    }
}