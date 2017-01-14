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
using MALClient.Models.Models.Forums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumIndexPage : Page
    {
        public ForumIndexViewModel ViewModel => ViewModelLocator.ForumsIndex;

        public ForumIndexPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init();
            base.OnNavigatedTo(e);
        }

        private void BoardGridOnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex,null);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                new ForumsBoardNavigationArgs((e.ClickedItem as ForumBoardEntryViewModel).Board));
        }

        private void BoardGridOnRightClick(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var flyout = FlyoutBase.GetAttachedFlyout(item);
            flyout.ShowAt(item);
        }

        private void RecentPostOnClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, null);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                new ForumsTopicNavigationArgs((e.ClickedItem as ForumPostEntry).Id,ForumBoards.Creative));
        }
    }
}
