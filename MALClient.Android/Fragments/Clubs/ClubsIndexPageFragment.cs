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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.PagerAdapters;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubsIndexPageFragment : MalFragmentBase
    {
        private static int _lastPivotIndex;
        private ClubIndexViewModel ViewModel = ViewModelLocator.ClubIndex;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.NavigatedTo();
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => LoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Pivot.Adapter = new ClubsIndexPagerAdapter(FragmentManager);
            TabStrip.SetViewPager(Pivot);
            Pivot.SetCurrentItem(_lastPivotIndex,false);

            Pivot.AddOnPageChangeListener(new OnPageChangedListener(i => _lastPivotIndex = i));
        }

        public override int LayoutResourceId => Resource.Layout.ClubsIndexPage;

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