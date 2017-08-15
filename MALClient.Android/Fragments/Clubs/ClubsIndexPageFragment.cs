using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using Com.Mikepenz.Materialdrawer;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.PagerAdapters;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
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
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModel.NavigatedTo();
        }

        protected override void InitBindings()
        {
            InitDrawer();
            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => LoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Pivot.Adapter = new ClubsIndexPagerAdapter(FragmentManager,_rightDrawer);
            TabStrip.SetViewPager(Pivot);
            TabStrip.CenterTabs();
            Pivot.SetCurrentItem(_lastPivotIndex,false);

            Pivot.AddOnPageChangeListener(new OnPageChangedListener(i => _lastPivotIndex = i));
        }

        private Drawer _rightDrawer;

        private void InitDrawer()
        {
            if (_rightDrawer != null)
                return;

            var builder = new DrawerBuilder().WithActivity(Activity);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);
            builder.WithDrawerGravity((int)GravityFlags.Right);

            builder.WithStickyHeaderShadow(true);
            builder.WithStickyHeader(Resource.Layout.AnimeListPageDrawerHeader);

            _rightDrawer = builder.Build();
            _rightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            _rightDrawer.StickyHeader.SetBackgroundColor(new Color(ResourceExtension.BrushAppBars));
            _rightDrawer.DrawerLayout.AddDrawerListener(new DrawerListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride(), null));
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