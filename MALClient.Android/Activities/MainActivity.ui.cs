using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;

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

        public NavigationView MainNavView => _mainNavView ?? (_mainNavView = FindViewById<NavigationView>(Resource.Id.MainNavView));

    }
}