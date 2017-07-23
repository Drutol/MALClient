using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MALClient.Models.Enums;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Utils.Managers
{
    /// <summary>
    ///     Back navigation manager , highly stripped down (compared to mobile version)
    ///     as on desktop we have separeate windows section dedicated to details and such...
    /// </summary>
    public class NavMgr : INavMgr
    {
        private ICommand _currentOverride;
        private ICommand _currentOverrideMain;

        private readonly Stack<Tuple<PageIndex,object>> _randomNavigationStackMain =
            new Stack<Tuple<PageIndex, object>>(30);

        private readonly Stack<Tuple<PageIndex,object>> _randomNavigationStackOff =
            new Stack<Tuple<PageIndex, object>>(30);


        public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            RegisterBackNav(PageIndex.PageAnimeDetails,args);
        }

        public bool HasSomethingOnStack()
        {
            return _randomNavigationStackMain.Any();
        }

        public void RegisterBackNav(ProfilePageNavigationArgs args)
        {
            RegisterBackNav(PageIndex.PageProfile, args);
        }

        public void CurrentOffViewOnBackRequested()
        {
            if (_currentOverride != null)
            {
                _currentOverride.Execute(null);
                _currentOverride = null;
                if (_randomNavigationStackOff.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = false;
                return;
            }

            if (_randomNavigationStackOff.Count == 0) //when we are called from mouse back button
                return;
            var data = _randomNavigationStackOff.Pop();
            ViewModelLocator.GeneralMain.Navigate(data.Item1, data.Item2);
            if (_randomNavigationStackOff.Count == 0)
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = false;
        }

        public void DeregisterBackNav()
        {

        }

        public void ResetOffBackNav()
        {
            _randomNavigationStackOff.Clear();
            _currentOverride = null;
            ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = false;
        }

        public void ResetMainBackNav()
        {
            _randomNavigationStackMain.Clear();
            _currentOverrideMain = null;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
        }

        public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
        {
            if (page == PageIndex.PageAnimeDetails || page == PageIndex.PageCharacterDetails ||
                page == PageIndex.PageStaffDetails ||
                ((page == PageIndex.PageSearch || page == PageIndex.PageMangaSearch) &&
                 (args as SearchPageNavArgsBase)?.DisplayMode == SearchPageDisplayModes.Off))
            {
                _randomNavigationStackOff.Push(new Tuple<PageIndex, object>(page, args));
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = true;
            }
            else if (page == PageIndex.PageProfile || page == PageIndex.PageArticles ||
                     page == PageIndex.PageForumIndex || page == PageIndex.PageFeeds ||
                     page == PageIndex.PageNotificationHub || page == PageIndex.PageFriends)
            {
                _randomNavigationStackMain.Push(new Tuple<PageIndex, object>(page, args));
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = true;
            }
        }

        public void RegisterUnmonitoredMainBackNav(PageIndex page, object args)
        {
            _randomNavigationStackMain.Push(new Tuple<PageIndex, object>(page, args));
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = true;
        }

        public void RegisterOneTimeOverride(ICommand command)
        {
            _currentOverride = command;
            ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = true;
        }

        public void ResetOneTimeOverride()
        {
            _currentOverride = null;
            if (_randomNavigationStackOff.Count == 0)
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = false;
        }

        public void RegisterOneTimeMainOverride(ICommand command)
        {
            _currentOverrideMain = command;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = true;
        }

        public void ResetOneTimeMainOverride()
        {
            _currentOverrideMain = null;
            if(_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
        }

        public void CurrentMainViewOnBackRequested()
        {
            if (_currentOverrideMain != null)
            {
                _currentOverrideMain.Execute(null);
                _currentOverrideMain = null;
                if (_randomNavigationStackMain.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
                return;
            }


            if (_randomNavigationStackMain.Count == 0) //when we are called from mouse back button
                return;
            var data = _randomNavigationStackMain.Pop();
            ViewModelLocator.GeneralMain.Navigate(data.Item1, data.Item2);
            if (_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
        }
    }
}