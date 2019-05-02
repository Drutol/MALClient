﻿using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.refractored;
using Com.Mikepenz.Materialdrawer;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.Clubs;
using MALClient.Android.Resources;

using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.ViewModels;


namespace MALClient.Android.PagerAdapters
{
    public class ClubsIndexPagerAdapter : FragmentStatePagerAdapter , ICustomTabProvider
    {
        public ClubsIndexPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ClubsIndexPagerAdapter(FragmentManager fm,Drawer drawer) : base(fm)
        {
            _myClubsFragment = new ClubIndexMyClubsTabFragment();
            _allClubsFragment = new ClubIndexAllClubsTabFragment(drawer);
        }

        private readonly MalFragmentBase _myClubsFragment;
        private readonly MalFragmentBase _allClubsFragment;

        public override int Count => 2;

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _myClubsFragment;
                case 1:
                    return _allClubsFragment;
            }
            throw new ArgumentException();
        }

        public void TabSelected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = 1f;
            switch ((int)p0.Tag)
            {
                case 0:
                    ViewModelLocator.ClubIndex.QueryType = MalClubQueries.QueryType.My;
                    break;
                case 1:
                    ViewModelLocator.ClubIndex.QueryType = MalClubQueries.QueryType.All;
                    break;
            }
        }

        public void TabUnselected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = .7f;
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.SetTextSize(ComplexUnitType.Sp, 18);
            txt.LayoutParameters = new LinearLayout.LayoutParams(-2,-2) {Gravity = GravityFlags.CenterVertical};
            txt.SetPadding(0, DimensionsHelper.DpToPx(4), 0, 0);
            txt.Tag = p1;
            switch (p1)
            {
                case 0:
                    txt.Text = "My Clubs";
                    break;
                case 1:
                    txt.Text = "All Clubs";
                    break;
            }

            return txt;
        }
    }
}