using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MALCLient.UrlInterceptor
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            OnLaunchedOrActivated(args);
            base.OnActivated(args);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if(args.PrelaunchActivated)
                return;

            OnLaunchedOrActivated(args);

            base.OnLaunched(args);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async void OnLaunchedOrActivated(IActivatedEventArgs e)
        {

            var arg = e as ProtocolActivatedEventArgs;
            if (arg != null)
            {
                var uri = arg.Uri.ToString();
                uri = uri.Replace("http://","https://");
                if (Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?subboard=\d+")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?board=\d+")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?animeid=\d+")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?mangaid=\d+")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist\.net\/forum\/message\/\d+\?goto=topic")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?topicid=\d+")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/comtocom.php\?id1=\d+&id2=\d+\|.*")
                    || Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/mymessages.php\?go=read&id=\d+\|.*")
                    || Regex.IsMatch(uri, "anime\\/\\d") || Regex.IsMatch(uri, "manga\\/\\d") 
                    || uri == "https://myanimelist.net/news"
                    || uri == "https://myanimelist.net/featured")
                   await Launcher.LaunchUriAsync(new Uri($"malclient://{arg.Uri}"));
                else
                   await Launcher.LaunchUriAsync(arg.Uri,new LauncherOptions() {IgnoreAppUriHandlers = true});
                if (Window.Current.Content == null)
                    App.Current.Exit();
            }
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }


                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage));
                }
                // Ensure the current window is active
                Window.Current.Activate();
            
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
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
