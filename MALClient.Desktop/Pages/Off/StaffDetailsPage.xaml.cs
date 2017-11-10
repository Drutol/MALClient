using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Off
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StaffDetailsPage : Page
    {
        private StaffDetailsNaviagtionArgs _lastArgs;

        public StaffDetailsPage()
        {
            this.InitializeComponent();
            Loaded += (a, b) =>
            {
                var vm = ViewModelLocator.StaffDetails;
                vm.OnPivotItemSelectionRequest += index => Pivot.SelectedIndex = index;
                vm.Init(_lastArgs);
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as StaffDetailsNaviagtionArgs;
            base.OnNavigatedTo(e);
        }
    }

   
}
