using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Comm.CommUtils;
using MalClient.Shared.Comm.Manga;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.Utils.Managers;
using MalClient.Shared.ViewModels;
using MALClient.Pages;
using MALClient.Utils.Managers;
using MALClient.ViewModels;
using Microsoft.HockeyApp;
using Settings = MALClient.Utils.Settings;

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
            DesktopViewModelLocator.RegisterDependencies();
#if !DEBUG
            HockeyClient.Current.Configure("b79e78858bdf44c4bfc3a1f37c8fd90c", new TelemetryConfiguration
            {
                Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException,
            });
#endif
            Current.RequestedTheme = Settings.SelectedTheme;
            InitializeComponent();


            Suspending += OnSuspending;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
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
                rootFrame.Navigate(typeof(MainPage), navArgs);
            }
            else if (navArgs != null)
            {
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(navArgs.Item1, navArgs.Item2, null, null));
            }
            // Ensure the current window is active

            HtmlClassMgr.Init();
            LiveTilesManager.LoadTileCache();
            FavouritesManager.LoadData();
            Window.Current.Activate();
            RateReminderPopUp.ProcessRatePopUp();
            RateReminderPopUp.ProcessDonatePopUp();
            var tb = ApplicationView.GetForCurrentView().TitleBar;
            tb.BackgroundColor =
                tb.ButtonBackgroundColor =
                    tb.InactiveBackgroundColor =
                        tb.ButtonInactiveBackgroundColor =
                            Settings.SelectedTheme == ApplicationTheme.Dark
                                ? Color.FromArgb(255, 41, 41, 41)
                                : Colors.White;
            ProcessUpdate();
        }

        private void ProcessUpdate()
        {
            //if (ApplicationData.Current.LocalSettings.Values["AppVersion"] == null
            //    || (string) ApplicationData.Current.LocalSettings.Values["AppVersion"] != Utilities.GetAppVersion())
            //    await Task.Run(async () =>
            //    {

            //    });
            ApplicationData.Current.LocalSettings.Values["AppVersion"] = Utilities.GetAppVersion();
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
                Utilities.GiveStatusBarFeedback("Invalid target url...");
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
                            ViewModelLocator.GeneralAnimeList.AllLoadedAnimeItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Anime);
                if (MangaUpdateQuery.UpdatedSomething)
                    await
                        DataCache.SaveDataForUser(Credentials.UserName,
                            ViewModelLocator.GeneralAnimeList.AllLoadedMangaItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Manga);
            }
            await DataCache.SaveVolatileData();
            await DataCache.SaveHumMalIdDictionary();
            try
            {
                foreach (
                    var file in
                        await ApplicationData.Current.TemporaryFolder.GetFilesAsync(CommonFileQuery.DefaultQuery))
                    if (file.Name.Contains("_cropTemp"))
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //well...
            }
            await FavouritesManager.SaveData();
            deferral.Complete();
        }
    }
}