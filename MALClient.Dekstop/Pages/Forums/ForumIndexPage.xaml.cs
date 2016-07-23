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
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Forums;

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
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                new ForumsBoardNavigationArgs((e.ClickedItem as ForumBoardEntryViewModel).Board));
        }
    }
}
