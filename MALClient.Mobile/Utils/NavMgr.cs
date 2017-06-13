using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Core;
using MALClient.Models.Enums;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Utils
{
    /// <summary>
    ///     Back navigation manager
    /// </summary>
    public class NavMgr : INavMgr
    {
        private ICommand _currentOverride;

        private readonly Stack<Tuple<PageIndex, object>> _randomNavigationStackMain =
            new Stack<Tuple<PageIndex, object>>(30);

        public NavMgr()
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested += CurrentViewOnBackRequested;
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (_currentOverride != null)
            {
                _currentOverride.Execute(null);
                _currentOverride = null;
                if (_randomNavigationStackMain.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
                e.Handled = true;
                return;
            }


            if (_randomNavigationStackMain.Count == 0) //when we are called from mouse back button
                return;
            var data = _randomNavigationStackMain.Pop();
            ViewModelLocator.GeneralMain.Navigate(data.Item1, data.Item2);
            if (_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
            e.Handled = true;
        }

        public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            RegisterBackNav(PageIndex.PageAnimeDetails, args);
        }

        public bool HasSomethingOnStack()
        {
            return _randomNavigationStackMain.Any();
        }

        public void RegisterBackNav(ProfilePageNavigationArgs args)
        {
            RegisterBackNav(PageIndex.PageProfile, args);
        }

        public void DeregisterBackNav()
        {
            _randomNavigationStackMain.Clear();
        }

        public void ResetOffBackNav()
        {
            _randomNavigationStackMain.Clear();
        }

        public void CurrentOffViewOnBackRequested()
        {
            //throw new NotImplementedException();
        }

        public void ResetMainBackNav()
        {
            _randomNavigationStackMain.Clear();
            _currentOverride = null;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
        }

        public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
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
            if (_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateOffBackButtonVisibility = true;
        }

        public void RegisterOneTimeMainOverride(ICommand command)
        {
            _currentOverride = command;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = true;
        }

        public void ResetOneTimeMainOverride()
        {
            _currentOverride = null;
            if (_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = true;
        }

        public void CurrentMainViewOnBackRequested()
        {
            CurrentViewOnBackRequested(null,null);
        }

        public void RegisterUnmonitoredMainBackNav(PageIndex page, object args)
        {
            throw new NotImplementedException();
        }
    }

}