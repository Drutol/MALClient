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
using MALClient.Android.Resources;

namespace MALClient.Android.Fragments
{
    public partial class AnimeDetailsPageFragment
    {
        private ImageViewAsync _animeDetailsPageShowCoverImage;
        private TextView _animeDetailsPageWatchedLabel;
        private TextView _animeDetailsPageReadVolumesLabel;
        private Button _animeDetailsPageScoreButton;
        private Button _animeDetailsPageStatusButton;
        private Button _animeDetailsPageWatchedButton;
        private Button _animeDetailsPageReadVolumesButton;
        private LinearLayout _animeDetailsPageUpdateSection;
        private FrameLayout _animeDetailsPageIncrementButton;
        private FrameLayout _animeDetailsPageDecrementButton;
        private ImageButton _animeDetailsPageFavouriteButton;
        private ImageButton _animeDetailsPageMoreButton;
        private RelativeLayout _animeDetailsPageIncDecSection;
        private FrameLayout _animeDetailsPageAddButton;
        private LinearLayout _animeDetailsPageAddSection;
        private PagerSlidingTabStrip _animeDetailsPageTabStrip;
        private ViewPager _animeDetailsPagePivot;
        private RelativeLayout _animeDetailsPageLoadingOverlay;

        public ImageViewAsync AnimeDetailsPageShowCoverImage => _animeDetailsPageShowCoverImage ?? (_animeDetailsPageShowCoverImage = FindViewById<ImageViewAsync>(Resource.Id.AnimeDetailsPageShowCoverImage));

        public TextView AnimeDetailsPageWatchedLabel => _animeDetailsPageWatchedLabel ?? (_animeDetailsPageWatchedLabel = FindViewById<TextView>(Resource.Id.AnimeDetailsPageWatchedLabel));

        public TextView AnimeDetailsPageReadVolumesLabel => _animeDetailsPageReadVolumesLabel ?? (_animeDetailsPageReadVolumesLabel = FindViewById<TextView>(Resource.Id.AnimeDetailsPageReadVolumesLabel));

        public Button AnimeDetailsPageScoreButton => _animeDetailsPageScoreButton ?? (_animeDetailsPageScoreButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageScoreButton));

        public Button AnimeDetailsPageStatusButton => _animeDetailsPageStatusButton ?? (_animeDetailsPageStatusButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageStatusButton));

        public Button AnimeDetailsPageWatchedButton => _animeDetailsPageWatchedButton ?? (_animeDetailsPageWatchedButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageWatchedButton));

        public Button AnimeDetailsPageReadVolumesButton => _animeDetailsPageReadVolumesButton ?? (_animeDetailsPageReadVolumesButton = FindViewById<Button>(Resource.Id.AnimeDetailsPageReadVolumesButton));

        public LinearLayout AnimeDetailsPageUpdateSection => _animeDetailsPageUpdateSection ?? (_animeDetailsPageUpdateSection = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageUpdateSection));

        public FrameLayout AnimeDetailsPageIncrementButton => _animeDetailsPageIncrementButton ?? (_animeDetailsPageIncrementButton = FindViewById<FrameLayout>(Resource.Id.AnimeDetailsPageIncrementButton));

        public FrameLayout AnimeDetailsPageDecrementButton => _animeDetailsPageDecrementButton ?? (_animeDetailsPageDecrementButton = FindViewById<FrameLayout>(Resource.Id.AnimeDetailsPageDecrementButton));

        public ImageButton AnimeDetailsPageFavouriteButton => _animeDetailsPageFavouriteButton ?? (_animeDetailsPageFavouriteButton = FindViewById<ImageButton>(Resource.Id.AnimeDetailsPageFavouriteButton));

        public ImageButton AnimeDetailsPageMoreButton => _animeDetailsPageMoreButton ?? (_animeDetailsPageMoreButton = FindViewById<ImageButton>(Resource.Id.AnimeDetailsPageMoreButton));

        public RelativeLayout AnimeDetailsPageIncDecSection => _animeDetailsPageIncDecSection ?? (_animeDetailsPageIncDecSection = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageIncDecSection));

        public FrameLayout AnimeDetailsPageAddButton => _animeDetailsPageAddButton ?? (_animeDetailsPageAddButton = FindViewById<FrameLayout>(Resource.Id.AnimeDetailsPageAddButton));

        public LinearLayout AnimeDetailsPageAddSection => _animeDetailsPageAddSection ?? (_animeDetailsPageAddSection = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageAddSection));

        public PagerSlidingTabStrip AnimeDetailsPageTabStrip => _animeDetailsPageTabStrip ?? (_animeDetailsPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.AnimeDetailsPageTabStrip));

        public ViewPager AnimeDetailsPagePivot => _animeDetailsPagePivot ?? (_animeDetailsPagePivot = FindViewById<ViewPager>(Resource.Id.AnimeDetailsPagePivot));

        public RelativeLayout AnimeDetailsPageLoadingOverlay => _animeDetailsPageLoadingOverlay ?? (_animeDetailsPageLoadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageLoadingOverlay));



    }
}