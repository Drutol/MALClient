using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Daimajia.Swipe;
using Com.Daimajia.Swipe.Implments;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Android.ViewModels;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Interfaces;
using Org.Zakariya.Flyoutmenu;
using Fragment = Android.Support.V4.App.Fragment;

namespace MALClient.Android.Activities
{
    [Activity(Label = "MALClient", MainLauncher = true, 
        Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public partial class MainActivity : AppCompatActivity , IDimensionsProvider
    {
        public static Activity CurrentContext { get; private set; }

        private static bool _addedNavHandlers;

        private MainViewModel _viewModel;
        private MainViewModel ViewModel => _viewModel ?? (_viewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

        public MainActivity()
        {
            CurrentContext = this;
        }

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            
            if (!_addedNavHandlers)
            {
                SetContentView(Resource.Layout.MainPage);
                _addedNavHandlers = true;
                InitBindings();
                ViewModel.MainNavigationRequested += ViewModelOnMainNavigationRequested;
                MainNavView.NavigationItemSelected += NavViewOnNavigationItemSelected;


                ViewModel.Navigate(PageIndex.PageLogIn);
            }
    
        }

        private void ViewModelOnMainNavigationRequested(Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.MainContentFrame, fragment)
                .Commit();
        }

        private void NavViewOnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            e.MenuItem.SetChecked(true);
            ViewModelLocator.NavMgr.ResetMainBackNav();
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.MainHamburgerBtnLogIn:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    break;
                case Resource.Id.MainHamburgerBtnAnimeList:
                    ViewModel.Navigate(PageIndex.PageAnimeList,null);
                    break;
            }

            DrawerLayout.CloseDrawers();
        }

        public double ActualWidth => 800;
        public double ActualHeight => 1200;
    }
}

