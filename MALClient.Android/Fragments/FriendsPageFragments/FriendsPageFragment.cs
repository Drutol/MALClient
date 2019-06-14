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

using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.PagerAdapters;

using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class FriendsPageFragment : MalFragmentBase
    {
        private readonly FriendsPageNavArgs _navArgs;

        private FriendsPageViewModel ViewModel = ViewModelLocator.Friends;

        public FriendsPageFragment(FriendsPageNavArgs args)
        {
            _navArgs = args;
        }


        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.NavigatedTo(_navArgs);
        }

        protected override void InitBindings()
        {
            Pivot.Adapter = new FriendsPagePagerAdapter(ChildFragmentManager);
            TabStrip.SetViewPager(Pivot);
            TabStrip.CenterTabs();
        }

        public override int LayoutResourceId => Resource.Layout.FriendsPage;

        #region Views

        private UserControls.PagerSlidingTabStrip _tabStrip;
        private ViewPager _pivot;

        public UserControls.PagerSlidingTabStrip TabStrip => _tabStrip ?? (_tabStrip = FindViewById<UserControls.PagerSlidingTabStrip>(Resource.Id.TabStrip));

        public ViewPager Pivot => _pivot ?? (_pivot = FindViewById<ViewPager>(Resource.Id.Pivot));

        #endregion
    }
}