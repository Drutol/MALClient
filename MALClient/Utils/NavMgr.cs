using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Core;
using MALClient.Pages;
using MALClient.ViewModels;

namespace MALClient
{
    /// <summary>
    ///     Back navigation manager
    /// </summary>
    public static class NavMgr
    {
        #region BackNavigation

        private static PageIndex _pageTo;
        private static object _args;

        private static readonly Stack<object> _detailsNavStack =
            new Stack<object>(10);

        private static bool _wasOnStack;
        private static bool _handlerRegistered;
        private static bool _oneTimeHandler;

        private static ICommand _currentOverride;

        public static void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
            //about because it is not used anywhere...
        {
            //if we are navigating inside details we have to create stack
            if (source == PageIndex.PageAnimeDetails || source == PageIndex.PageProfile)
            {
                _wasOnStack = true;
                _detailsNavStack.Push(args);
                //we can only navigate to details from details so...
            }
            else //non details navigation
            {
                _detailsNavStack.Clear();
                _wasOnStack = false;
                _pageTo = page;
                _args = args;
            }

            if (!_handlerRegistered)
            {
                var currentView = SystemNavigationManager.GetForCurrentView();
                currentView.BackRequested += CurrentViewOnBackRequested;
                _handlerRegistered = true;
            }
        }

        private static void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs args)
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
            if (_detailsNavStack.Count == 0) //nothing on the stack = standard
                ViewModelLocator.Main.Navigate(_pageTo, _args);
            else //take an element from stack otherwise
            {
                object arg = _detailsNavStack.Pop();
                ViewModelLocator.Main.Navigate( arg is AnimeDetailsPageNavigationArgs ? PageIndex.PageAnimeDetails : PageIndex.PageProfile ,arg );
            }

            if (_args is AnimeListPageNavigationArgs)
            {
                var param = (AnimeListPageNavigationArgs) _args;
                if (param.WorkMode == AnimeListWorkModes.TopManga)
                    _pageTo = PageIndex.PageTopManga;
                else if (param.WorkMode == AnimeListWorkModes.TopAnime)
                    _pageTo = PageIndex.PageTopAnime;
            }

            ViewModelLocator.Hamburger.SetActiveButton(Utils.GetButtonForPage(_pageTo));
        }

        public static void DeregisterBackNav()
        {
            if (_detailsNavStack.Count > 0)
            {
                return; //we still have stack to go
            }
            if (_wasOnStack)
            {
                _wasOnStack = false;
                return;
            }
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested -= CurrentViewOnBackRequested;
            _handlerRegistered = false;
        }

        public static void ResetBackNav()
        {
            _detailsNavStack.Clear();
            _wasOnStack = false;
            _handlerRegistered = false;
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested -= CurrentViewOnBackRequested;
        }

        public static void RegisterOneTimeOverride(ICommand command)
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

        #endregion
    }
}