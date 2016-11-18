using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.Fragments.HamburgerMenuTabs
{
    public class HamburgerMenuAnimeTab : MalFragmentBase
    {
        protected override void Init(Bundle savedInstanceState)
        {
           
        }

        protected override void InitBindings()
        {
            
        }

        public override int LayoutResourceId => Resource.Layout.HamburgerMenuAnimeTab;

        #region Views
        private LinearLayout _hamburgerMenuAnimeListButton;
        private LinearLayout _hamburgerMenuAnimeSearchButton;
        private LinearLayout _hamburgerMenuSeasonalAnimeButton;
        private LinearLayout _hamburgerMenuRecomsButton;
        private LinearLayout _hamburgerMenuTopAnimeButton;
        private LinearLayout _hamburgerMenuCalendarButton;

        public LinearLayout HamburgerMenuAnimeListButton => _hamburgerMenuAnimeListButton ?? (_hamburgerMenuAnimeListButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuAnimeListButton));

        public LinearLayout HamburgerMenuAnimeSearchButton => _hamburgerMenuAnimeSearchButton ?? (_hamburgerMenuAnimeSearchButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuAnimeSearchButton));

        public LinearLayout HamburgerMenuSeasonalAnimeButton => _hamburgerMenuSeasonalAnimeButton ?? (_hamburgerMenuSeasonalAnimeButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuSeasonalAnimeButton));

        public LinearLayout HamburgerMenuRecomsButton => _hamburgerMenuRecomsButton ?? (_hamburgerMenuRecomsButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuRecomsButton));

        public LinearLayout HamburgerMenuTopAnimeButton => _hamburgerMenuTopAnimeButton ?? (_hamburgerMenuTopAnimeButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuTopAnimeButton));

        public LinearLayout HamburgerMenuCalendarButton => _hamburgerMenuCalendarButton ?? (_hamburgerMenuCalendarButton = FindViewById<LinearLayout>(Resource.Id.HamburgerMenuCalendarButton));
        #endregion
    }
}