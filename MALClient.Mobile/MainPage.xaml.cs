using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared.UserControls;
using MALClient.Shared.ViewModels;
using MALClient.Pages;
using MALClient.Shared.ViewModels.Interfaces;
using MALClient.ViewModels;
using MALClient.XShared.Utils;
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
        private Timer _timer;

        public MainPage()
        {
            InitializeComponent();
            Loaded += (a1, a2) =>
            {
                MobileViewModelLocator.Main.View = this;
                ViewModelLocator.GeneralMain.MediaElementCollapsed += GeneralMainOnMediaElementCollapsed;
                UWPViewModelLocator.PinTileDialog.ShowPinDialog += () =>
                {
                    PinDialogStoryboard.Begin();
                };
                ViewModelLocator.GeneralMain.PropertyChanged += GeneralMainOnPropertyChanged;
                UWPViewModelLocator.PinTileDialog.HidePinDialog += HidePinDialog;
                StartAdsTimeMeasurements();
                ViewModelLocator.Settings.OnAdsMinutesPerDayChanged += SettingsOnOnAdsMinutesPerDayChanged;
            };
            ViewModel.MainNavigationRequested += Navigate;
        }

        private void GeneralMainOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ViewModelLocator.GeneralMain.AdsContainerVisibility))
            {
                if (ViewModelLocator.GeneralMain.AdsContainerVisibility)
                    AdControl.Resume();
                else
                    AdControl.Suspend();
            }
        }

        #region AdsTimer
        private void SettingsOnOnAdsMinutesPerDayChanged()
        {
            if (Settings.AdsEnable)
            {
                var passed = (int)(ResourceLocator.ApplicationDataService["AdsTimeToday"] ?? 0);
                _timer?.Dispose();
                if (passed < Settings.AdsSecondsPerDay || Settings.AdsSecondsPerDay == 0)
                {
                    ViewModelLocator.GeneralMain.AdsContainerVisibility = true;
                    _timer = new Timer(AdTimerCallback, null, 0, 10000);
                }
                else
                {
                    ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
                }
            }
            else if (_timer != null && !Settings.AdsEnable)
            {
                ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
                _timer?.Dispose();
                _timer = null;
            }
            else if (!Settings.AdsEnable)
                ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
        }

        private void StartAdsTimeMeasurements()
        {
            var day = ResourceLocator.ApplicationDataService["AdsCurrentDay"];
            if (day != null)
            {
                if ((int)day != DateTime.Today.DayOfYear)
                    ResourceLocator.ApplicationDataService["AdsTimeToday"] = 0;
            }
            ResourceLocator.ApplicationDataService["AdsCurrentDay"] = DateTime.Today.DayOfYear;
            if (Settings.AdsEnable)
            {
                _timer = new Timer(AdTimerCallback, null, 0, 10000);
                ViewModelLocator.GeneralMain.AdsContainerVisibility = true;
            }
            else
            {
                AdControl.Suspend();
            }
        }

        private async void AdTimerCallback(object state)
        {
            var passed = (int)(ResourceLocator.ApplicationDataService["AdsTimeToday"] ?? 0);
            passed += 10;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => AdControl.Resume());
            ResourceLocator.ApplicationDataService["AdsTimeToday"] = passed;
            if (!Settings.AdsEnable || (Settings.AdsSecondsPerDay != 0 && passed > Settings.AdsSecondsPerDay))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => ViewModelLocator.GeneralMain.AdsContainerVisibility = false);
                _timer?.Dispose();
                _timer = null;
            }
        }
        #endregion

        private void GeneralMainOnMediaElementCollapsed()
        {
            MediaElement.Stop();
        }

        private void HidePinDialog()
        {
            HidePinDialogStoryboard.Completed += SbOnCompleted;
            HidePinDialogStoryboard.Begin();
        }

        private void SbOnCompleted(object sender, object o)
        {
            (sender as Storyboard).Completed -= SbOnCompleted;
            UWPViewModelLocator.PinTileDialog.RaisePropertyChanged("GeneralVisibility");
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
            if (ViewModel.CurrentMainPage == PageIndex.PageAnimeList)
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
                UWPViewModelLocator.PinTileDialog.CloseDialogCommand.Execute(null);
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