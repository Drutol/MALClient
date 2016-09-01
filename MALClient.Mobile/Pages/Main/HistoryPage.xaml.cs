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
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    public sealed partial class HistoryPage : Page
    {
        private HistoryNavigationArgs _args;

        public HistoryViewModel ViewModel => ViewModelLocator.History;

        public HistoryPage()
        {
            this.InitializeComponent();
            Loaded += (a1, a2) => ViewModel.Init(_args);
        }

        private void AnimeItemOnClick(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>).Item1.NavigateDetails(PageIndex.PageHistory, _args);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            _args = e.Parameter as HistoryNavigationArgs;
            if (_args == null)
            {
                ViewModelLocator.NavMgr.DeregisterBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList,null);
            }
            base.OnNavigatedTo(e);
        }

        private void AnimeItemOnRightClick(object sender, RightTappedRoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeGridItemFlyout(sender as FrameworkElement);
        }

        private void HistoryScrollViewerOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>).Item1.NavigateDetails();
        }
    }
}
