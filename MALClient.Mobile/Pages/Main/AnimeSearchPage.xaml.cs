using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeSearchPage : Page
    {
        public AnimeSearchPage()
        {
            InitializeComponent();
        }

        private SearchPageViewModel ViewModel => DataContext as SearchPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList,null);
            var param = e.Parameter as SearchPageNavigationArgs;
            if (param != null)
                Loaded += (sender, args) =>
                {
                    if (param.ByGenre)
                        AnimeByGenreToggleButton.IsChecked = true;
                    else if (param.ByStudio)
                        AnimeByStudioToggleButton.IsChecked = true;
                };
            ViewModel.Init(param);
            base.OnNavigatedTo(e);
        }

        private void SelectionGridViewOnClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AnimeGenres)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs((AnimeGenres)e.ClickedItem));
            else
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs((AnimeStudios)e.ClickedItem));
        }

        private void ButtonByStudioBase_OnClick(object sender, RoutedEventArgs e)
        {
            if(AnimeByStudioToggleButton.IsChecked == true)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch, new SearchPageNavigationArgs { ByStudio = true });
            else
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch, new SearchPageNavigationArgs());
        }

        private void ButtonByGenreBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (AnimeByGenreToggleButton.IsChecked == true)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch, new SearchPageNavigationArgs { ByGenre = true });
            else
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch, new SearchPageNavigationArgs());

        }
    }
}