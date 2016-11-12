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
using MALClient.Android.Fragments.AnimeDetailsPageTabs;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace MALClient.Android.Adapters.PagerAdapters
{
    public class AnimeDetailsPagerAdapter : FragmentStatePagerAdapter , PagerSlidingTabStrip.ICustomTabProvider
    {
        public AnimeDetailsPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnimeDetailsPagerAdapter(FragmentManager fm) : base(fm)
        {
        }

        public override int Count => 5;

        private AnimeDetailsPageGeneralTabFragment _generalFragment;
        private AnimeDetailsPageDetailsTabFragment _detailsFragment;

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return _generalFragment ?? (_generalFragment = AnimeDetailsPageGeneralTabFragment.Instance);
                case 1:
                    return _detailsFragment ?? (_detailsFragment = AnimeDetailsPageDetailsTabFragment.Instance);
                case 2:
                    return AnimeDetailsPageGeneralTabFragment.Instance;//_generalFragment ?? (_generalFragment = AnimeDetailsPageGeneralTabFragment.Instance);
                case 3:
                    return AnimeDetailsPageGeneralTabFragment.Instance;//_generalFragment ?? (_generalFragment = AnimeDetailsPageGeneralTabFragment.Instance);
                case 4:
                    return AnimeDetailsPageGeneralTabFragment.Instance;//_generalFragment ?? (_generalFragment = AnimeDetailsPageGeneralTabFragment.Instance);
            }
            throw new Exception("Emm we've run out of fragments?");
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Tag = p1;
            switch (p1)
            {
                case 0:
                    txt.Text = "General";
                    break;
                case 1:
                    txt.Text = "Details";
                    break;
                case 2:
                    txt.Text = "Reviews";
                    break;
                case 3:
                    txt.Text = "Recoms";
                    break;
                case 4:
                    txt.Text = "Related";
                    break;
            }
            
            return txt;
        }

        public void TabSelected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = 1f;
            switch ((int)p0.Tag)
            {
                case 1:
                    ViewModelLocator.AnimeDetails.LoadDetails();
                    break;
                case 2:
                    txt.Text = "Reviews";
                    break;
                case 3:
                    txt.Text = "Recoms";
                    break;
                case 4:
                    txt.Text = "Related";
                    break;
            }
        }

        public void TabUnselected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = .7f;
        }
    }
}