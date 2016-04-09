using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.Comm;
using MALClient.Comm.Anime;
using MALClient.UserControls;
using MALClient.ViewModels;
using WinRTXamlToolkit.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IMainViewInteractions
    {

        public MainPage()
        {
            InitializeComponent();
            Utils.CheckTiles();
            ViewModelLocator.Main.View = this;
        }

        public void Navigate(Type page, object args = null)
        {
            MainContent.Navigate(page, args);
        }

        public void NavigateOff(Type page, object args = null)
        {
            OffContent.Navigate(page, args);
        }

        public void SearchInputFocus(FocusState state)
        {
            SearchInput.Focus(state);
        }

        private double _prevOffContntWidth = 0;
        public void InitSplitter()
        {
            RootContentGrid.ColumnDefinitions[2].Width = new GridLength(_prevOffContntWidth == 0 ? (_prevOffContntWidth = 420) : _prevOffContntWidth);
            OffContent.UpdateLayout();
        }

        public HamburgerControl Hamburger => HamburgerControl;

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

        private void CustomGridSplitter_OnDraggingCompleted(object sender, EventArgs e)
        {
            if (RootContentGrid.ColumnDefinitions[2].ActualWidth < _prevOffContntWidth && RootContentGrid.ColumnDefinitions[2].ActualWidth - _prevOffContntWidth < -50)
            {
                var vm = ViewModelLocator.AnimeList;
                if(vm.AreThereItemsWaitingForLoad)
                    ViewModelLocator.AnimeList.RefreshList();
            }
            
            _prevOffContntWidth = RootContentGrid.ColumnDefinitions[2].ActualWidth;
            (DataContext as MainViewModel).OffContentStatusBarWidth = _prevOffContntWidth ;
        }
    }
}