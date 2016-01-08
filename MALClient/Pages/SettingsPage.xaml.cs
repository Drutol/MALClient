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
        //(string)ApplicationData.Current.LocalSettings.Values["username"];
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
    }
}
