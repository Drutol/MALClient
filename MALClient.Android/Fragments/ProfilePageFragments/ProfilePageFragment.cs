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
using Com.Astuetz;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
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
#pragma warning disable 4014
            ViewModel.LoadProfileData(_args);
#pragma warning restore 4014
        }

        protected override void InitBindings()
        {
            ProfilePagePivot.Adapter = new ProfilePagePagerAdapter(FragmentManager);
            ProfilePageTabStrip.SetViewPager(ProfilePagePivot);

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingVisibility,
                    () => ProfilePageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        public override int LayoutResourceId => Resource.Layout.ProfilePage;


        #region Views

        private ProgressBar _profilePageLoadingSpinner;
        private PagerSlidingTabStrip _profilePageTabStrip;
        private ViewPager _profilePagePivot;

        public ProgressBar ProfilePageLoadingSpinner => _profilePageLoadingSpinner ?? (_profilePageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ProfilePageLoadingSpinner));

        public PagerSlidingTabStrip ProfilePageTabStrip => _profilePageTabStrip ?? (_profilePageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.ProfilePageTabStrip));

        public ViewPager ProfilePagePivot => _profilePagePivot ?? (_profilePagePivot = FindViewById<ViewPager>(Resource.Id.ProfilePagePivot));

        #endregion
    }
}