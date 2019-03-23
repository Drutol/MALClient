using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Off.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsAdsPage : Page
    {
        private bool _initialized;

        public SettingsAdsPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //switch (Settings.AdsSecondsPerDay)
            //{
            //    case 0:
            //        LengthCombobox.SelectedIndex = 0;
            //        break;                  
            //    case 300:
            //        LengthCombobox.SelectedIndex = 1;
            //        break;
            //    case 600:
            //        LengthCombobox.SelectedIndex = 2;
            //        break;
            //    case 900:
            //        LengthCombobox.SelectedIndex = 3;
            //        break;
            //    case 1200:
            //        LengthCombobox.SelectedIndex = 4;
            //        break;
            //    case 1800:
            //        LengthCombobox.SelectedIndex = 5;
            //        break;
                    
            //}
            //_initialized = true;
        }

        private void LengthCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (_initialized)
            //    ViewModelLocator.Settings.AdsSecondsPerDay = (int) (LengthCombobox.SelectedItem as FrameworkElement).Tag;

        }
    }
}
