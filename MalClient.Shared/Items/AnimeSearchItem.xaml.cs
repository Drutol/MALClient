using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Library;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Shared.Items
{
    public sealed partial class AnimeSearchItem : UserControl
    {
        AnimeSearchItemViewModel ViewModel => DataContext as AnimeSearchItemViewModel;

        public AnimeSearchItem()
        {
            InitializeComponent();
        }

        private async void CopyLinkToClipboardCommand(object sender, RoutedEventArgs e)
        {
            FlyoutMore.Hide();
            var dp = new DataPackage();
            dp.SetText($"http://www.myanimelist.net/{(ViewModel.AnimeMode ? "anime" : "manga")}/{ViewModel.Id}");
            Clipboard.SetContent(dp);
            FlyoutMore.Hide();
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