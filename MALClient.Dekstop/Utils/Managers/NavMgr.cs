using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MALClient.Pages;
using MALClient.ViewModels;

namespace MALClient.Utils.Managers
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
                    ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = Visibility.Collapsed;
                return;
            }

            if (_randomNavigationStackOff.Count == 0) //when we are called from mouse back button
                return;
            var data = _randomNavigationStackOff.Pop();
            ViewModelLocator.GeneralMain.Navigate(data.Item1, data.Item2);
            if (_randomNavigationStackOff.Count == 0)
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = Visibility.Collapsed;
        }

        public void DeregisterBackNav()
        {

        }

        public void ResetOffBackNav()
        {
            _randomNavigationStackOff.Clear();
            _currentOverride = null;
            ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = Visibility.Collapsed;
        }

        public void ResetMainBackNav()
        {
            _randomNavigationStackMain.Clear();
            _currentOverrideMain = null;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Collapsed;
        }

        public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
        {
            if (page == PageIndex.PageAnimeDetails)
            {
                _randomNavigationStackOff.Push(new Tuple<PageIndex, object>(page, args));
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = Visibility.Visible;
            }
            else if(page == PageIndex.PageProfile || page == PageIndex.PageArticles || page == PageIndex.PageForumIndex)
            {
                _randomNavigationStackMain.Push(new Tuple<PageIndex, object>(page, args));
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Visible;
            }
        }

        public void RegisterOneTimeOverride(ICommand command)
        {
            _currentOverride = command;
            ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = Visibility.Visible;
        }

        public void ResetOneTimeOverride()
        {
            _currentOverride = null;
            if (_randomNavigationStackOff.Count == 0)
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = Visibility.Visible;
        }

        public void RegisterOneTimeMainOverride(ICommand command)
        {
            _currentOverrideMain = command;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Visible;
        }

        public void ResetOneTimeMainOverride()
        {
            _currentOverrideMain = null;
            if(_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Visible;
        }

        public void CurrentMainViewOnBackRequested()
        {
            if (_currentOverrideMain != null)
            {
                _currentOverrideMain.Execute(null);
                _currentOverrideMain = null;
                if (_randomNavigationStackMain.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Collapsed;
                return;
            }


            if (_randomNavigationStackMain.Count == 0) //when we are called from mouse back button
                return;
            var data = _randomNavigationStackMain.Pop();
            ViewModelLocator.GeneralMain.Navigate(data.Item1, data.Item2);
            if (_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Collapsed;
        }
    }
}