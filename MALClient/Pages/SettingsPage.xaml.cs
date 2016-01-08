using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.UserControls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private bool _initialized = false;
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ToggleCache.IsOn = (bool)(ApplicationData.Current.LocalSettings.Values["EnableCache"] ?? false);
            ComboCachePersistency.SelectedIndex =
                SecondsToIndexHelper((int) (ApplicationData.Current.LocalSettings.Values["CachePersistency"] ?? 3600));

            PopulateCachedEntries();
            Utils.GetMainPageInstance()?.SetStatus("Settings");
            _initialized = true;
            base.OnNavigatedTo(e);
        }


        private async void PopulateCachedEntries()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.DisplayName.Contains("anime_data"))
                {
                    ListCurrentlyCached.Items.Add(new CachedEntryItem(file));
                }
            }
            if(files.Count == 0)
                ListEmptyNotice.Visibility = Visibility.Visible;
            else
            {
                ListEmptyNotice.Visibility = Visibility.Collapsed;
            }
        }
        
        
        
        
        /// <summary>
        /// Converts seconds to combo box item index.
        /// </summary>
        /// <returns></returns>
        private int SecondsToIndexHelper(int secs)
        {
            switch (secs)
            {
                case 600: //10m
                    return 0;
                case 3600: //1h
                    return 1;
                case 7200: //2h
                    return 2;
                case 10800: //3h
                    return 3;
                case 18000: //5h
                    return 4;
                case 36000: //10h
                    return 5;
                case 86400: //1d
                    return 6;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int IndexToSecondsHelper(int index)
        {
            switch (index)
            {
                case 0: //10m
                    return 600;
                case 1: //1h
                    return 3600;
                case 2: //2h
                    return 7200;
                case 3: //3h
                    return 10800;
                case 4: //5h
                    return 18000;
                case 5: //10h
                    return 36000;
                case 6: //1d
                    return 86400;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeCachePersistency(object sender, SelectionChangedEventArgs e)
        {
            if(!_initialized)
                return;;
            var cmb = sender as ComboBox;
            ApplicationData.Current.LocalSettings.Values["CachePersistency"] = IndexToSecondsHelper(cmb.SelectedIndex);
        }

        private void ToggleDataCaching(object sender, RoutedEventArgs e)
        {
            if(!_initialized)
                return;
            ApplicationData.Current.LocalSettings.Values["EnableCache"] = ToggleCache.IsOn;
        }
    }
}
