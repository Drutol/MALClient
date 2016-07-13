using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MALClient.Pages;
using MALClient.Utils.Enums;
using MALClient.ViewModels;

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
            Loaded += (a1,a2) => ViewModelLocator.Main.View = this;
        }

        public MainViewModel ViewModel => DataContext as MainViewModel;
        public Tuple<int, string> InitDetails { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitDetails = e.Parameter as Tuple<int, string>;
        }


        public void Navigate(Type page, object args = null)
        {
            MainContent.Navigate(page, args);
        }

        public void SearchInputFocus(FocusState state)
        {
            SearchInput.Focus(state);
        }

        public Storyboard PinDialogStoryboard => FadeInPinDialogStoryboard;
        public Storyboard CurrentStatusStoryboard => FadeInCurrentStatus;
        public Storyboard CurrentOffSubStatusStoryboard => FadeInCurrentSubStatus;
        public Storyboard HidePinDialogStoryboard => FadeOutPinDialogStoryboard;

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
                ViewModelLocator.Main.PinDialogViewModel.CloseDialogCommand.Execute(null);
        }

        private void SearchInput_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if ( SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                ViewModelLocator.Main.OnSearchInputSubmit();
            }
        }
    }
}