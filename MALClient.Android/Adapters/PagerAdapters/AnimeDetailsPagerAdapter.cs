using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments.AnimeDetailsPageTabs;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;
using Fragment = Android.App.Fragment;
using FragmentManager = Android.App.FragmentManager;

namespace MALClient.Android.PagerAdapters
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
        private AnimeDetailsPageReviewsTabFragment _reviewsFragment;
        private AnimeDetailsPageRecomsTabFragment _recomsFragment;
        private AnimeDetailsPageRelatedTabFragment _relatedFragment;

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return _generalFragment ?? (_generalFragment = AnimeDetailsPageGeneralTabFragment.Instance);
                case 1:
                    return _detailsFragment ?? (_detailsFragment = AnimeDetailsPageDetailsTabFragment.Instance);
                case 2:
                    return _reviewsFragment ?? (_reviewsFragment = AnimeDetailsPageReviewsTabFragment.Instance);
                case 3:
                    return _recomsFragment ?? (_recomsFragment = AnimeDetailsPageRecomsTabFragment.Instance);
                case 4:
                    return _relatedFragment ?? (_relatedFragment = AnimeDetailsPageRelatedTabFragment.Instance);
            }
            throw new Exception("Emm we've run out of fragments?");
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Tag = p1;
            txt.SetPadding(0,(int)DimensionsHelper.PxToDp(8),0,0);
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
                    ViewModelLocator.AnimeDetails.LoadReviews();
                    break;
                case 3:
                    ViewModelLocator.AnimeDetails.LoadRecommendations();
                    break;
                case 4:
                    ViewModelLocator.AnimeDetails.LoadRelatedAnime();
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