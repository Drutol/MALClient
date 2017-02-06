using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;
using MALClient.XShared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopularVideosPage : Page
    {
        private PopularVideosViewModel ViewModel => ViewModelLocator.PopularVideos;

        public PopularVideosPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModelLocator.PopularVideos.Init();
            base.OnNavigatedTo(e);
        }

        private void BtnNavDetails(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn.DataContext as AnimeVideoData;

            ViewModelLocator.PopularVideos.NavDetailsCommand.Execute(item);

        }

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.Loading = true;
            await Task.Delay(1);
            await AnimeDetailsPageViewModel.OpenVideo(e.ClickedItem as AnimeVideoData);
            ViewModel.Loading = false;
        }
    }
}
