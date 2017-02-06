using Android.Support.V4.View;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Resources;

namespace MALClient.Android.Fragments.CalendarFragments
{
    public partial class CalendarPageFragment
    {
        private PagerSlidingTabStrip _calendarPageTabStrip;
        private ViewPager _calendarPageViewPager;
        private LinearLayout _calendarPageContentGrid;
        private ProgressBar _calendarPageProgressBar;
        private LinearLayout _calendarPageProgressBarGrid;

        public PagerSlidingTabStrip CalendarPageTabStrip => _calendarPageTabStrip ?? (_calendarPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.CalendarPageTabStrip));

        public ViewPager CalendarPageViewPager => _calendarPageViewPager ?? (_calendarPageViewPager = FindViewById<ViewPager>(Resource.Id.CalendarPageViewPager));

        public LinearLayout CalendarPageContentGrid => _calendarPageContentGrid ?? (_calendarPageContentGrid = FindViewById<LinearLayout>(Resource.Id.CalendarPageContentGrid));

        public ProgressBar CalendarPageProgressBar => _calendarPageProgressBar ?? (_calendarPageProgressBar = FindViewById<ProgressBar>(Resource.Id.CalendarPageProgressBar));

        public LinearLayout CalendarPageProgressBarGrid => _calendarPageProgressBarGrid ?? (_calendarPageProgressBarGrid = FindViewById<LinearLayout>(Resource.Id.CalendarPageProgressBarGrid));



    }
}