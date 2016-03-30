using System.Collections.Generic;
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

        private static readonly Stack<AnimeDetailsPageNavigationArgs> _detailsNavStack =
            new Stack<AnimeDetailsPageNavigationArgs>(4);

        private static bool _wasOnStack;
        private static bool _handlerRegistered;

        public static void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
            //about because it is not used anywhere...
        {
            //if we are navigating inside details we have to create stack
            if (source == PageIndex.PageAnimeDetails)
            {
                _wasOnStack = true;
                _detailsNavStack.Push(args as AnimeDetailsPageNavigationArgs);
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
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                currentView.BackRequested += CurrentViewOnBackRequested;
                _handlerRegistered = true;
            }
        }

        private static async void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs args)
        {
            args.Handled = true;
            if (_detailsNavStack.Count == 0) //nothing on the stack = standard
                await ViewModelLocator.Main.Navigate(_pageTo, _args);
            else //take an element from stack otherwise
                await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails, _detailsNavStack.Pop());

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

        #endregion
    }
}