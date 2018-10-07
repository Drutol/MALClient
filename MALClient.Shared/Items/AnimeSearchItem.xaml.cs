using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Shared.Items
{
    public sealed partial class AnimeSearchItem : UserControl
    {
        AnimeSearchItemViewModel ViewModel => DataContext as AnimeSearchItemViewModel;

        public AnimeSearchItem()
        {
            InitializeComponent();
        }

        private void CopyLinkToClipboardCommand(object sender, RoutedEventArgs e)
        {
            FlyoutMore.Hide();
			var dp = new DataPackage();
			dp.SetText($"http://www.myanimelist.net/{(ViewModel.AnimeMode ? "anime" : "manga")}/{ViewModel.Id}");
			Clipboard.SetContent(dp);
			UWPUtilities.GiveStatusBarFeedback("Copied to clipboard...");
        }

        private async void OpenInMALCommand(object sender, RoutedEventArgs e)
        {
            FlyoutMore.Hide();
            await
                Launcher.LaunchUriAsync(
                    new Uri(
                        $"https://myanimelist.net/{(ViewModel.AnimeMode ? "anime" : "manga")}/{ViewModel.Id}"));
        }
    }
}