using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.Pages;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IMainViewInteractions
    {
#pragma warning disable 4014
        public MainPage()
        {
            InitializeComponent();
            Utils.CheckTiles();
            ViewModelLocator.Main.View = this;
#if DEBUG
            //new MALProfileQuery().GetProfileData();
#endif
        }
#pragma warning restore 4014
        public MainViewModel ViewModel => DataContext as MainViewModel;

        public void Navigate(Type page, object args = null)
        {
            MainContent.Navigate(page, args);
        }

        public void SearchInputFocus(FocusState state)
        {
            SearchInput.Focus(state);
        }

        #region Search

        private void SearchInput_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((e == null || e.Key == VirtualKey.Enter) && SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                ViewModelLocator.Main.OnSearchInputSubmit();
                e.Handled = true;
            }
        }

        #endregion

        private void CurrentStatus_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.LastIndex == PageIndex.PageAnimeList)
                CurrentStatusListFilterSelectorFlyout.ShowAt(sender as FrameworkElement);
        }
    }
}