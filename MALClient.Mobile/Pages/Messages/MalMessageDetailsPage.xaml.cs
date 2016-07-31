using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Messages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MalMessageDetailsPage : Page
    {
        private MalMessageDetailsNavArgs _lastArgs;

        public MalMessageDetailsPage()
        {
            InitializeComponent();
            Loaded += (sender, args) => ViewModel.Init(_lastArgs);
        }

        private MalMessageDetailsViewModel ViewModel => DataContext as MalMessageDetailsViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as MalMessageDetailsNavArgs;
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageMessanging,null);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
        }
    }
}