using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.SearchFragments;
using MALClient.Android.Resources;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;


namespace MALClient.Android.Adapters.PagerAdapters
{
    public class SearchPagePagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {

        public SearchPagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SearchPagePagerAdapter(FragmentManager fm, SearchPageNavArgsBase args, out int startPage) : base(fm)
        {
            int targetPage;
            var arg = args as SearchPageNavigationArgs;
            if (arg != null)
            {
                ViewModelLocator.SearchPage.Init(arg);
                if (arg.Anime)
                {
                    _animeSearchPageFragment = AnimeSearchPageFragment.BuildInstance();
                    _mangaSearchPageFragment = AnimeSearchPageFragment.BuildInstance(false);
                    targetPage = 0;
                }
                else
                {
                    _animeSearchPageFragment = AnimeSearchPageFragment.BuildInstance(false);
                    _mangaSearchPageFragment = AnimeSearchPageFragment.BuildInstance();
                    targetPage = 1;
                }
                _characterSearchPageFragment = CharacterSearchPageFragment.BuildInstance(new SearchPageNavArgsBase());
            }
            else
            {
                _animeSearchPageFragment = AnimeSearchPageFragment.BuildInstance(false);
                _mangaSearchPageFragment = AnimeSearchPageFragment.BuildInstance(false);

                ViewModelLocator.CharacterSearch.Init(args);
                _characterSearchPageFragment = CharacterSearchPageFragment.BuildInstance(new SearchPageNavArgsBase(),true);
                targetPage = 2;

            }



            _studiosSearchPageFragment = AnimeTypeSearchFragment.Instance;
            _genresSearchPageFragment = AnimeTypeSearchFragment.Instance;

            startPage = targetPage;
        }

        public override int Count => 5;

        private MalFragmentBase _currentFragment;

        private readonly AnimeSearchPageFragment _animeSearchPageFragment;
        private readonly AnimeSearchPageFragment _mangaSearchPageFragment;
        private readonly CharacterSearchPageFragment _characterSearchPageFragment;
        private readonly AnimeTypeSearchFragment _studiosSearchPageFragment;
        private readonly AnimeTypeSearchFragment _genresSearchPageFragment;


        public void TabSelected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = 1f;
            _currentFragment?.DetachBindings();
            switch ((int)p0.Tag)
            {
                case 0:
                    _currentFragment = _animeSearchPageFragment;
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs { Query = ViewModelLocator.GeneralMain.CurrentSearchQuery });
                    break;
                case 1:
                    _currentFragment = _mangaSearchPageFragment;
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs {Anime = false , Query = ViewModelLocator.GeneralMain.CurrentSearchQuery});
                    break;
                case 2:
                    _currentFragment = _characterSearchPageFragment;
                    ViewModelLocator.CharacterSearch.Init(new SearchPageNavArgsBase());
                    break;
                case 3:
                    _currentFragment = _genresSearchPageFragment;
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs { ByGenre = true});
                    break;
                case 4:
                    _currentFragment = _studiosSearchPageFragment;
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs { ByStudio = true});
                    break;
            }
            _currentFragment?.ReattachBindings();
        }

        public void TabUnselected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = .7f;
        }

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _animeSearchPageFragment;
                case 1:
                    return  _mangaSearchPageFragment;
                case 2:
                    return _characterSearchPageFragment;
                case 3:
                    return _genresSearchPageFragment;
                case 4:
                    return _studiosSearchPageFragment;
            }
            throw new ArgumentException();
        }


        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Tag = p1;
            switch (p1)
            {
                case 0:
                    txt.Text = "Anime";
                    break;
                case 1:
                    txt.Text = "Manga";
                    break;
                case 2:
                    txt.Text = "Characters";
                    break;
                case 3:
                    txt.Text = "Genres";
                    break;
                case 4:
                    txt.Text = "Studios";
                    break;
            }

            return txt;
        }
    }
}