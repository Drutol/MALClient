using System;
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
using GalaSoft.MvvmLight.Ioc;
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Android.Fragments;
using MALClient.Android.ViewModels;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
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

        private static bool _addedNavHandlers;

        private MainViewModel _viewModel;
        private MalFragmentBase _lastPage;
        private MainViewModel ViewModel => _viewModel ?? (_viewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

        public MainActivity()
        {
            CurrentContext = this;
        }

        protected override void OnCreate(Bundle bundle)
        {
            Settings.SelectedTheme = 1;
            SetTheme(Resource.Style.Theme_MALClient_Dark);
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            
            if (!_addedNavHandlers)
            {
                SetContentView(Resource.Layout.MainPage);
                _addedNavHandlers = true;
                InitBindings();
                ViewModel.MainNavigationRequested += ViewModelOnMainNavigationRequested;

                ViewModelLocator.AnimeList.DimensionsProvider = this;
                ViewModel.Navigate(PageIndex.PageAnimeList);
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

        //private void NavViewOnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        //{
        //    e.MenuItem.SetChecked(true);
        //    ViewModelLocator.NavMgr.ResetMainBackNav();
        //    switch (e.MenuItem.ItemId)
        //    {
        //        case Resource.Id.MainHamburgerBtnLogIn:
        //            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
        //            break;
        //        case Resource.Id.MainHamburgerBtnAnimeList:
        //            ViewModel.Navigate(PageIndex.PageAnimeList,null);
        //            break;
        //    }

        //    DrawerLayout.CloseDrawers();
        //}

        public double ActualWidth => -1;
        public double ActualHeight => -1;
    }
}

