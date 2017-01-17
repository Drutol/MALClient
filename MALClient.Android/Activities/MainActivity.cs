using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Daimajia.Swipe;
using Com.Daimajia.Swipe.Implments;
using Com.Mikepenz.Materialdrawer;
using Com.Shehabic.Droppy;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Android.Fragments;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Interfaces;

namespace MALClient.Android.Activities
{
    [Activity(Label = "MALClient", MainLauncher = true, 
        Icon = "@drawable/icon", /*ScreenOrientation = ScreenOrientation.Portrait,*/
        Theme = "@style/Theme.AppCompat.NoActionBar",ConfigurationChanges = ConfigChanges.Orientation|ConfigChanges.ScreenSize)]
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

                ViewModelLocator.AnimeList.DimensionsProvider = this;

                ViewModel.Navigate(Credentials.Authenticated
                    ? (Settings.DefaultMenuTab == "anime" ? PageIndex.PageAnimeList : PageIndex.PageMangaList)
                    : PageIndex.PageLogIn);

                DroppyMenuPopup.RequestedElevation = DimensionsHelper.DpToPx(10);
            }
    
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }

        public override void OnBackPressed()
        {
            ViewModelLocator.NavMgr.CurrentMainViewOnBackRequested();
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

        protected override void OnPause()
        {
#pragma warning disable 4014
            if (Settings.IsCachingEnabled)
            {
                if (AnimeUpdateQuery.UpdatedSomething)                  
                        DataCache.SaveDataForUser(Credentials.UserName,
                            ResourceLocator.AnimeLibraryDataStorage.AllLoadedAnimeItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Anime);
                if (MangaUpdateQuery.UpdatedSomething)                
                        DataCache.SaveDataForUser(Credentials.UserName,
                            ResourceLocator.AnimeLibraryDataStorage.AllLoadedMangaItemAbstractions.Select(
                                abstraction => abstraction.EntryData), AnimeListWorkModes.Manga);
            }
            DataCache.SaveVolatileData();
            DataCache.SaveHumMalIdDictionary();
            ViewModelLocator.ForumsMain.SavePinnedTopics();
            FavouritesManager.SaveData();
#pragma warning restore 4014
            base.OnPause();
        }

        public double ActualWidth => -1;
        public double ActualHeight => -1;
    }
}

