using Windows.UI.Xaml;
using MALClient.Flyouts;
using MALClient.UserControls;

namespace MALClient
{
    /// <summary>
    ///     This class mangaes with item flyouts, instead of having tons of flyouts we have
    ///     one with changing data context.
    /// </summary>
    public static class ItemFlyoutService
    {
        private static GridItemFlyout _gridFlyout;
        private static ListItemFlyout _listFlyout;
        private static WatchedEpisodesFlyout _watchedFlyout;
        private static ListItemScoreFlyout _scoreFlyout;
        private static ListItemStatusFlyout _statusFlyout;

        private static GridItemFlyout GridFlyout => _gridFlyout ?? (_gridFlyout = new GridItemFlyout());
        private static ListItemFlyout ListFlyout => _listFlyout ?? (_listFlyout = new ListItemFlyout());

        private static WatchedEpisodesFlyout WatchedFlyout
            => _watchedFlyout ?? (_watchedFlyout = new WatchedEpisodesFlyout());

        private static ListItemScoreFlyout ScoreFlyout => _scoreFlyout ?? (_scoreFlyout = new ListItemScoreFlyout());

        private static ListItemStatusFlyout StatusFlyout
            => _statusFlyout ?? (_statusFlyout = new ListItemStatusFlyout());

        public static void ShowAnimeGridItemFlyout(FrameworkElement placement)
        {
            GridFlyout.ShowAt(placement);
        }

        public static void ShowWatchedEpisodesFlyout(FrameworkElement placement)
        {
            WatchedFlyout.ShowAt(placement);
        }

        public static void ShowAnimeListItemFlyout(FrameworkElement placement)
        {
            ListFlyout.ShowAt(placement);
        }

        public static void ShowAnimeListItemScoreFlyout(FrameworkElement placement)
        {
            ScoreFlyout.ShowAt(placement);
        }

        public static void ShowAnimeListItemStatusFlyout(FrameworkElement placement)
        {
            StatusFlyout.ShowAt(placement);
        }
    }
}