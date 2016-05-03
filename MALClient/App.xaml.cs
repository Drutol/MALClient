using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Pages;
using MALClient.ViewModels;
using Microsoft.ApplicationInsights;

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
            WindowsAppInitializer.InitializeAsync(
                WindowsCollectors.Metadata |
                WindowsCollectors.Session);
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

            if (!string.IsNullOrWhiteSpace(e.Arguments))
            {
                LaunchUri(e.Arguments);
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
                rootFrame.Navigate(typeof (MainPage), e.Arguments);
            }
            // Ensure the current window is active

            try
            {
                HtmlClassMgr.Init();
            }
            catch (Exception)
            {
                // no internet?
            }
            
            Window.Current.Activate();
            RateReminderPopUp.ProcessRatePopUp();
            var tb = ApplicationView.GetForCurrentView().TitleBar;
            tb.BackgroundColor = tb.ButtonBackgroundColor =tb.InactiveBackgroundColor = tb.ButtonInactiveBackgroundColor = Settings.SelectedTheme == ApplicationTheme.Dark ? Color.FromArgb(255,41,41,41) : Colors.White;
            ProcessUpdate();
        }

        private async void ProcessUpdate()
        {
            if (ApplicationData.Current.LocalSettings.Values["AppVersion"] == null
                || (string) ApplicationData.Current.LocalSettings.Values["AppVersion"] != Utils.GetAppVersion())
                await Task.Run(async () =>
                {
                    try
                    {
                        var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails");
                        foreach (var file in await folder.GetFilesAsync())
                        {
                            if (file.Name.Contains("related") || file.Name.Contains("direct_recommendations"))
                                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                    }
                    catch (Exception)
                    {
                        //
                    }
                });
            ApplicationData.Current.LocalSettings.Values["AppVersion"] = Utils.GetAppVersion();
        }

        private async void LaunchUri(string url)
        {
            await Launcher.LaunchUriAsync(new Uri(url));
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
                    await DataCache.SaveDataForUser(Credentials.UserName, ViewModelLocator.AnimeList.AllLoadedAnimeItemAbstractions.Select(abstraction => abstraction.EntryData), AnimeListWorkModes.Anime);
                if(MangaUpdateQuery.UpdatedSomething)
                    await DataCache.SaveDataForUser(Credentials.UserName, ViewModelLocator.AnimeList.AllLoadedMangaItemAbstractions.Select(abstraction => abstraction.EntryData), AnimeListWorkModes.Manga);
            }
            await DataCache.SaveVolatileData();
            await DataCache.SaveHumMalIdDictionary();

            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}