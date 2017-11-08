using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotificationsHubPage : Page
    {
        public NotificationsHubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NotificationsHub.Init();
            base.OnNavigatedTo(e);
        }

        private void NotificationClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.NotificationsHub.NavigateNotificationCommand.Execute(e.ClickedItem);
        }

        private void NotificationGroup(object sender, ItemClickEventArgs e)
        {
            var model = (e.ClickedItem as NotificationsHubViewModel.NotificationGroupViewModel);
            if (model.NotificationType == ViewModelLocator.NotificationsHub.CurrentNotificationType)
            {
                ViewModelLocator.NotificationsHub.CurrentNotificationType = null;
                GroupsList.SelectionMode = ListViewSelectionMode.None;
            }
            else
            {
                GroupsList.SelectionMode = ListViewSelectionMode.Single;
                GroupsList.SelectedItem = e.ClickedItem;
                ViewModelLocator.NotificationsHub.CurrentNotificationType = model.NotificationType;
            }
        }
    }
}
