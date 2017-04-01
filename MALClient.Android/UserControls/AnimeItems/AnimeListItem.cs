using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.UserControls
{
    public class AnimeListItem : UserControlBase<AnimeItemViewModel,FrameLayout>
    {
        private readonly Action<AnimeItemViewModel> _onItemClickAction;
        private DroppyMenuPopup _menu;

        #region Constructors

        public AnimeListItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnimeListItem(Context context, Action<AnimeItemViewModel> onItemClickAction) : base(context)
        {
            _onItemClickAction = onItemClickAction;
        }

        public AnimeListItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public AnimeListItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public AnimeListItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        protected override int ResourceId => Resource.Layout.AnimeListItem;

        protected override void BindModelFling()
        {
            AnimeListItemImage.Visibility = ViewStates.Invisible;
            AnimeListItemImgPlaceholder.Visibility = ViewStates.Visible;
        }

        protected override void BindModelFull()
        {
            if ((string)AnimeListItemImage.Tag != ViewModel.ImgUrl)
            {
                AnimeListItemImage.AnimeInto(ViewModel.ImgUrl);
                AnimeListItemImage.Tag = ViewModel.ImgUrl;
            }
            else
            {
                AnimeListItemImage.Visibility = ViewStates.Visible;
            }


            AnimeListItemMoreButton.SetOnClickListener(new OnClickListener(view => MoreButtonOnClick()));
            AnimeListItemWatchedButton.SetCommand("Click", new RelayCommand(ShowWatchedDialog));
            AnimeListItemStatusButton.SetCommand("Click", new RelayCommand(ShowStatusDialog));
            AnimeListItemScoreButton.SetCommand("Click", new RelayCommand(ShowRatingDialog));

            AnimeListItemIncButton.SetCommand("Click", ViewModel.IncrementWatchedCommand);
            AnimeListItemDecButton.SetCommand("Click", ViewModel.DecrementWatchedCommand);

            RootContainer.SetOnClickListener(new OnClickListener(view => ContainerOnClick()));

            ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;
        }

        protected override void BindModelBasic()
        {
            ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;

            AnimeListItemTitle.Text = ViewModel.Title;


            if (string.IsNullOrEmpty(ViewModel.Type))
            {
                AnimeListItemTypeTextView.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeListItemTypeTextView.Visibility = ViewStates.Visible;
                AnimeListItemTypeTextView.Text = ViewModel.Type;
            }
        }

        protected override void RootContainerInit()
        {
            
        }

        protected override void CreateBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.MyStatusBindShort).WhenSourceChanges(() =>
            {
                AnimeListItemWatchedButton.Text = ViewModel.MyStatusBind;
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.MyEpisodesBindShort).WhenSourceChanges(() =>
            {
                AnimeListItemStatusButton.Text = ViewModel.MyEpisodesBind;
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.MyScoreBindShort).WhenSourceChanges(() =>
            {
                AnimeListItemScoreButton.Text = ViewModel.MyScoreBind;
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.IncrementEpsVisibility).WhenSourceChanges(() =>
            {
                AnimeListItemIncButton.Visibility = ViewModel.IncrementEpsVisibility
                    ? ViewStates.Visible
                    : ViewStates.Gone;
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.DecrementEpsVisibility).WhenSourceChanges(() =>
            {
                AnimeListItemDecButton.Visibility = ViewModel.DecrementEpsVisibility
                    ? ViewStates.Visible
                    : ViewStates.Gone;
            }));

            //Bindings.Add(this.SetBinding(() => ViewModel.AddToListVisibility).WhenSourceChanges(() =>
            //{
            //    Add.Visibility = ViewModel.AddToListVisibility ? ViewStates.Visible : ViewStates.Gone;
            //}));
        }

        #region Flyouts

        private void MoreButtonOnClick()
        {
            _menu = AnimeItemFlyoutBuilder.BuildForAnimeItem(MainActivity.CurrentContext, AnimeListItemMoreButton, null,
                OnMoreFlyoutSelection, true);
            _menu.Show();
        }

        private void OnMoreFlyoutSelection(AnimeGridItemMoreFlyoutButtons animeGridItemMoreFlyoutButtons)
        {
            switch (animeGridItemMoreFlyoutButtons)
            {
                case AnimeGridItemMoreFlyoutButtons.CopyLink:
                    ViewModel.CopyLinkToClipboardCommand.Execute(null);
                    break;
                case AnimeGridItemMoreFlyoutButtons.OpenInBrowser:
                    ViewModel.OpenInMALCommand.Execute(null);
                    break;
            }
            _menu.Dismiss(true);
        }


        #endregion

        #region Dialogs

        private void ShowStatusDialog()
        {
            AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel, ViewModel.ParentAbstraction.RepresentsAnime);
        }
        private void ShowWatchedDialog()
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel);
        }
        private void ShowRatingDialog()
        {
            AnimeUpdateDialogBuilder.BuildScoreDialog(ViewModel);
        }
        #endregion

        private void ContainerOnClick()
        {
            if (_onItemClickAction != null)
                _onItemClickAction.Invoke(ViewModel);
            else
                ViewModel.NavigateDetailsCommand.Execute(null);
        }

        #region Views

        private ProgressBar _animeListItemImgPlaceholder;
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

        public ProgressBar AnimeListItemImgPlaceholder => _animeListItemImgPlaceholder ?? (_animeListItemImgPlaceholder = FindViewById<ProgressBar>(Resource.Id.AnimeListItemImgPlaceholder));

        public ImageViewAsync AnimeListItemImage => _animeListItemImage ?? (_animeListItemImage = FindViewById<ImageViewAsync>(Resource.Id.AnimeListItemImage));

        public ImageButton AnimeListItemMoreButton => _animeListItemMoreButton ?? (_animeListItemMoreButton = FindViewById<ImageButton>(Resource.Id.AnimeListItemMoreButton));

        public TextView AnimeListItemTitle => _animeListItemTitle ?? (_animeListItemTitle = FindViewById<TextView>(Resource.Id.AnimeListItemTitle));

        public TextView AnimeGridItemToLeftInfo => _animeGridItemToLeftInfo ?? (_animeGridItemToLeftInfo = FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo));

        public TextView AnimeListItemTypeTextView => _animeListItemTypeTextView ?? (_animeListItemTypeTextView = FindViewById<TextView>(Resource.Id.AnimeListItemTypeTextView));

        public Button AnimeListItemWatchedButton => _animeListItemWatchedButton ?? (_animeListItemWatchedButton = FindViewById<Button>(Resource.Id.AnimeListItemWatchedButton));

        public LinearLayout AnimeListItemBtmRightSectionTop => _animeListItemBtmRightSectionTop ?? (_animeListItemBtmRightSectionTop = FindViewById<LinearLayout>(Resource.Id.AnimeListItemBtmRightSectionTop));

        public Button AnimeListItemStatusButton => _animeListItemStatusButton ?? (_animeListItemStatusButton = FindViewById<Button>(Resource.Id.AnimeListItemStatusButton));

        public Button AnimeListItemScoreButton => _animeListItemScoreButton ?? (_animeListItemScoreButton = FindViewById<Button>(Resource.Id.AnimeListItemScoreButton));

        public LinearLayout AnimeListItemStatusScoreSection => _animeListItemStatusScoreSection ?? (_animeListItemStatusScoreSection = FindViewById<LinearLayout>(Resource.Id.AnimeListItemStatusScoreSection));

        public ImageButton AnimeListItemIncButton => _animeListItemIncButton ?? (_animeListItemIncButton = FindViewById<ImageButton>(Resource.Id.AnimeListItemIncButton));

        public ImageButton AnimeListItemDecButton => _animeListItemDecButton ?? (_animeListItemDecButton = FindViewById<ImageButton>(Resource.Id.AnimeListItemDecButton));

        public LinearLayout AnimeListItemIncDecSection => _animeListItemIncDecSection ?? (_animeListItemIncDecSection = FindViewById<LinearLayout>(Resource.Id.AnimeListItemIncDecSection));


        #endregion
    }
}