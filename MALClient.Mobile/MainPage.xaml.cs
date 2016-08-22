using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.UserControls;
using MALClient.Pages;
using MALClient.ViewModels;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

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
            Loaded += (a1,a2) => MobileViewModelLocator.Main.View = this;
            ViewModel.NavigationRequested += Navigate;
        }

        public MainViewModel ViewModel => DataContext as MainViewModel;

        private void Navigate(Type page, object args = null)
        {
            MainContent.Navigate(page, args);
        }

        public void SearchInputFocus(FocusState state)
        {
            SearchInput.Focus(state);
        }

        public void InitSplitter()
        {
            
        }

        public Storyboard CurrentStatusStoryboard => FadeInCurrentStatus;
        public Storyboard CurrentOffStatusStoryboard { get; } //unused
        public Storyboard CurrentOffSubStatusStoryboard => FadeInCurrentSubStatus;
        public Storyboard PinDialogStoryboard => FadeInPinDialogStoryboard;
        public Storyboard HidePinDialogStoryboard => FadeOutPinDialogStoryboard;
        public SplitViewDisplayMode CurrentDisplayMode => MainSplitView.DisplayMode;

        #region Search

        private void SearchInput_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((e == null || e.Key == VirtualKey.Enter) && SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                MobileViewModelLocator.Main.OnSearchInputSubmit();
                e.Handled = true;
            }
        }

        #endregion

        private void CurrentStatus_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.LastIndex == PageIndex.PageAnimeList)
                CurrentStatusListFilterSelectorFlyout.ShowAt(sender as FrameworkElement);
        }

        /// <summary>
        /// Hack for pivot not to consume mouse wheel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinPivotItem_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - e.GetCurrentPoint(ScrollView).Properties.MouseWheelDelta);
        }

        private void PinDialog_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).Name == "PinDialog")
                MobileViewModelLocator.Main.PinDialogViewModel.CloseDialogCommand.Execute(null);
        }

        private void SearchInput_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if ( SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                MobileViewModelLocator.Main.OnSearchInputSubmit();
            }
        }

        private async void ToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10); // wait for it to appear.. nana, this is completly vaild approach... nananaa
            var btn = sender as LockableToggleButton;
            if (btn.IsChecked.GetValueOrDefault(false))
                SearchInput.Focus(FocusState.Keyboard);
        }
    }
}