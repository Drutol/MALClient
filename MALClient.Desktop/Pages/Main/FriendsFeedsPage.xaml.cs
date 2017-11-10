﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
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
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.FriendsFeeds.Init();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.FriendsFeeds.NavigateDeitalsCommand.Execute(e.ClickedItem);
        }

        private void UIElement_OnPointerPressed(object sender, TappedRoutedEventArgs e)
        {
            ViewModelLocator.FriendsFeeds.NavigateProfileCommand.Execute(((sender as FrameworkElement).DataContext as UserFeedEntryModel).User);
        }

        private void UIElement_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as FrameworkElement).Opacity = .7;
        }

        private void UIElement_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as FrameworkElement).Opacity = 1;
        }
    }
}
