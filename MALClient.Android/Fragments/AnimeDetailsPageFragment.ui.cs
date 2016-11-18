using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using FFImageLoading.Views;

namespace MALClient.Android.Fragments
{
    public partial class AnimeDetailsPageFragment
    {
        private RelativeLayout _animeDetailsPageLoadingOverlay;
        public RelativeLayout AnimeDetailsPageLoadingOverlay => _animeDetailsPageLoadingOverlay ?? (_animeDetailsPageLoadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageLoadingOverlay));

        private ImageViewAsync _animeDetailsPageShowCoverImage;
        private TextView _animeDetailsPageWatchedLabel;
        private TextView _animeDetailsPageReadVolumesLabel;
        private Button _animeDetailsPageScoreButton;
        private Button _animeDetailsPageStatusButton;
        private Button _animeDetailsPageWatchedButton;
        private Button _animeDetailsPageReadVolumesButton;
        private ImageButton _animeDetailsPageIncrementButton;
        private ImageButton _animeDetailsPageDecrementButton;
        private ImageButton _animeDetailsPageFavouriteButton;
        private ImageButton _animeDetailsPageMoreButton;
        private PagerSlidingTabStrip _animeDetailsPageTabStrip;
        private ViewPager _animeDetailsPagePivot;

        public ImageViewAsync AnimeDetailsPageShowCoverImage => _animeDetailsPageShowCoverImage ?? (_animeDetailsPageShowCoverImage = FindViewById<ImageViewAsync>(Resource.Id.AnimeDetailsPageShowCoverImage));

        public TextView AnimeDetailsPageWatchedLabel => _animeDetailsPageWatchedLabel ?? (_animeDetailsPageWatchedLabel = FindViewById<TextView>(Resource.Id.AnimeDetailsPageWatchedLabel));

        public TextView AnimeDetailsPageReadVolumesLabel => _animeDetailsPageReadVolumesLabel ?? (_animeDetailsPageReadVolumesLabel = FindViewById<TextView>(Resource.Id.AnimeDetailsPageReadVolumesLabel));

        public Button AnimeDetailsPageScoreButton => _animeDetailsPageScoreButton ?? (_animeDetailsPageScoreButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageScoreButton));

        public Button AnimeDetailsPageStatusButton => _animeDetailsPageStatusButton ?? (_animeDetailsPageStatusButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageStatusButton));

        public Button AnimeDetailsPageWatchedButton => _animeDetailsPageWatchedButton ?? (_animeDetailsPageWatchedButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageWatchedButton));

        public Button AnimeDetailsPageReadVolumesButton => _animeDetailsPageReadVolumesButton ?? (_animeDetailsPageReadVolumesButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageReadVolumesButton));

        public ImageButton AnimeDetailsPageIncrementButton => _animeDetailsPageIncrementButton ?? (_animeDetailsPageIncrementButton = FindViewById<ImageButton>(Resource.Id.AnimeDetailsPageIncrementButton));

        public ImageButton AnimeDetailsPageDecrementButton => _animeDetailsPageDecrementButton ?? (_animeDetailsPageDecrementButton = FindViewById<ImageButton>(Resource.Id.AnimeDetailsPageDecrementButton));

        public ImageButton AnimeDetailsPageFavouriteButton => _animeDetailsPageFavouriteButton ?? (_animeDetailsPageFavouriteButton = FindViewById<ImageButton>(Resource.Id.AnimeDetailsPageFavouriteButton));

        public ImageButton AnimeDetailsPageMoreButton => _animeDetailsPageMoreButton ?? (_animeDetailsPageMoreButton = FindViewById<ImageButton>(Resource.Id.AnimeDetailsPageMoreButton));

        public PagerSlidingTabStrip AnimeDetailsPageTabStrip => _animeDetailsPageTabStrip ?? (_animeDetailsPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.AnimeDetailsPageTabStrip));

        public ViewPager AnimeDetailsPagePivot => _animeDetailsPagePivot ?? (_animeDetailsPagePivot = FindViewById<ViewPager>(Resource.Id.AnimeDetailsPagePivot));


    }
}