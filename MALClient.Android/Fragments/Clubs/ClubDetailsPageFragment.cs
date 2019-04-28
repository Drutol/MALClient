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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.PagerAdapters;

using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubDetailsPageFragment : MalFragmentBase
    {
        private static ClubDetailsPageNavArgs _args;
        private static int? _lastIndex;

        private ClubDetailsViewModel ViewModel = ViewModelLocator.ClubDetails;

        public ClubDetailsPageFragment(ClubDetailsPageNavArgs args)
        {
            if (!args.Equals(_args))
                _lastIndex = null;
            _args = args;        
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.NavigatedTo(_args);
        }

        protected override void InitBindings()
        {
            Pivot.OffscreenPageLimit = 3;
            Pivot.Adapter = new ClubDetailsPagerAdapter(FragmentManager);
            TabStrip.SetViewPager(Pivot);
            TabStrip.CenterTabs();
            if (_lastIndex == null)
                _lastIndex = 1;

            Pivot.SetCurrentItem(_lastIndex.Value, false);

            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => LoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Pivot.AddOnPageChangeListener(new OnPageChangedListener(i => _lastIndex = i));
        }

        public override int LayoutResourceId => Resource.Layout.ClubDetailsPage;


        #region Views

        private com.refractored.PagerSlidingTabStrip _tabStrip;
        private ViewPager _pivot;
        private FrameLayout _loadingSpinner;

        public com.refractored.PagerSlidingTabStrip TabStrip => _tabStrip ?? (_tabStrip = FindViewById<com.refractored.PagerSlidingTabStrip>(Resource.Id.TabStrip));

        public ViewPager Pivot => _pivot ?? (_pivot = FindViewById<ViewPager>(Resource.Id.Pivot));

        public FrameLayout LoadingSpinner => _loadingSpinner ?? (_loadingSpinner = FindViewById<FrameLayout>(Resource.Id.LoadingSpinner));

        #endregion
    }
}