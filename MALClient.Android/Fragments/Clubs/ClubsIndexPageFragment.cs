using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubsIndexPageFragment : MalFragmentBase
    {
        protected override void Init(Bundle savedInstanceState)
        {
            throw new NotImplementedException();
        }

        protected override void InitBindings()
        {
            throw new NotImplementedException();
        }

        public override int LayoutResourceId { get; }

        #region Views

        private PagerSlidingTabStrip _tabStrip;
        private ViewPager _pivot;
        private ProgressBar _loadingSpinner;

        public PagerSlidingTabStrip TabStrip => _tabStrip ?? (_tabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.TabStrip));

        public ViewPager Pivot => _pivot ?? (_pivot = FindViewById<ViewPager>(Resource.Id.Pivot));

        public ProgressBar LoadingSpinner => _loadingSpinner ?? (_loadingSpinner = FindViewById<ProgressBar>(Resource.Id.LoadingSpinner));

        #endregion
    }
}