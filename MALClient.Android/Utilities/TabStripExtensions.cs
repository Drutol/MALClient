using System.Threading.Tasks;
using Android.Views;

namespace MALClient.Android
{
    public static class TabStripExtensions
    {
        public static void CenterTabs(this UserControls.PagerSlidingTabStrip strip)
        {
            //Yeah... so , well...
            strip.TabsContainer.SetGravity(GravityFlags.CenterHorizontal);
            //strip.ScrollOffset = -350;
        }
    }
}