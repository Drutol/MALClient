using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Renderscripts;
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
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageFragment : MalFragmentBase
    {
        private readonly ProfilePageNavigationArgs _args;
        private ProfilePageViewModel ViewModel = ViewModelLocator.ProfilePage;

        public ProfilePageFragment(ProfilePageNavigationArgs args)
        {
            _args = args;
        }

        protected override  void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.AnimeList.AnimeItemsDisplayContext = AnimeItemDisplayContext.AirDay;
#pragma warning disable 4014
            ViewModel.LoadProfileData(_args);
#pragma warning restore 4014
        }

        protected override void InitBindings()
        {
            ProfilePagePivot.Adapter = new ProfilePagePagerAdapter(ChildFragmentManager);
            ProfilePageTabStrip.SetViewPager(ProfilePagePivot);
            ProfilePagePivot.OffscreenPageLimit = 4;
            ProfilePageTabStrip.CenterTabs();

            Bindings.Add(this.SetBinding(() => ViewModel.CurrentPivotIndex).WhenSourceChanges(() =>
            {
                ProfilePagePivot.SetCurrentItem(ViewModel.CurrentPivotIndex, false);
            }));


            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingVisibility,
                    () => ProfilePageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            ProfilePageTabStrip.OnPageChangeListener = new OnPageChangedListener(i => ViewModel.CurrentPivotIndex = i);
        }

        public override int LayoutResourceId => Resource.Layout.ProfilePage;


        #region Views

        private FrameLayout _profilePageLoadingSpinner;
        private UserControls.PagerSlidingTabStrip _profilePageTabStrip;
        private ViewPager _profilePagePivot;

        public FrameLayout ProfilePageLoadingSpinner => _profilePageLoadingSpinner ?? (_profilePageLoadingSpinner = FindViewById<FrameLayout>(Resource.Id.ProfilePageLoadingSpinner));

        public UserControls.PagerSlidingTabStrip ProfilePageTabStrip => _profilePageTabStrip ?? (_profilePageTabStrip = FindViewById<UserControls.PagerSlidingTabStrip>(Resource.Id.ProfilePageTabStrip));

        public ViewPager ProfilePagePivot => _profilePagePivot ?? (_profilePagePivot = FindViewById<ViewPager>(Resource.Id.ProfilePagePivot));

        #endregion
    }
}