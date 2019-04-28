using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using com.refractored;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.ArticlesPageFragments;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.PagerAdapters
{
    class ArticlesPagePagerAdapter : FragmentStatePagerAdapter, ICustomTabProvider
    {
        public ArticlesPagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ArticlesPagePagerAdapter(FragmentManager fm) : base(fm)
        {
            _articlesFragment = new ArticlesPageTabFragment(true,ArticlePageWorkMode.Articles);
            _newsFragment = new ArticlesPageTabFragment(false,ArticlePageWorkMode.News);
        }

        private MalFragmentBase _currentFragment;

        private readonly MalFragmentBase _articlesFragment;
        private readonly MalFragmentBase _newsFragment;

        public override int Count => 2;

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _articlesFragment;
                case 1:
                    return _newsFragment;
            }
            throw new ArgumentException();
        }

        public void TabSelected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = 1f;
            switch ((int)p0.Tag)
            {
                case 0:
                    _currentFragment = _articlesFragment;
                    ViewModelLocator.MalArticles.Init(MalArticlesPageNavigationArgs.Articles);
                    break;
                case 1:
                    _currentFragment = _newsFragment;
                    ViewModelLocator.MalArticles.Init(MalArticlesPageNavigationArgs.News);
                    break;
            }
        }

        public void TabUnselected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = .7f;
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Tag = p1;
            switch (p1)
            {
                case 0:
                    txt.Text = "Articles";
                    break;
                case 1:
                    txt.Text = "News";
                    break;
            }

            return txt;
        }
    }
}