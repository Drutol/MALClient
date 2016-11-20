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
using FFImageLoading;
using FFImageLoading.Views;
using MALClient.Android.CollectionAdapters;
using MALClient.XShared.ViewModels;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.Android.BindingInformation
{
    class AnimeListItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        public AnimeListItemBindingInfo(View container, AnimeItemViewModel viewModel) : base(container, viewModel)
        {
        }

        protected override void InitBindings()
        {
            Bindings.Add(AnimeListItemWatchedButton.Id, new List<Binding>());
            Bindings[AnimeListItemWatchedButton.Id].Add(new Binding<string, string>(
                ViewModel,
                () => ViewModel.MyEpisodesBind,
                AnimeListItemWatchedButton,
                () => AnimeListItemWatchedButton.Text));

            Bindings.Add(AnimeListItemScoreButton.Id, new List<Binding>());
            Bindings[AnimeListItemScoreButton.Id].Add(new Binding<string, string>(
                ViewModel,
                () => ViewModel.MyScoreBind,
                AnimeListItemScoreButton,
                () => AnimeListItemScoreButton.Text));

            Bindings.Add(AnimeListItemStatusButton.Id, new List<Binding>());
            Bindings[AnimeListItemStatusButton.Id].Add(new Binding<string, string>(
                ViewModel,
                () => ViewModel.MyStatusBind,
                AnimeListItemStatusButton,
                () => AnimeListItemStatusButton.Text));


        }

        protected override void InitOneTimeBindings()
        {
            AnimeListItemTitle.Text = ViewModel.Title;

            ImageService.Instance.LoadUrl(ViewModel.ImgUrl).FadeAnimation(true, true).Into(AnimeListItemImage);
        }

        protected override void DetachInnerBindings()
        {
            
        }

        #region Views

        private ImageViewAsync _animeListItemImage;
        private ImageButton _animeListItemMoreButton;
        private TextView _animeListItemTitle;
        private TextView _animeGridItemToLeftInfo;
        private TextView _animeListItemTypeTextView;
        private Button _animeListItemWatchedButton;
        private LinearLayout _animeListItemBtmRightSectionTop;
        private Button _animeListItemStatusButton;
        private Button _animeListItemScoreButton;
        private LinearLayout _animeListItemStatusScoreSection;
        private ImageButton _animeListItemIncButton;
        private ImageButton _animeListItemDecButton;
        private LinearLayout _animeListItemIncDecSection;

        public ImageViewAsync AnimeListItemImage => _animeListItemImage ?? (_animeListItemImage = Container.FindViewById<ImageViewAsync>(Resource.Id.AnimeListItemImage));

        public ImageButton AnimeListItemMoreButton => _animeListItemMoreButton ?? (_animeListItemMoreButton = Container.FindViewById<ImageButton>(Resource.Id.AnimeListItemMoreButton));

        public TextView AnimeListItemTitle => _animeListItemTitle ?? (_animeListItemTitle = Container.FindViewById<TextView>(Resource.Id.AnimeListItemTitle));

        public TextView AnimeGridItemToLeftInfo => _animeGridItemToLeftInfo ?? (_animeGridItemToLeftInfo = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo));

        public TextView AnimeListItemTypeTextView => _animeListItemTypeTextView ?? (_animeListItemTypeTextView = Container.FindViewById<TextView>(Resource.Id.AnimeListItemTypeTextView));

        public Button AnimeListItemWatchedButton => _animeListItemWatchedButton ?? (_animeListItemWatchedButton = Container.FindViewById<Button>(Resource.Id.AnimeListItemWatchedButton));

        public LinearLayout AnimeListItemBtmRightSectionTop => _animeListItemBtmRightSectionTop ?? (_animeListItemBtmRightSectionTop = Container.FindViewById<LinearLayout>(Resource.Id.AnimeListItemBtmRightSectionTop));

        public Button AnimeListItemStatusButton => _animeListItemStatusButton ?? (_animeListItemStatusButton = Container.FindViewById<Button>(Resource.Id.AnimeListItemStatusButton));

        public Button AnimeListItemScoreButton => _animeListItemScoreButton ?? (_animeListItemScoreButton = Container.FindViewById<Button>(Resource.Id.AnimeListItemScoreButton));

        public LinearLayout AnimeListItemStatusScoreSection => _animeListItemStatusScoreSection ?? (_animeListItemStatusScoreSection = Container.FindViewById<LinearLayout>(Resource.Id.AnimeListItemStatusScoreSection));

        public ImageButton AnimeListItemIncButton => _animeListItemIncButton ?? (_animeListItemIncButton = Container.FindViewById<ImageButton>(Resource.Id.AnimeListItemIncButton));

        public ImageButton AnimeListItemDecButton => _animeListItemDecButton ?? (_animeListItemDecButton = Container.FindViewById<ImageButton>(Resource.Id.AnimeListItemDecButton));

        public LinearLayout AnimeListItemIncDecSection => _animeListItemIncDecSection ?? (_animeListItemIncDecSection = Container.FindViewById<LinearLayout>(Resource.Id.AnimeListItemIncDecSection));



        #endregion
    }
}