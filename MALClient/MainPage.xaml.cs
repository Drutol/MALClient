using System;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Comm.Anime;

using MALClient.UserControls;
using MALClient.ViewModels;
using XamlCropControl;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IMainViewInteractions
    {
        private double _prevOffContntWidth;
        public Tuple<int,string> InitDetails { get; private set; }
        public MainPage()
        {
            InitializeComponent();
            //Utils.CheckTiles();
            Loaded += (sender, args) =>
            {

                LogoImage.Source =
                    new BitmapImage(
                        new Uri(Settings.SelectedTheme == ApplicationTheme.Dark
                            ? "ms-appx:///Assets/upperappbarlogowhite.png"
                            : "ms-appx:///Assets/upperappbarlogoblue.png"));

                ViewModelLocator.Main.View = this;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitDetails = e.Parameter as Tuple<int,string>;
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

        public void InitSplitter()
        {
            RootContentGrid.ColumnDefinitions[2].Width =
                new GridLength(_prevOffContntWidth == 0
                    ? (_prevOffContntWidth = GetStartingSplitterWidth())
                    : _prevOffContntWidth);
            OffContent.UpdateLayout();
        }



        public HamburgerControl Hamburger => HamburgerControl;
        public Grid GridRootContent => RootContentGrid;
        public Image Logo => LogoImage;
        public Storyboard PinDialogStoryboard => FadeInPinDialogStoryboard;
        public Storyboard CurrentStatusStoryboard => FadeInCurrentStatus;
        public Storyboard CurrentOffStatusStoryboard => FadeInCurrentOffStatus;

        private double GetStartingSplitterWidth()
        {
            return ApplicationView.GetForCurrentView().VisibleBounds.Width > 1400.0 ? 535 : 420;
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

        private void CustomGridSplitter_OnDraggingCompleted(object sender, EventArgs e)
        {
            if (RootContentGrid.ColumnDefinitions[2].ActualWidth < _prevOffContntWidth &&
                RootContentGrid.ColumnDefinitions[2].ActualWidth - _prevOffContntWidth < -50)
            {
                var vm = ViewModelLocator.AnimeList;
                if (vm.AreThereItemsWaitingForLoad)
                    ViewModelLocator.AnimeList.RefreshList();
            }

            _prevOffContntWidth = RootContentGrid.ColumnDefinitions[2].ActualWidth;
        }

        private void OffContent_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            (DataContext as MainViewModel).OffContentStatusBarWidth = e.NewSize.Width;
        }


        private void OffContent_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(this).Properties;
                if (properties.IsXButton1Pressed)
                    NavMgr.CurrentViewOnBackRequested();
            }
        }
        /// <summary>
        /// Hack for pivot not to consume mouse wheel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinPivotItem_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset -e.GetCurrentPoint(ScrollView).Properties.MouseWheelDelta);
        }

        private void PinDialog_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if((e.OriginalSource as FrameworkElement).Name == "PinDialog")
                ViewModelLocator.Main.PinDialogViewModel.CloseDialogCommand.Execute(null);
        }
    }
}