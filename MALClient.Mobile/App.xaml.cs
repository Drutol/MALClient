using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
            var e = args as ToastNotificationActivatedEventArgs;
            if (e != null)
            {
                var arg = await MalLinkParser.GetNavigationParametersForUrl(e.Argument);
                if (arg != null)
                    ViewModelLocator.GeneralMain.Navigate(arg.Item1, arg.Item2);
            }
        }
        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            Tuple<int, string> navArgs = null;
            if (!string.IsNullOrWhiteSpace(e.Arguments))
            {
                var options = e.Arguments.Split(';');
                if (options[0] == TileActions.OpenUrl.ToString())
                    LaunchUri(options[1]);
                else
                {
                    var detailArgs = options[1].Split('|');
                    navArgs = new Tuple<int, string>(int.Parse(detailArgs[0]), detailArgs[1]);
                }
            }
            Credentials.Init();
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
                if (e.PrelaunchActivated)
                {
                    return;
                }


                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                MainViewModel.InitDetails = navArgs;
                rootFrame.Navigate(typeof(MainPage));
            }
            else if(navArgs != null)
            {
                MobileViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(navArgs.Item1, navArgs.Item2, null, null));
            }
            // Ensure the current window is active
            NotificationTaskManager.StartNotificationTask();
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