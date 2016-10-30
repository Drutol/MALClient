using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.Shared.Items;
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
        private bool _initialized;
        private SearchPageViewModel ViewModel => DataContext as SearchPageViewModel;

        public AnimeSearchPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ViewModel.PrevArgs.ByGenre)
                UpperNavBarPivot.SelectedIndex = 3;
            else if (ViewModel.PrevArgs.ByStudio)
                UpperNavBarPivot.SelectedIndex = 4;
            else
                UpperNavBarPivot.SelectedIndex = ViewModel.PrevArgs.Anime ? 0 : 1;
            _initialized = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModel.Init(e.Parameter as SearchPageNavigationArgs);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModelLocator.SearchPage.OnNavigatedFrom();
            base.OnNavigatedFrom(e);
        }

        private void Animes_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.SearchPage.CurrentlySelectedItem = e.ClickedItem as AnimeSearchItemViewModel;
        }

        private void DirectInputOnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.SubmitQuery(DirectInput.Text);
        }

        private void UpperNavBarPivotOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;

            switch (UpperNavBarPivot.SelectedIndex)
            {
                case 0:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch,new SearchPageNavigationArgs());
                    break;
                case 1:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMangaSearch,
                        new SearchPageNavigationArgs {Anime = false});
                    break;
                case 2:
                    _initialized = false;
                    UpperNavBarPivot.SelectedIndex = ViewModel.PrevArgs.Anime ? 0 : 1;
                    _initialized = true;
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterSearch);
                    break;
                case 3:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch,new SearchPageNavigationArgs {ByGenre = true});
                    break;
                case 4:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch,new SearchPageNavigationArgs {ByStudio = true});
                    break;
            }
        }

        private void SelectionGridViewOnClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is AnimeGenres)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList,new AnimeListPageNavigationArgs((AnimeGenres)e.ClickedItem));
            else
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs((AnimeStudios)e.ClickedItem));
        }
    }
}