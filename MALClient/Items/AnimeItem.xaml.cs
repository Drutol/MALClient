using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm;
using MALClient.Pages;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl , IAnimeItemInteractions
    {

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        public AnimeItem(AnimeItemViewModel vm)
        {
            InitializeComponent();
            vm.View = this;
            DataContext = vm;      
        }  

        private bool _expandState;
        private void ShowMore(object sender, RoutedEventArgs e)
        {
            if (!_expandState)
            {
                SynopsisShow.Begin();
                _expandState = true;
            }
            else
            {
                SynopsisHide.Begin();
                _expandState = false;
            }
        }

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ViewModel.ChangeWatchedEps();
        }

        #region Swipe

        private Point _initialPoint;
        private bool _manipulating;

        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
            _manipulating = true;
        }

        private void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && _manipulating)
            {
                Point currentpoint = e.Position;
                if (currentpoint.X - _initialPoint.X >= 70) // swipe right
                {
                    e.Complete();
                    e.Handled = true;
                    _manipulating = false;
                    ViewModel.NavigateDetails();
                }
            }
        }

        #endregion
       
        #region CustomTilePin

        private void TxtTileUrl_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            var txt = sender as TextBox;
            txt.IsEnabled = false; //reset input
            txt.IsEnabled = true;
            ViewModel.PinTile();
            CloseTileUrlInput(null, null);
        }

        public void OpenTileUrlInput()
        {
            TxtTileUrl.Text = "";
            //Utils.GetMainPageInstance().AnimeListScrollTo(this);
            ViewModel.TileUrlInputVisibility = Visibility.Visible;
            TxtTileUrl.Focus(FocusState.Keyboard);
        }

        private void CloseTileUrlInput(object sender, RoutedEventArgs e)
        {
            ViewModel.TileUrlInputVisibility = Visibility.Collapsed;
        }

        #endregion

        public Flyout WatchedFlyout => WatchedEpsFlyout;
    }
}