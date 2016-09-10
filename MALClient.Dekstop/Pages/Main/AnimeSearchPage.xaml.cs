using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared.Items;
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
    }
}