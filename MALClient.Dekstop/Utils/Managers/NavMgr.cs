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

        private readonly Stack<AnimeDetailsPageNavigationArgs> _detailsNavStack =
            new Stack<AnimeDetailsPageNavigationArgs>(10);

        private readonly Stack<ProfilePageNavigationArgs> _profileNavigationStack =
            new Stack<ProfilePageNavigationArgs>(10);

        public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            _detailsNavStack.Push(args);
            ViewModelLocator.GeneralMain.NavigateBackButtonVisibility = Visibility.Visible;
        }

        public void RegisterBackNav(ProfilePageNavigationArgs args)
        {
            _profileNavigationStack.Push(args);
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Visible;
        }

        public void CurrentViewOnBackRequested()
        {
            if (_currentOverride != null)
            {
                _currentOverride.Execute(null);
                _currentOverride = null;
                if (_detailsNavStack.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateBackButtonVisibility = Visibility.Collapsed;
                return;
            }

            if (_detailsNavStack.Count == 0) //when we are called from mouse back button
                return;

            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails, _detailsNavStack.Pop());
            if (_detailsNavStack.Count == 0)
                ViewModelLocator.GeneralMain.NavigateBackButtonVisibility = Visibility.Collapsed;
        }

        public void DeregisterBackNav()
        {
            throw new System.NotImplementedException();
        }

        public void ResetBackNav()
        {
            _detailsNavStack.Clear();
            _currentOverride = null;
            ViewModelLocator.GeneralMain.NavigateBackButtonVisibility = Visibility.Collapsed;
        }

        public void ResetMainBackNav()
        {
            _profileNavigationStack.Clear();
            _currentOverrideMain = null;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Collapsed;
        }

        public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
        {
           
        }

        public void RegisterOneTimeOverride(ICommand command)
        {
            _currentOverride = command;
            ViewModelLocator.GeneralMain.NavigateBackButtonVisibility = Visibility.Visible;
        }

        public void RegisterOneTimeMainOverride(ICommand command)
        {
            _currentOverrideMain = command;
            ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Visible;
        }

        public void CurrentMainViewOnBackRequested()
        {
            if (_currentOverrideMain != null)
            {
                _currentOverrideMain.Execute(null);
                _currentOverrideMain = null;
                if (_profileNavigationStack.Count == 0)
                    ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Collapsed;
                return;
            }


            if (_profileNavigationStack.Count == 0) //when we are called from mouse back button
                return;

            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, _profileNavigationStack.Pop());
            if (_profileNavigationStack.Count == 0)
                ViewModelLocator.GeneralMain.NavigateMainBackButtonVisibility = Visibility.Collapsed;
        }
    }
}