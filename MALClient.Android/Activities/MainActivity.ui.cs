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

namespace MALClient.Android.Activities
{
    public partial class MainActivity
    {
        private Toolbar _toolbar;
        private FrameLayout _contentFrame;
        private NavigationView _navView;
        private DrawerLayout _drawerLayout;

        public Toolbar Toolbar => _toolbar ?? (_toolbar = FindViewById<Toolbar>(Resource.Id.MainToolbar));

        public FrameLayout ContentFrame => _contentFrame ?? (_contentFrame = FindViewById<FrameLayout>(Resource.Id.MainContentFrame));

        public NavigationView NavView => _navView ?? (_navView = FindViewById<NavigationView>(Resource.Id.MainNavView));

        public DrawerLayout DrawerLayout => _drawerLayout ?? (_drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout));

    }
}