using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MalClient.Shared.Models.Misc;
using MalClient.Shared.Utils;
using MALClient.ViewModels.Off;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Off.SettingsPages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsCachingPage : Page
    {
        private bool _initialized;

        public SettingsCachingPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public SettingsPageViewModel ViewModel => DataContext as SettingsPageViewModel;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ComboCachePersistency.SelectedIndex = SecondsToIndexHelper(Settings.CachePersitence);
            _initialized = true;
        }


        private void ChangeCachePersistency(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;
            var cmb = sender as ComboBox;
            Settings.CachePersitence = IndexToSecondsHelper(cmb.SelectedIndex);
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

        /// <summary>
        ///     Converts seconds to combo box item index.
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

        private async void RemoveAllAnimeDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails")).DeleteAsync(
                    StorageDeleteOption.PermanentDelete);

                (sender as Button).IsEnabled = false;
            }
            catch (Exception)
            {
                //
            }
        }

        private void BtnCachedEntryRemove(object sender, RoutedEventArgs e)
        {
            ViewModel.CachedEntries.Remove((sender as FrameworkElement).Tag as CachedEntryModel);
        }
    }
}