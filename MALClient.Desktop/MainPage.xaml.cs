﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.UWP.Adapters;
using MALClient.UWP.Shared.UserControls;
using MALClient.UWP.Shared.ViewModels;
using MALClient.UWP.Shared.ViewModels.Interfaces;
using MALClient.UWP.ViewModels;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient.UWP
{
	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page, IMainViewInteractions
	{
		private double _prevOffContntWidth;
		private Timer _timer;
		private bool _paneState;// _currentPaneState;

		public MainPage()
		{
			InitializeComponent();

		    DataTransferManager.GetForCurrentView().DataRequested += ShareDataRequested;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
			coreTitleBar.ExtendViewIntoTitleBar = false;

			Loaded += (sender, args) =>
			{
				var vm = DesktopViewModelLocator.Main;
				vm.MainNavigationRequested += Navigate;
				vm.OffNavigationRequested += NavigateOff;
				vm.PropertyChanged += VmOnPropertyChanged;
				UWPViewModelLocator.PinTileDialog.ShowPinDialog += () =>
				{
					PinDialogStoryboard.Begin();
				};
				vm.MediaElementCollapsed += VmOnMediaElementCollapsed;
				UWPViewModelLocator.PinTileDialog.HidePinDialog += HidePinDialog;
				DesktopViewModelLocator.Main.View = this;
				//StartAdsTimeMeasurements();
				//ViewModelLocator.Settings.OnAdsMinutesPerDayChanged += SettingsOnOnAdsMinutesPerDayChanged;
				ViewModelLocator.GeneralMain.ChangelogVisibility = ResourceLocator.ChangelogProvider.NewChangelog;
			    ResourceLocator.ShareManager.TimerStateChanged += ShareManagerOnTimerStateChanged;
            };
		}

	    private async void ShareManagerOnTimerStateChanged(object sender, bool e)
	    {
	        //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
	        //{
	        //    if (e)
	        //    {
	        //        ExampleVSCodeInAppNotification.Show();
	        //    }
	        //    else
	        //    {
	        //        ExampleVSCodeInAppNotification.Dismiss();
	        //    }
	        //});
	    }

	    private void VmOnMediaElementCollapsed()
		{
			MediaElement.Stop();
		}

		//#region AdsTimer
		//private void SettingsOnOnAdsMinutesPerDayChanged()
		//{
		//	if (Settings.AdsEnable)
		//	{
		//		var passed = (int)(ResourceLocator.ApplicationDataService["AdsTimeToday"] ?? 0);
		//		_timer?.Dispose();
		//		if (passed < Settings.AdsSecondsPerDay || Settings.AdsSecondsPerDay == 0)
		//		{
		//			ViewModelLocator.GeneralMain.AdsContainerVisibility = true;
		//			_timer = new Timer(AdTimerCallback, null, 0, 10000);
		//		}
		//		else
		//		{
		//			ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
		//		}
		//	}
		//	else if (_timer != null && !Settings.AdsEnable)
		//	{
		//		ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
		//		_timer?.Dispose();
		//		_timer = null;
		//	}
		//	else if (!Settings.AdsEnable)
		//		ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
		//}

		//private void StartAdsTimeMeasurements()
		//{
		//	var day = ResourceLocator.ApplicationDataService["AdsCurrentDay"];
		//	if (day != null)
		//	{
		//		if ((int)day != DateTime.Today.DayOfYear)
		//			ResourceLocator.ApplicationDataService["AdsTimeToday"] = 0;
		//	}
		//	ResourceLocator.ApplicationDataService["AdsCurrentDay"] = DateTime.Today.DayOfYear;
		//	if (Settings.AdsEnable)
		//	{
		//		_timer = new Timer(AdTimerCallback, null, 0, 10000);
		//		ViewModelLocator.GeneralMain.AdsContainerVisibility = true;
		//	}
		//	else
		//	{
		//		AdControl.Suspend();
		//	}
		//}

		//private async void AdTimerCallback(object state)
		//{
		//	var passed = (int)(ResourceLocator.ApplicationDataService["AdsTimeToday"] ?? 0);
		//	passed += 10;
		//	ResourceLocator.ApplicationDataService["AdsTimeToday"] = passed;
		//	await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
		//		() => AdControl.Resume());
		//	if (!Settings.AdsEnable || (Settings.AdsSecondsPerDay != 0 && passed > Settings.AdsSecondsPerDay))
		//	{
		//		await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
		//			() => ViewModelLocator.GeneralMain.AdsContainerVisibility = false);
		//		_timer?.Dispose();
		//		_timer = null;
		//	}
		//}



		//#endregion

		private void HidePinDialog()
		{
			HidePinDialogStoryboard.Completed += SbOnCompleted;
			HidePinDialogStoryboard.Begin();
		}

		private void SbOnCompleted(object sender, object o)
		{
			(sender as Storyboard).Completed -= SbOnCompleted;
			UWPViewModelLocator.PinTileDialog.RaisePropertyChanged("GeneralVisibility");
			UnloadObject(PinDialog);
		}

		private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "MenuPaneState")
			{
				var paneState = (sender as MainViewModel).MenuPaneState;
				_paneState = paneState;

				HamburgerControl.Width = _paneState ? 250 : 48;
				HamburgerTopGrid.Width = _paneState ? 250 : 48;
			}
			else if (args.PropertyName == "OffContentVisibility")
			{
				SplitterColumn.Width = new GridLength(ViewModelLocator.GeneralMain.OffContentVisibility ? 16 : 0);
			}
			else if (args.PropertyName == nameof(ViewModelLocator.GeneralMain.AdsContainerVisibility))
			{
				//if (ViewModelLocator.GeneralMain.AdsContainerVisibility)
				//	AdControl.Resume();
				//else
				//	AdControl.Suspend();
			}

		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!_paneState)
				return;

			if (e.NewSize.Width > 720)
			{
				HamburgerControl.Width = 250;
				HamburgerTopGrid.Width = 250;
				//_currentPaneState = true;
			}
			else
			{
				HamburgerControl.Width = 48;
				HamburgerTopGrid.Width = 48;
				//_currentPaneState = false;
			}
		}

		public Tuple<int, string> InitDetails { get; private set; }

		private void Navigate(Type page, object args = null)
		{
			MainContent.Navigate(page, args);
		}

		private void NavigateOff(Type page, object args = null)
		{
			OffContent.Navigate(page, args);
		}

		public SplitViewDisplayMode CurrentDisplayMode { get; }

		public void SearchInputFocus(FocusState state)
		{
			SearchInput.Focus(state);
		}

		public void InitSplitter()
		{
			try
			{
				RootContentGrid.ColumnDefinitions[2].Width =
					new GridLength(_prevOffContntWidth == 0
						? (_prevOffContntWidth = GetStartingSplitterWidth())
						: _prevOffContntWidth);
				OffContent.UpdateLayout();
			}
			catch (Exception)
			{
				//magic
			}

		}

		public Storyboard PinDialogStoryboard => FadeInPinDialogStoryboard;
		public Storyboard CurrentStatusStoryboard => FadeInCurrentStatus;
		public Storyboard CurrentOffStatusStoryboard => FadeInCurrentOffStatus;
		public Storyboard CurrentOffSubStatusStoryboard => FadeInCurrentSubStatus;
		public Storyboard HidePinDialogStoryboard => FadeOutPinDialogStoryboard;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			InitDetails = e.Parameter as Tuple<int, string>;
		}

		private double GetStartingSplitterWidth()
		{
			return ApplicationView.GetForCurrentView().VisibleBounds.Width > 1400.0 ? 535 : 465;
		}

		#region Search

		private void SearchInput_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (SearchInput.Text.Length >= 2)
			{
				SearchInput.IsEnabled = false; //reset input
				SearchInput.IsEnabled = true;
				ViewModelLocator.GeneralMain.OnSearchInputSubmit();
			}
		}

		#endregion

		private void CustomGridSplitter_OnDraggingCompleted(object sender, EventArgs e)
		{
			if (DesktopViewModelLocator.Main.CurrentMainPageKind == PageIndex.PageAnimeList && RootContentGrid.ColumnDefinitions[2].ActualWidth < _prevOffContntWidth &&
				RootContentGrid.ColumnDefinitions[2].ActualWidth - _prevOffContntWidth < -50)
			{
				var vm = ViewModelLocator.AnimeList;
				if (vm.AreThereItemsWaitingForLoad)
					vm.RefreshList();
			}
			if (RootContentGrid.ColumnDefinitions[2].ActualWidth == 0)
				DesktopViewModelLocator.Main.HideOffContentCommand.Execute(null);

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
					ViewModelLocator.NavMgr.CurrentOffViewOnBackRequested();
			}
		}

		private void MainContent_OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
			{
				var properties = e.GetCurrentPoint(this).Properties;
				if (properties.IsXButton1Pressed)
					ViewModelLocator.NavMgr.CurrentMainViewOnBackRequested();
			}
		}

		/// <summary>
		///     Hack for pivot not to consume mouse wheel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PinPivotItem_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
		{
			ScrollView.ChangeView(null, ScrollView.VerticalOffset -
										e.GetCurrentPoint(ScrollView).Properties.MouseWheelDelta, null);
		}

		private void PinDialog_OnTapped(object sender, TappedRoutedEventArgs e)
		{
			if ((e.OriginalSource as FrameworkElement).Name == "PinDialog")
				UWPViewModelLocator.PinTileDialog.CloseDialogCommand.Execute(null);
		}

		private void ToggleButton_OnClick(object sender, RoutedEventArgs e)
		{
			var btn = sender as LockableToggleButton;
			if (btn.IsChecked.GetValueOrDefault(false))
				SearchInput.Focus(FocusState.Keyboard);
		}

		private void ButtonCloseChangelogOnClick(object sender, RoutedEventArgs e)
		{
			ViewModelLocator.GeneralMain.ChangelogVisibility = false;
		}

	    private void ShareButtonOnClick(object sender, RoutedEventArgs e)
	    {
	        DataTransferManager.ShowShareUI();
            ExampleVSCodeInAppNotification.Dismiss();
        }

	    private void ShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
	    {
	        args.Request.Data.Properties.Title = "MyAnimeList update";
	        args.Request.Data.Properties.Description = "Share your anime endeavours!";
            args.Request.Data.SetText(ResourceLocator.ShareManager.GenerateMessage());
        }

	    private void DismissButtonOnClick(object sender, RoutedEventArgs e)
	    {
	        ExampleVSCodeInAppNotification.Dismiss();
        }
	}
}