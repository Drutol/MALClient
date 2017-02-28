using System;
using System.Collections.Generic;
using System.Windows.Input;
using Android.Content;
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Managers
{
    /// <summary>
    ///     Back navigation manager
    /// </summary>
    public class NavMgr : INavMgr
    {
        private ICommand _currentOverride;

        private readonly Stack<Tuple<PageIndex, object>> _randomNavigationStackMain =
            new Stack<Tuple<PageIndex, object>>(30);

        private void CurrentViewOnBackRequested()
        {
            if (_currentOverride != null)
            {
                _currentOverride.Execute(null);
                _currentOverride = null;
                if (_randomNavigationStackMain.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
                return;
            }


            if (_randomNavigationStackMain.Count == 0) //when we are called from mouse back button
            {
                GoHome();
                return;
            }
            var data = _randomNavigationStackMain.Pop();
            ViewModelLocator.GeneralMain.Navigate(data.Item1, data.Item2);
            if (_randomNavigationStackMain.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = false;
        }


        private void GoHome()
        {  
            Intent intent = new Intent(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryHome);
            MainActivity.CurrentContext.StartActivity(intent);      
        }

        public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            RegisterBackNav(PageIndex.PageAnimeDetails, args);
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
            CurrentViewOnBackRequested();
        }

        public void RegisterUnmonitoredMainBackNav(PageIndex page, object args)
        {
            throw new NotImplementedException();
        }
    }

}