using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.ApplicationInsights;
using UniversalRateReminder;

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
            Application.Current.RequestedTheme = Settings.SelectedTheme;
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
            var rootFrame = Window.Current.Content as Frame;

            if (!string.IsNullOrWhiteSpace(e.Arguments))
            {
                LaunchUri(e.Arguments);
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
            Window.Current.Activate();
            await ProcessPopUp();
            ProcessStatusBar();
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

        private async Task ProcessPopUp()
        {
            RatePopup.Title = "Rate this app!";
            RatePopup.CancelButtonText = "Not now...";
            RatePopup.Content =
                "Your feedback helps improve this app!\n\nPlease take a minute to review this application , if you want to fill in bug report check out the about page. :) ";
            RatePopup.ResetCountOnNewVersion = false;
            RatePopup.RateButtonText = "To the store!";
            await RatePopup.CheckRateReminderAsync();
        }

        private void ProcessStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Application.Current.RequestedTheme == ApplicationTheme.Dark
                        ? Colors.Black
                        : Colors.White;
                    statusBar.ForegroundColor = Application.Current.RequestedTheme == ApplicationTheme.Dark
                        ? Colors.White
                        : Colors.Black;
                }
            }
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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}