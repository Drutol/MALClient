using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared;
using MALClient.Shared.ViewModels;
using MALClient.Pages;
using MALClient.Shared.Managers;
using MALClient.UWP.BGTaskNotifications;
using MALClient.ViewModels;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.CommUtils;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using Microsoft.HockeyApp;

namespace MALClient
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static bool _initialized;

        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            MobileViewModelLocator.RegisterDependencies();
            UWPViewModelLocator.RegisterDependencies();
            ResourceLocator.TelemetryProvider.Init();
            Current.RequestedTheme = (ApplicationTheme)Settings.SelectedTheme;
            InitializeComponent();
            Suspending += OnSuspending;
        }



        protected override async void OnActivated(IActivatedEventArgs args)
        {
            OnLaunchedOrActivated(args);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PrelaunchActivated)
                return;

            OnLaunchedOrActivated(e);
        }

        protected async void OnLaunchedOrActivated(IActivatedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
            var rootFrame = Window.Current.Content as Frame;
            Tuple<int, string> navArgs = null;
            Tuple<PageIndex, object> fullNavArgs = null;
            var launchArgs = e as LaunchActivatedEventArgs;
            if (!string.IsNullOrWhiteSpace(launchArgs?.Arguments))
            {

                if (Regex.IsMatch(launchArgs.Arguments, @"[OpenUrl,OpenDetails];.*"))
                {
                    var options = launchArgs.Arguments.Split(';');
                    if (options[0] == TileActions.OpenUrl.ToString())
                        LaunchUri(options[1]);
                    else if (launchArgs.Arguments.Contains('|')) //legacy
                    {
                        var detailArgs = options[1].Split('|');
                        navArgs = new Tuple<int, string>(int.Parse(detailArgs[0]), detailArgs[1]);
                    }
                    else
                    {
                        fullNavArgs = await MalLinkParser.GetNavigationParametersForUrl(options[1]);
                    }
                }
                else
                {
                    fullNavArgs = await MalLinkParser.GetNavigationParametersForUrl(launchArgs.Arguments);
                }
            }
            else
            {
                var activationArgs = e as ToastNotificationActivatedEventArgs;
                if (activationArgs != null)
                {
                    if (activationArgs.Argument.Contains(";"))
                    {
                        var options = activationArgs.Argument.Split(';');
                        if (options[0] == TileActions.OpenUrl.ToString())
                            LaunchUri(options[1]);
                    }
                    else
                    {
                        fullNavArgs = await MalLinkParser.GetNavigationParametersForUrl(activationArgs.Argument);
                    }
                }
            }



            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //nothing
                }
                if (e.PreviousExecutionState == ApplicationExecutionState.NotRunning)
                //Crashed - we have to remove cached anime list
                {
                    if (Settings.IsCachingEnabled)
                    {
                        await DataCache.ClearAnimeListData(); //clear all cached users data
                    }
                }



                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (navArgs != null)
                    MainViewModelBase.InitDetails = navArgs;
                else if (fullNavArgs != null)
                {
                    MainViewModelBase.InitDetailsFull = fullNavArgs;
                }
                rootFrame.Navigate(typeof(MainPage));
            }
            else if (navArgs != null)
            {
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(navArgs.Item1, navArgs.Item2, null, null));
            }

            if (_initialized && fullNavArgs != null)
            {
                ViewModelLocator.GeneralMain.Navigate(fullNavArgs.Item1, fullNavArgs.Item2);
            }
            // Ensure the current window is active
            if (_initialized)
                return;
            Credentials.Init();
            NotificationTaskManager.StartNotificationTask(false);
            NotificationTaskManager.OnNotificationTaskRequested += NotificationTaskManagerOnOnNotificationTaskRequested;
            ImageCache.PerformScheduledCacheCleanup();
            HtmlClassMgr.Init();
            LiveTilesManager.LoadTileCache();
            FavouritesManager.LoadData();
            Window.Current.Activate();
            RateReminderPopUp.ProcessRatePopUp();
            RateReminderPopUp.ProcessDonatePopUp();
            ViewModelLocator.ForumsMain.LoadPinnedTopics();
            ProcessStatusBar();
            ProcessUpdate();
            StoreLogoWorkaroundHacker.Hack();
            _initialized = true;
        }

        private void NotificationTaskManagerOnOnNotificationTaskRequested()
        {
            new NotificationsBackgroundTask().Run(null);
        }

        private async void ProcessUpdate()
        {
            //if (ApplicationData.Current.LocalSettings.Values["AppVersion"] != null
            //    && (string) ApplicationData.Current.LocalSettings.Values["AppVersion"] != Utilities.GetAppVersion())
            //{
            //    var msg =
            //        new MessageDialog(
            //            "This build was supposed to bring ads... but I decided to add forums (beta) instead, rejoice! I don't want to add ads and I won't add them for now at least :)\n\nI'm also resetting review pop-up in order to get fresher opinions... Keep the feedback flowing!",
            //            "About this update");
            //    await msg.ShowAsync();
            //    Settings.RatePopUpEnable = true;
            //    Settings.RatePopUpStartupCounter = 0;
            //}

            ApplicationData.Current.LocalSettings.Values["AppVersion"] = UWPUtilities.GetAppVersion();
        }

        private void ProcessStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Current.RequestedTheme == ApplicationTheme.Dark
                        ? Colors.Black
                        : Colors.White;
                    statusBar.ForegroundColor = Current.RequestedTheme == ApplicationTheme.Dark
                        ? Colors.White
                        : Colors.Black;
                }

                var disp = DisplayInformation.GetForCurrentView();
                DispOnOrientationChanged(disp, null);
                disp.OrientationChanged += DispOnOrientationChanged;
            }
        }

        private void DispOnOrientationChanged(DisplayInformation sender, object args)
        {
            if (sender.CurrentOrientation == DisplayOrientations.Landscape ||
                sender.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                StatusBar.GetForCurrentView().HideAsync();
            else
                StatusBar.GetForCurrentView().ShowAsync();
        }

        private async void LaunchUri(string url)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri(url));
            }
            catch (Exception)
            {
                //wrong url provided
                UWPUtilities.GiveStatusBarFeedback("Invalid target url...");
            }

        }


        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            if (Settings.IsCachingEnabled)
            {
                if (AnimeUpdateQuery.UpdatedSomething)
                    await
                        DataCache.SaveDataForUser(Credentials.UserName,
                            MobileViewModelLocator.AnimeList.AllLoadedAnimeItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Anime);
                if (MangaUpdateQuery.UpdatedSomething)
                    await
                        DataCache.SaveDataForUser(Credentials.UserName,
                            MobileViewModelLocator.AnimeList.AllLoadedMangaItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Manga);
            }
            await DataCache.SaveVolatileData();
            await DataCache.SaveHumMalIdDictionary();
            try
            {
                foreach (var file in await ApplicationData.Current.TemporaryFolder.GetFilesAsync(CommonFileQuery.DefaultQuery))
                    if (file.Name.Contains("_cropTemp"))
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //well...
            }
            await ViewModelLocator.ForumsMain.SavePinnedTopics();
            await FavouritesManager.SaveData();

            deferral.Complete();
        }
    }
}