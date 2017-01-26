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
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopularVideosPage : Page
    {
        public PopularVideosPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.PopularVideos.Init();
            base.OnNavigatedTo(e);
        }

        private void BtnNavDetails(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn.DataContext as AnimeVideoData;

            ViewModelLocator.PopularVideos.NavDetailsCommand.Execute(item);

        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            AnimeDetailsPageViewModel.OpenVideo(e.ClickedItem as AnimeVideoData);
        }
    }
}
