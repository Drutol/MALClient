using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Off.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsNotificationsPage : Page
    {
        private SettingsViewModelBase ViewModel => DataContext as SettingsViewModelBase;
        private bool _initialized;

        public SettingsNotificationsPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var src = new List<MalNotificationsTypes>();
            foreach (MalNotificationsTypes malNotificationsType in Enum.GetValues(typeof(MalNotificationsTypes)))
            {
                if(malNotificationsType != MalNotificationsTypes.Generic)
                    src.Add(malNotificationsType);
            }
            NotificationItemTypes.ItemsSource = src;
            int index = 0;
            switch (Settings.NotificationsRefreshTime)
            {
                case 15:
                    index = 0;
                    break;
                case 30:
                    index = 1;
                    break;
                case 45:
                    index = 2;
                    break;
                case 60:
                    index = 3;
                    break;
                case 120:
                    index = 4;
                    break;
            }
            FrequencyCombobox.SelectedIndex = index;
            _initialized = true;
        }

        private void ButtonNotificationType_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as CheckBox;
            var val = (MalNotificationsTypes)btn.Tag;
            if (btn.IsChecked == true)
                ViewModel.EnabledNotificationTypes |= val;
            else
                ViewModel.EnabledNotificationTypes &= ~val;
        }

        private void FrequencyCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_initialized)
                ViewModel.NotificationsRefreshTime = (int)((sender as ComboBox).SelectedItem as ComboBoxItem).Tag;
        }
    }
}
