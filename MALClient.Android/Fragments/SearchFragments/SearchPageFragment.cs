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
using MALClient.XShared.NavArgs;

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
            SearchPageViewPager.Adapter = new SearchPagePagerAdapter(FragmentManager,_args);
            SearchPageTabStrip.SetViewPager(SearchPageViewPager);
        }

        protected override void Init(Bundle savedInstanceState)
        {
            
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