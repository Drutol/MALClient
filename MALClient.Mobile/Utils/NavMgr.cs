using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Core;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MALClient.ViewModels;

namespace MALClient.Utils
{
    /// <summary>
    ///     Back navigation manager
    /// </summary>
    public class NavMgr : INavMgr
    {
        private PageIndex _pageTo;
        private object _args;

        private readonly Stack<object> DetailsNavStack =
            new Stack<object>(10);

        private bool _wasOnStack;
        private bool _handlerRegistered;
        private bool _oneTimeHandler;

        private ICommand _currentOverride;

        public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
            //about because it is not used anywhere...
        {
            //if we are navigating inside details we have to create stack
            if (source == PageIndex.PageAnimeDetails || source == PageIndex.PageProfile)
            {
                _wasOnStack = true;
                DetailsNavStack.Push(args);
                //we can only navigate to details from details so...
            }
            else //non details navigation
            {
                DetailsNavStack.Clear();
                _wasOnStack = false;
                _pageTo = page;
                _args = args;
            }

            if (!_handlerRegistered)
            {
                var currentView = SystemNavigationManager.GetForCurrentView();
                currentView.BackRequested += CurrentViewOnBackRequested;
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                _handlerRegistered = true;
            }
        }

        private void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs args)
        {
            args.Handled = true;
            if (_currentOverride != null)
            {
                _currentOverride.Execute(null);
                _currentOverride = null;
                if (_oneTimeHandler)
                {
                    _oneTimeHandler = false;
                    var currentView = SystemNavigationManager.GetForCurrentView();
                    currentView.BackRequested -= CurrentViewOnBackRequested;
                }
                return;
            }
            if (DetailsNavStack.Count == 0) //nothing on the stack = standard
                MobileViewModelLocator.Main.Navigate(_pageTo, _args);
            else //take an element from stack otherwise
            {
                object arg = DetailsNavStack.Pop();
                MobileViewModelLocator.Main.Navigate( arg is AnimeDetailsPageNavigationArgs ? PageIndex.PageAnimeDetails : PageIndex.PageProfile ,arg );
            }

            if (_args is AnimeListPageNavigationArgs)
            {
                var param = (AnimeListPageNavigationArgs) _args;
                if (param.WorkMode == AnimeListWorkModes.TopManga)
                    _pageTo = PageIndex.PageTopManga;
                else if (param.WorkMode == AnimeListWorkModes.TopAnime)
                    _pageTo = PageIndex.PageTopAnime;
            }

            MobileViewModelLocator.Hamburger.SetActiveButton(Utilities.GetButtonForPage(_pageTo));
        }

        public void DeregisterBackNav()
        {
            if (DetailsNavStack.Count > 0)
            {
                return; //we still have stack to go
            }
            if (_wasOnStack)
            {
                _wasOnStack = false;
                return;
            }
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested -= CurrentViewOnBackRequested;
            _handlerRegistered = false;
        }

        public void ResetOffBackNav()
        {
            DetailsNavStack.Clear();
            _wasOnStack = false;
            _handlerRegistered = false;
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested -= CurrentViewOnBackRequested;
        }

        public void RegisterBackNav(ProfilePageNavigationArgs args)
        {
           // throw new System.NotImplementedException();
        }

        public void CurrentMainViewOnBackRequested()
        {
            //throw new System.NotImplementedException();
        }

        public void CurrentOffViewOnBackRequested()
        {
            //throw new System.NotImplementedException();
        }

        public void ResetMainBackNav()
        {
           // throw new System.NotImplementedException();
        }

        public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            //throw new System.NotImplementedException();
        }

        public void RegisterOneTimeMainOverride(ICommand command)
        {
            //throw new System.NotImplementedException();
        }

        public void ResetOneTimeOverride()
        {
            throw new System.NotImplementedException();
        }

        public void ResetOneTimeMainOverride()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterOneTimeOverride(ICommand command)
        {
            if (!_handlerRegistered)
            {
                _oneTimeHandler = true;
                var currentView = SystemNavigationManager.GetForCurrentView();
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                currentView.BackRequested += CurrentViewOnBackRequested;
            }

            _currentOverride = command;
        }

    }
}