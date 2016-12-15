using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using WinRTXamlToolkit.Controls.Extensions;

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
            ViewModelLocator.MalMessageDetails.PropertyChanged += MalMessageDetailsOnPropertyChanged;
        }

        private async void MalMessageDetailsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.MalMessageDetails.MessageSet))
            {
                await Task.Delay(50);
                ListView.ScrollToBottom();
            }
        }

        private MalMessageDetailsViewModel ViewModel => DataContext as MalMessageDetailsViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as MalMessageDetailsNavArgs;
            base.OnNavigatedTo(e);
        }
    }
}