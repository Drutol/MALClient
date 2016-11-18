using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments.HamburgerMenuTabs;
using MALClient.Android.Resources;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace MALClient.Android.Adapters.PagerAdapters
{
    public class HamburgerMenuPagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {
        public HamburgerMenuPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public HamburgerMenuPagerAdapter(FragmentManager fm) : base(fm)
        {
        }

        public override int Count => 5;

        private HamburgerMenuAnimeTab _hamburgerMenuAnimeTab = new HamburgerMenuAnimeTab();
        private HamburgerMenuMangaTab _hamburgerMenuMangaTab = new HamburgerMenuMangaTab();
        private HamburgerMenuMoreTab _hamburgerMenuMoreTab = new HamburgerMenuMoreTab();

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new Fragment();
                case 1:
                    return _hamburgerMenuAnimeTab;
                case 2:
                    return _hamburgerMenuMangaTab;
                case 3:
                    return _hamburgerMenuMoreTab;
                case 4:
                    return new Fragment();

            }
            throw new Exception("We have run out of fragments!");
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Tag = p1;
            switch (p1)
            {
                case 0:
                    txt.LayoutParameters = new ViewGroup.LayoutParams(0, 0);
                    break;
                case 1:
                    txt.Text = "Anime";
                    break;
                case 2:
                    txt.Text = "Manga";
                    break;
                case 3:
                    txt.Text = "More";
                    break;
                case 4:
                    txt.LayoutParameters = new ViewGroup.LayoutParams(0,0);
                    break;
            }

            return txt;
        }

        public void TabSelected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = 1f;        
        }

        public void TabUnselected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = .7f;
        }
    }
}