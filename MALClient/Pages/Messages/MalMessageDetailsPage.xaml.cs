using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models;
using MALClient.ViewModels.Messages;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Messages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MalMessageDetailsPage : Page
    {
        private MalMessageModel _lastArgs;

        public MalMessageDetailsPage()
        {
            InitializeComponent();
            Loaded += (sender, args) => ViewModel.Init(_lastArgs);
        }

        private MalMessageDetailsViewModel ViewModel => DataContext as MalMessageDetailsViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as MalMessageModel;
            base.OnNavigatedTo(e);
        }
    }
}