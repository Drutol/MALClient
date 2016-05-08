using System.Collections.Generic;
using Windows.UI.Xaml;
using MALClient.Pages;
using MALClient.ViewModels;

namespace MALClient
{
    /// <summary>
    ///     Back navigation manager , highly stripped down (compared to mobile version)
    ///     as on desktop we have separeate windows section dedicated to details and such...
    /// </summary>
    public static class NavMgr
    {
        #region BackNavigation

        private static readonly Stack<AnimeDetailsPageNavigationArgs> _detailsNavStack =
            new Stack<AnimeDetailsPageNavigationArgs>(10);

        public static void RegisterBackNav(object args)
        {
            _detailsNavStack.Push(args as AnimeDetailsPageNavigationArgs);
            ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Visible;
        }

        public static async void CurrentViewOnBackRequested()
        {
            if (_detailsNavStack.Count == 0) //when we are called from mouse back button
                return;

            await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails, _detailsNavStack.Pop());
            if (_detailsNavStack.Count == 0)
                ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Collapsed;
        }

        public static void ResetBackNav()
        {
            _detailsNavStack.Clear();
            ViewModelLocator.Main.NavigateBackButtonVisibility = Visibility.Collapsed;
        }

        #endregion
    }
}