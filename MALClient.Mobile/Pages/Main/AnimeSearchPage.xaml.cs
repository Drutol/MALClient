using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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
            ViewModel.Init(e.Parameter as SearchPageNavigationArgs);
            base.OnNavigatedTo(e);
        }
    }
}