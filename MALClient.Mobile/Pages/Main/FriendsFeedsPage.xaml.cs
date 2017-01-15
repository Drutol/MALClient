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
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendsFeedsPage : Page
    {
        public FriendsFeedsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModelLocator.FriendsFeeds.Init();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.FriendsFeeds.NavigateDeitalsCommand.Execute(e.ClickedItem);
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModelLocator.FriendsFeeds.NavigateProfileCommand.Execute(((sender as FrameworkElement).DataContext as UserFeedEntryModel).User);
        }
    }
}
