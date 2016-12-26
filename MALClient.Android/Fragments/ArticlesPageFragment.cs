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
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class ArticlesPageFragment : MalFragmentBase
    {
        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.MalArticles.Init(MalArticlesPageNavigationArgs.Articles);
        }

        protected override void InitBindings()
        {
            ArticlesPagePivot.Adapter = new ArticlesPagePagerAdapter(FragmentManager);
            ArticlesPageTabStrip.SetViewPager(ArticlesPagePivot);
        }

        public override int LayoutResourceId => Resource.Layout.ArticlesPage;

        #region Views

        private PagerSlidingTabStrip _articlesPageTabStrip;
        private ViewPager _articlesPagePivot;

        public PagerSlidingTabStrip ArticlesPageTabStrip => _articlesPageTabStrip ?? (_articlesPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.ArticlesPageTabStrip));

        public ViewPager ArticlesPagePivot => _articlesPagePivot ?? (_articlesPagePivot = FindViewById<ViewPager>(Resource.Id.ArticlesPagePivot));



        #endregion
    }
}