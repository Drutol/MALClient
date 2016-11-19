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
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Holder;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Activities
{
    public partial class MainActivity
    {
        private Drawer _drawer;
        private Dictionary<int, Binding> _bindings;

        private void InitBindings()
        {
            _bindings = new Dictionary<int, Binding>
            {
                {Resource.Id.MainPageCurrentStatus, this.SetBinding(() => ViewModel.CurrentStatus,() => MainPageCurrentStatus.Text)}
            };
            MainPageHamburgerButton.Click +=  MainPageHamburgerButtonOnClick;      
            BuildDrawer();     
        }


        private void MainPageHamburgerButtonOnClick(object sender, EventArgs eventArgs)
        {
            _drawer.OpenDrawer();
        }

        private PrimaryDrawerItem GetBasePrimaryItem()
        {
            var btn = new PrimaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(Resource.Color.BrushText);
            btn.WithIconColorRes(Resource.Color.BrushText);
            btn.WithSelectedColorRes(Resource.Color.BrushHamburgerInnerBackground);
            btn.WithSelectedTextColorRes(Resource.Color.AccentColour);
            btn.WithSelectedIconColorRes(Resource.Color.AccentColourDark);
            return btn;
        }

        private void BuildDrawer()
        {
            var builder = new DrawerBuilder().WithActivity(this);
            builder.WithSliderBackgroundColorRes(Resource.Color.BrushHamburgerBackground);
            var animeButton = GetBasePrimaryItem();
            animeButton.WithName("Anime list");
            animeButton.WithIcon(Resource.Drawable.icon_list);
            var searchButton = GetBasePrimaryItem();
            searchButton.WithName("Anime search");
            searchButton.WithIcon(Resource.Drawable.icon_search);
            var seasonalButton = GetBasePrimaryItem();
            seasonalButton.WithName("Seasonal anime");
            seasonalButton.WithIcon(Resource.Drawable.icon_seasonal);
            var recomButton = GetBasePrimaryItem();
            recomButton.WithName("Recommendations");
            recomButton.WithIcon(Resource.Drawable.icon_recom);
            var topAnimeButton = GetBasePrimaryItem();
            topAnimeButton.WithName("Top anime");
            topAnimeButton.WithIcon(Resource.Drawable.icon_fav_outline);
            var calendarButton = GetBasePrimaryItem();
            calendarButton.WithName("Calendar");
            calendarButton.WithIcon(Resource.Drawable.icon_calendar);

            //var animeSection = new ContainerDrawerItem();
            //animeSection.WithSubItems(animeButton);
            builder.WithDrawerItems(new List<IDrawerItem>() { animeButton,searchButton,seasonalButton,recomButton,topAnimeButton,calendarButton});
            _drawer = builder.Build();
         
        }

        //private DrawerLayout _drawerLayout;
        //public DrawerLayout DrawerLayout => _drawerLayout ?? (_drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout));

        private TextView _mainPageCurrentStatus;
        private FrameLayout _mainContentFrame;
        //private NavigationView _mainNavView;
        private ImageButton _mainPageHamburgerButton;

        public ImageButton MainPageHamburgerButton => _mainPageHamburgerButton ?? (_mainPageHamburgerButton = FindViewById<ImageButton>(Resource.Id.MainPageHamburgerButton));

        public TextView MainPageCurrentStatus => _mainPageCurrentStatus ?? (_mainPageCurrentStatus = FindViewById<TextView>(Resource.Id.MainPageCurrentStatus));

        public FrameLayout MainContentFrame => _mainContentFrame ?? (_mainContentFrame = FindViewById<FrameLayout>(Resource.Id.MainContentFrame));

        // public NavigationView MainNavView => _mainNavView ?? (_mainNavView = FindViewById<NavigationView>(Resource.Id.MainNavView));


    }
}