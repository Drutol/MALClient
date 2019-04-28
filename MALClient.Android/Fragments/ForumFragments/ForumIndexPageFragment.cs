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
using MALClient.Android.Listeners;
using MALClient.Android.PagerAdapters;

using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumIndexPageFragment : MalFragmentBase
    {
        private ForumIndexViewModel ViewModel;
        private int _currentTab = -1;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsIndex;
            ViewModel.Init();
        }

        protected override void InitBindings()
        {
            ForumIndexPagePivot.Adapter = new ForumIndexPagerAdapter(ChildFragmentManager);
            ForumIndexPageTabStrip.SetViewPager(ForumIndexPagePivot);
            ForumIndexPageTabStrip.CenterTabs();
            
            if(_currentTab == 1)
                ForumIndexPagePivot.SetCurrentItem(1,false);

            ForumIndexPagePivot.AddOnPageChangeListener(new OnPageChangedListener(i => _currentTab = i));
        }

        public override int LayoutResourceId => Resource.Layout.ForumIndexPage;

        #region Views

        private com.refractored.PagerSlidingTabStrip _forumIndexPageTabStrip;
        private ViewPager _forumIndexPagePivot;

        public com.refractored.PagerSlidingTabStrip ForumIndexPageTabStrip => _forumIndexPageTabStrip ?? (_forumIndexPageTabStrip = FindViewById<com.refractored.PagerSlidingTabStrip>(Resource.Id.ForumIndexPageTabStrip));

        public ViewPager ForumIndexPagePivot => _forumIndexPagePivot ?? (_forumIndexPagePivot = FindViewById<ViewPager>(Resource.Id.ForumIndexPagePivot));

        #endregion
    }
}