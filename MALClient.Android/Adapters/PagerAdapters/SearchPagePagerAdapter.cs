using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.SearchFragments;
using MALClient.Android.Resources;

using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using PagerSlidingTab;

namespace MALClient.Android.PagerAdapters
{
    public class SearchPagePagerAdapter : FragmentStatePagerAdapter, ICustomTabProvider
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
                    _animeSearchPageFragment = new AnimeSearchPageFragment(true);
                    _mangaSearchPageFragment = new AnimeSearchPageFragment(false);
                    targetPage = 0;
                }
                else
                {
                    _animeSearchPageFragment = new AnimeSearchPageFragment(false);
                    _mangaSearchPageFragment = new AnimeSearchPageFragment(true);
                    targetPage = 1;
                }
                _characterSearchPageFragment = CharacterSearchPageFragment.BuildInstance(new SearchPageNavArgsBase());
            }
            else
            {
                _animeSearchPageFragment = new AnimeSearchPageFragment(false);
                _mangaSearchPageFragment = new AnimeSearchPageFragment(false);

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
                    _animeSearchPageFragment.NavigatedTo();
                    _currentFragment = _animeSearchPageFragment;
                    ShowSearchStuff();
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs { Query = ViewModelLocator.GeneralMain.CurrentSearchQuery });
                    break;
                case 1:
                    _mangaSearchPageFragment.NavigatedTo();
                    _currentFragment = _mangaSearchPageFragment;
                    ShowSearchStuff();
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs {Anime = false , Query = ViewModelLocator.GeneralMain.CurrentSearchQuery});
                    break;
                case 2:
                    _currentFragment = _characterSearchPageFragment;
                    ShowSearchStuff();
                    ViewModelLocator.CharacterSearch.Init(new SearchPageNavArgsBase());
                    _currentFragment?.ReattachBindings();
                    break;
                case 3:
                    _currentFragment = _genresSearchPageFragment;
                    ViewModelLocator.GeneralMain.SearchToggleLock = false;
                    ViewModelLocator.GeneralMain.HideSearchStuff();
                    ViewModelLocator.GeneralMain.CurrentStatus = "Anime by Genre";
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs { ByGenre = true});
                    _currentFragment?.ReattachBindings();
                    break;
                case 4:
                    ViewModelLocator.GeneralMain.HideSearchStuff();
                    ViewModelLocator.GeneralMain.SearchToggleLock = false;
                    ViewModelLocator.GeneralMain.CurrentStatus = "Anime by Studio";
                    _currentFragment = _studiosSearchPageFragment;
                    ViewModelLocator.SearchPage.Init(new SearchPageNavigationArgs { ByStudio = true});
                    _currentFragment?.ReattachBindings();
                    break;
            }

        }

        private void ShowSearchStuff()
        {
            if(ViewModelLocator.GeneralMain.SearchToggleLock)
                return;
            ViewModelLocator.GeneralMain.SearchToggleLock = true;
            ViewModelLocator.GeneralMain.ShowSearchStuff();
            ViewModelLocator.GeneralMain.ToggleSearchStuff();
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