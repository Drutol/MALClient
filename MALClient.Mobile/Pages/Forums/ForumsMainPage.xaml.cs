using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
    public sealed partial class ForumsMainPage : Page
    {
        public ForumsMainViewModel ViewModel => ViewModelLocator.ForumsMain;

        private ForumsNavigationArgs _args;

        public ForumsMainPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.NavigationRequested += ViewModelOnNavigationRequested;
            ViewModel.Init(_args);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _args = e.Parameter as ForumsNavigationArgs;
            base.OnNavigatedTo(e);
        }

        private void ViewModelOnNavigationRequested(int page, object args)
        {
            switch((ForumsPageIndex)page)
            {
                case ForumsPageIndex.PageIndex:
                    MainForumContentFrame.Navigate(typeof(ForumIndexPage), args);
                    break;
                case ForumsPageIndex.PageBoard:
                    MainForumContentFrame.Navigate(typeof(ForumBoardPage), args);
                    break;
                case ForumsPageIndex.PageTopic:
                    MainForumContentFrame.Navigate(typeof(ForumTopicPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
            
        }

        private void PinnedButtonOnRightClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            (FlyoutBase.GetAttachedFlyout(btn)).ShowAt(btn);
        }

        private void PinnedButtonOnHoldingClick(object sender, HoldingRoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            (FlyoutBase.GetAttachedFlyout(btn)).ShowAt(btn);
        }

        private void PinnedTopicListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            PinnedTopicListView.SelectedIndex = -1;
            ViewModel.SelectedForumTopicLightEntry = e.ClickedItem as ForumTopicLightEntry;
            PinnedTopicsFlyout.Hide();
        }
    }
}
