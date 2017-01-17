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
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class SearchPageFragment : MalFragmentBase
    {
        private readonly SearchPageNavigationArgs _args;

        private SearchPageFragment(SearchPageNavigationArgs args)
        {
            _args = args;
        }

        protected override void InitBindings()
        {
            int start;
            SearchPageViewPager.Adapter = new SearchPagePagerAdapter(FragmentManager,_args,out start);
            SearchPageTabStrip.SetViewPager(SearchPageViewPager);

            SearchPageViewPager.SetCurrentItem(start,false);
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }


        #region Views

        private PagerSlidingTabStrip _searchPageTabStrip;
        private ViewPager _searchPageViewPager;

        public PagerSlidingTabStrip SearchPageTabStrip => _searchPageTabStrip ?? (_searchPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.SearchPageTabStrip));

        public ViewPager SearchPageViewPager => _searchPageViewPager ?? (_searchPageViewPager = FindViewById<ViewPager>(Resource.Id.SearchPageViewPager));

        #endregion

        public static SearchPageFragment BuildInstance(SearchPageNavigationArgs args)
        {
            return new SearchPageFragment(args); //just so we follow the pattern
        }

        public override int LayoutResourceId => Resource.Layout.SearchPage;
        
    }
}