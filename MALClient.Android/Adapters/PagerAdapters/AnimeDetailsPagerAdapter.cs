using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using com.refractored;
using MALClient.Android.Fragments.AnimeDetailsPageTabs;
using MALClient.Android.Resources;

using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.PagerAdapters
{
    public class AnimeDetailsPagerAdapter : FragmentStatePagerAdapter , ICustomTabProvider
    {
        private AnimeDetailsPageViewModel _viewModel;

        public AnimeDetailsPagerAdapter(FragmentManager fm) : base(fm)
        {
            _viewModel = ViewModelLocator.AnimeDetails;
        }

        public override int Count
        {
            get
            {
                var newCount = _viewModel.AnimeMode ? 7 : 5;
                return newCount;
            }
        }

        private AnimeDetailsPageGeneralTabFragment _generalFragment;
        private AnimeDetailsPageDetailsTabFragment _detailsFragment;
        private AnimeDetailsPageReviewsTabFragment _reviewsFragment;
        private AnimeDetailsPageRecomsTabFragment _recomsFragment;
        private AnimeDetailsPageRelatedTabFragment _relatedFragment;
        private AnimeDetailsPageCharactersTabFragment _charactersFragment;
        private AnimeDetailsPageStaffTabFragment _staffFragment;

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
                case 5:
                    return _charactersFragment ?? (_charactersFragment = new AnimeDetailsPageCharactersTabFragment());
                case 6:
                    return _staffFragment ?? (_staffFragment = new AnimeDetailsPageStaffTabFragment());
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
                case 5:
                    txt.Text = "Characters";
                    break;
                case 6:
                    txt.Text = "Staff";
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
                    _viewModel.LoadDetails();
                    break;
                case 2:
                    _viewModel.LoadReviews();
                    break;
                case 3:
                    _viewModel.LoadRecommendations();
                    break;
                case 4:
                    _viewModel.LoadRelatedAnime();
                    break;
                case 5:
                case 6:
                    _viewModel.LoadCharacters();
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