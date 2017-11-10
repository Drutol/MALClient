using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Forums.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumIndexPage : Page
    {
        public ForumIndexViewModel ViewModel => ViewModelLocator.ForumsIndex;

        public ForumIndexPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init();
            base.OnNavigatedTo(e);
        }

        private void BoardGridOnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateBoardCommand.Execute((e.ClickedItem as ForumBoardEntryViewModel).Board);
        }

        private void BoardGridOnRightClick(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var flyout = FlyoutBase.GetAttachedFlyout(item);
            flyout.ShowAt(item);
        }

        private void RecentPostOnClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateRecentPostCommand.Execute(e.ClickedItem as ForumPostEntry);
        }
    }
}
