using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Com.Shehabic.Droppy;
using GalaSoft.MvvmLight.Ioc;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using MALClient.Android.Fragments;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Interfaces;

namespace MALClient.Android.Activities
{
    [Activity(Label = "MALClient", ScreenOrientation = ScreenOrientation.Portrait,
        Icon = "@drawable/icon",WindowSoftInputMode = SoftInput.AdjustResize,MainLauncher = true,LaunchMode = LaunchMode.SingleInstance,
        Theme = "@style/Theme.Splash",ConfigurationChanges = ConfigChanges.Orientation|ConfigChanges.ScreenSize)]
    public partial class MainActivity : AppCompatActivity , IDimensionsProvider
    {
        public static Activity CurrentContext { get; private set; }
        public static int CurrentTheme { get; private set; }

        private static bool _addedNavHandlers;

        private MainViewModel _viewModel;
        private MalFragmentBase _lastPage;
        private MainViewModel ViewModel => _viewModel ?? (_viewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

        public MainActivity()
        {
            CurrentContext = this;
            SimpleIoc.Default.Register<Activity>(() => this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            RequestedOrientation = ScreenOrientation.Unspecified;
            SetTheme(Settings.SelectedTheme == 1
                ? Resource.Style.Theme_MALClient_Dark
                : Resource.Style.Theme_MALClient_Light);
            RequestWindowFeature(WindowFeatures.NoTitle);
            CurrentTheme = Settings.SelectedTheme;
            base.OnCreate(bundle);

            if (!_addedNavHandlers)
            {
                SetContentView(Resource.Layout.MainPage);
                _addedNavHandlers = true;
                InitBindings();
                ViewModel.MainNavigationRequested += ViewModelOnMainNavigationRequested;
                ViewModel.MediaElementCollapsed += ViewModelOnMediaElementCollapsed;

                ViewModelLocator.AnimeList.DimensionsProvider = this;

                var args = Intent.Extras?.GetString("launchArgs");
                ProcessLaunchArgs(args, true);
                ViewModel.PerformFirstNavigation();

                DroppyMenuPopup.RequestedElevation = DimensionsHelper.DpToPx(10);

                ResourceLocator.NotificationsTaskManager.StartTask(BgTasks.Notifications);

                StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
                StrictMode.SetThreadPolicy(policy);
            }

#if !DEBUG
            CrashManager.Register(this, "4bfd20dcd9ba4bdfbb1501397ec4a176");
            MetricsManager.Register(App.Current, "4bfd20dcd9ba4bdfbb1501397ec4a176");
#endif
        }

        private void ViewModelOnMediaElementCollapsed()
        {
            MainPageVideoView.StopPlayback();
            MainPageVideoView.SetMediaController(null);
            MainPageVideoView.SetVideoURI(null);
        }

        public override void OnBackPressed()
        {
            ViewModelLocator.NavMgr.CurrentMainViewOnBackRequested();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            RunOnUiThread(() =>
            {
                var args = intent.Extras?.GetString("launchArgs");
                ProcessLaunchArgs(args, false);
            });

        }

        private void ViewModelOnMainNavigationRequested(Fragment fragment)
        {
            _lastPage = fragment as MalFragmentBase;
            var trans = FragmentManager.BeginTransaction();
            trans.SetCustomAnimations(Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out,
                Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out);
            trans.Replace(Resource.Id.MainContentFrame, fragment);
            trans.Commit();
        }

        private void ProcessLaunchArgs(string args,bool startup)
        {

            if (!string.IsNullOrWhiteSpace(args))
            {
                Tuple<int, string> navArgs = null;
                Tuple<PageIndex, object> fullNavArgs = null;
                if (args.Contains('~')) //from notification -> mark read
                {
                    var arg = args;
                    var pos = arg.IndexOf('~');
                    if (pos != -1)
                    {
                        var id = arg.Substring(0, pos);
                        arg = arg.Substring(pos + 1);
                        MalNotificationsQuery.MarkNotifiactionsAsRead(new MalNotification(id));
                        fullNavArgs = MalLinkParser.GetNavigationParametersForUrl(arg);
                    }
                    else
                    {
                        fullNavArgs = MalLinkParser.GetNavigationParametersForUrl(arg);
                    }
                }
                else if (Regex.IsMatch(args, @"[OpenUrl,OpenDetails];.*"))
                {
                    var options = args.Split(';');
                    if (args.Contains('|')) //legacy
                    {
                        var detailArgs = options[1].Split('|');
                        navArgs = new Tuple<int, string>(int.Parse(detailArgs[0]), detailArgs[1]);
                    }
                    else
                    {
                        fullNavArgs = MalLinkParser.GetNavigationParametersForUrl(options[1]);
                    }
                }
                else
                {
                    fullNavArgs = MalLinkParser.GetNavigationParametersForUrl(args);
                }
                if (startup)
                {
                    MainViewModelBase.InitDetailsFull = fullNavArgs;
                    MainViewModelBase.InitDetails = navArgs;
                }
                else
                {
                    if (navArgs != null)
                    {
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                            new AnimeDetailsPageNavigationArgs(navArgs.Item1, navArgs.Item2, null, null));
                    }
                    if (fullNavArgs != null)
                    {
                        ViewModelLocator.GeneralMain.Navigate(fullNavArgs.Item1, fullNavArgs.Item2);
                    }
                }
            }          
        }

        protected override void OnPause()
        {
#pragma warning disable 4014
            if (Settings.IsCachingEnabled)
            {
                if (AnimeUpdateQuery.UpdatedSomething)
                {
                    DataCache.SaveDataForUser(Credentials.UserName,
                            ResourceLocator.AnimeLibraryDataStorage.AllLoadedAnimeItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Anime);
                    AnimeUpdateQuery.UpdatedSomething = false;
                }
                if (MangaUpdateQuery.UpdatedSomething)
                {
                    DataCache.SaveDataForUser(Credentials.UserName,
                            ResourceLocator.AnimeLibraryDataStorage.AllLoadedMangaItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Manga);
                    MangaUpdateQuery.UpdatedSomething = false;
                }
            }
            DataCache.SaveVolatileData();
            DataCache.SaveHumMalIdDictionary();
            ViewModelLocator.ForumsMain.SavePinnedTopics();
            FavouritesManager.SaveData();
            AnimeImageQuery.SaveData();
            ResourceLocator.HandyDataStorage.SaveData();
#pragma warning restore 4014
            base.OnPause();
        }

        public double ActualWidth => -1;
        public double ActualHeight => -1;
    }
}

