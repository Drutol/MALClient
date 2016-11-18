using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Activities
{
    public partial class MainActivity
    {

        private Dictionary<int, Binding> _bindings;
        private void InitBindings()
        {
            _bindings = new Dictionary<int, Binding>
            {
                {Resource.Id.MainPageCurrentStatus, this.SetBinding(() => ViewModel.CurrentStatus,() => MainPageCurrentStatus.Text)}
            };
            MainPageHamburgerButton.Click +=  MainPageHamburgerButtonOnClick;
            DrawerLayout.DrawerOpened += DrawerLayoutOnDrawerOpened;
            
        }

        private void DrawerLayoutOnDrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs drawerOpenedEventArgs)
        {
            DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedOpen);
            DrawerLayout.SetOnClickListener(new OnClickListener(view => DrawerLayout.CloseDrawer((int)GravityFlags.Start)));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => DrawerLayout.CloseDrawer((int)GravityFlags.Start))); //TODO Priority override
        }

        private void MainPageHamburgerButtonOnClick(object sender, EventArgs eventArgs)
        {
            DrawerLayout.OpenDrawer((int)GravityFlags.Start);
        }


        private DrawerLayout _drawerLayout;
        public DrawerLayout DrawerLayout => _drawerLayout ?? (_drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout));

        private TextView _mainPageCurrentStatus;
        private FrameLayout _mainContentFrame;
        private NavigationView _mainNavView;
        private ImageButton _mainPageHamburgerButton;

        public ImageButton MainPageHamburgerButton => _mainPageHamburgerButton ?? (_mainPageHamburgerButton = FindViewById<ImageButton>(Resource.Id.MainPageHamburgerButton));

        public TextView MainPageCurrentStatus => _mainPageCurrentStatus ?? (_mainPageCurrentStatus = FindViewById<TextView>(Resource.Id.MainPageCurrentStatus));

        public FrameLayout MainContentFrame => _mainContentFrame ?? (_mainContentFrame = FindViewById<FrameLayout>(Resource.Id.MainContentFrame));

        // public NavigationView MainNavView => _mainNavView ?? (_mainNavView = FindViewById<NavigationView>(Resource.Id.MainNavView));


        #region Hamburger
        private PagerSlidingTabStrip _hamburgerMenuTabStrip;
        private ViewPager _hamburgerMenuPivot;
        private LinearLayout _hamburgerMenuAccountButton;
        private LinearLayout _hamburgerMenuSettingsButton;

        public PagerSlidingTabStrip HamburgerMenuTabStrip => _hamburgerMenuTabStrip ?? (_hamburgerMenuTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.HamburgerMenuTabStrip));

        public ViewPager HamburgerMenuPivot => _hamburgerMenuPivot ?? (_hamburgerMenuPivot = FindViewById<ViewPager>(Resource.Id.HamburgerMenuPivot));

        public LinearLayout HamburgerMenuAccountButton => _hamburgerMenuAccountButton ?? (_hamburgerMenuAccountButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuAccountButton));

        public LinearLayout HamburgerMenuSettingsButton => _hamburgerMenuSettingsButton ?? (_hamburgerMenuSettingsButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuSettingsButton));
        #endregion
    }
}