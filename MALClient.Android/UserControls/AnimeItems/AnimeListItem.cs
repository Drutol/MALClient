using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
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

namespace MALClient.Android.UserControls.AnimeItems
{
    public class AnimeListItem : UserControlBase<AnimeItemViewModel,FrameLayout>
    {
        private readonly Action<AnimeItemViewModel> _onItemClickAction;
        private DroppyMenuPopup _menu;
        private DroppyMenuPopup _tagsMenu;

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
            if (!AnimeListItemImage.AnimeIntoIfLoaded(ViewModel.ImgUrl))
            {
                AnimeListItemImage.Visibility = ViewStates.Invisible;
                AnimeListItemImgPlaceholder.Visibility = ViewStates.Visible;
            }
            else
                AnimeListItemImgPlaceholder.Visibility = ViewStates.Gone;
        }

        protected override void BindModelFull()
        {
            if ((string)AnimeListItemImage.Tag != ViewModel.ImgUrl)
            {
                AnimeListItemImage.AnimeInto(ViewModel.ImgUrl, AnimeListItemImgPlaceholder);
            }
            else
            {
                AnimeListItemImage.Visibility = ViewStates.Visible;
            }






            if (ViewModel.Auth)
            {
                AnimeListItemWatchedButton.Clickable = true;
                AnimeListItemWatchedButton.Focusable = true;
                AnimeListItemWatchedButton.SetCommand("Click", new RelayCommand(ShowWatchedDialog));
            }
            else
            {
                AnimeListItemWatchedButton.Clickable = false;
                AnimeListItemWatchedButton.Focusable = false;
            }


            ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;


        }

        protected override void BindModelBasic()
        {
            AnimeListItemStatusButton.Text = ViewModel.MyStatusBind;
            AnimeListItemWatchedButton.Text = ViewModel.MyEpisodesBind;
            AnimeListItemScoreButton.Text = ViewModel.MyScoreBind;
            AnimeListItemIncButton.Visibility = ViewModel.IncrementEpsVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;
            AnimeListItemDecButton.Visibility = ViewModel.DecrementEpsVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;
            AnimeListItemUpdatingBar.Visibility = ViewModel.LoadingUpdate
                ? ViewStates.Visible
                : ViewStates.Gone;

            AnimeListItemAddToListButton.Visibility = ViewModel.AddToListVisibility ? ViewStates.Visible : ViewStates.Gone;
            ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;

            AnimeListItemTitle.Text = ViewModel.Title;

            if (string.IsNullOrEmpty(ViewModel.TopLeftInfoBind))
            {
                AnimeListItemTopLeftInfo.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeListItemTopLeftInfo.Visibility = ViewStates.Visible;
                AnimeListItemTopLeftInfoMain.Text = ViewModel.TopLeftInfoBind;
                if (ViewModel.AirDayBrush == true)
                {
                    AnimeListItemTopLeftInfoMain.SetTextColor(new Color(110, 110, 110)); //gray
                    AnimeListItemTopLeftInfoSub.Text = ViewModel.AirDayTillBind;
                    AnimeListItemTopLeftInfoSub.Visibility = ViewStates.Visible;
                }
                else
                {
                    AnimeListItemTopLeftInfoMain.SetTextColor(new Color(255, 255, 255));
                    AnimeListItemTopLeftInfoSub.Visibility = ViewStates.Gone;
                }
            }

            if (string.IsNullOrEmpty(ViewModel.Type))
            {
                AnimeListItemTypeTextView.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeListItemTypeTextView.Visibility = ViewStates.Visible;
                AnimeListItemTypeTextView.Text = ViewModel.Type;
            }

            AnimeListItemTagsButton.Visibility = ViewModel.TagsControlVisibility ? ViewStates.Visible : ViewStates.Invisible;
            AnimeListItemStatusScoreSection.Visibility = ViewModel.UpdateButtonsVisibility ? ViewStates.Visible : ViewStates.Gone;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(ViewModel.MyStatusBind):
                    AnimeListItemStatusButton.Text = ViewModel.MyStatusBind;
                    break;
                case nameof(ViewModel.MyEpisodesBind):
                    AnimeListItemWatchedButton.Text = ViewModel.MyEpisodesBind;
                    break;
                case nameof(ViewModel.MyScoreBind):
                    AnimeListItemScoreButton.Text = ViewModel.MyScoreBind;
                    break;
                case nameof(ViewModel.IncrementEpsVisibility):
                    AnimeListItemIncButton.Visibility = ViewModel.IncrementEpsVisibility
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                    break;
                case nameof(ViewModel.DecrementEpsVisibility):
                    AnimeListItemDecButton.Visibility = ViewModel.DecrementEpsVisibility
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                    break;
                case nameof(ViewModel.LoadingUpdate):
                    AnimeListItemUpdatingBar.Visibility = ViewModel.LoadingUpdate
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                    break;
                case nameof(ViewModel.AddToListVisibility):
                    AnimeListItemAddToListButton.Visibility = ViewModel.AddToListVisibility ? ViewStates.Visible : ViewStates.Gone;
                    break;
            }
        }

        protected override void CleanupPreviousModel()
        {
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        protected override void RootContainerInit()
        {
            AnimeListItemAddToListButton.SetOnClickListener(new OnClickListener(view => ViewModel.AddAnimeCommand.Execute(null)));
            AnimeListItemTagsButton.SetOnClickListener(new OnClickListener(OnTagsButtonClick));
            AnimeListItemStatusButton.SetOnClickListener(
                new OnClickListener(view => ShowStatusDialog()));
            AnimeListItemScoreButton.SetOnClickListener(
                new OnClickListener(view => ShowRatingDialog()));
            AnimeListItemIncButton.SetOnClickListener(
                new OnClickListener(view => ViewModel.IncrementWatchedCommand.Execute(null)));
            AnimeListItemDecButton.SetOnClickListener(
                new OnClickListener(view => ViewModel.DecrementWatchedCommand.Execute(null)));

            RootContainer.SetOnLongClickListener(new OnLongClickListener(view => MoreButtonOnClick()));
            RootContainer.SetOnClickListener(new OnClickListener(view => ContainerOnClick()));
            base.RootContainerInit();
        }

        #region Flyouts

        private void OnTagsButtonClick(View view)
        {
            _tagsMenu = AnimeItemFlyoutBuilder.BuildForAnimeItemTags(Context, view, ViewModel,
                () => _tagsMenu.Dismiss(true));
            _tagsMenu.Show();
        }

        private void MoreButtonOnClick()
        {
            _menu = AnimeItemFlyoutBuilder.BuildForAnimeItem(MainActivity.CurrentContext, AnimeListItemImgPlaceholder, null,
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
                case AnimeGridItemMoreFlyoutButtons.CopyTitle:
                    ViewModel.CopyTitleToClipboardCommand.Execute(null);
                    break;
            }
            _menu.Dismiss(true);
        }


        #endregion

        #region Dialogs

        private void ShowStatusDialog()
        {
            if (ViewModel.Auth)
                AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel, ViewModel.ParentAbstraction.RepresentsAnime);
        }
        private void ShowWatchedDialog()
        {
            if (ViewModel.Auth)
                AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel);
        }
        private void ShowRatingDialog()
        {
            if (ViewModel.Auth)
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
        private FrameLayout _animeListItemTagsButton;
        private ImageView _imageView;
        private FrameLayout _animeListItemAddToListButton;
        private ProgressBar _animeListItemUpdatingBar;
        private TextView _animeListItemTitle;
        private TextView _animeListItemTopLeftInfoMain;
        private TextView _animeListItemTopLeftInfoSub;
        private LinearLayout _animeListItemTopLeftInfo;
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

        public FrameLayout AnimeListItemTagsButton => _animeListItemTagsButton ?? (_animeListItemTagsButton = FindViewById<FrameLayout>(Resource.Id.AnimeListItemTagsButton));

        public ImageView ImageView => _imageView ?? (_imageView = FindViewById<ImageView>(Resource.Id.imageView));

        public FrameLayout AnimeListItemAddToListButton => _animeListItemAddToListButton ?? (_animeListItemAddToListButton = FindViewById<FrameLayout>(Resource.Id.AnimeListItemAddToListButton));

        public ProgressBar AnimeListItemUpdatingBar => _animeListItemUpdatingBar ?? (_animeListItemUpdatingBar = FindViewById<ProgressBar>(Resource.Id.AnimeListItemUpdatingBar));

        public TextView AnimeListItemTitle => _animeListItemTitle ?? (_animeListItemTitle = FindViewById<TextView>(Resource.Id.AnimeListItemTitle));

        public TextView AnimeListItemTopLeftInfoMain => _animeListItemTopLeftInfoMain ?? (_animeListItemTopLeftInfoMain = FindViewById<TextView>(Resource.Id.AnimeListItemTopLeftInfoMain));

        public TextView AnimeListItemTopLeftInfoSub => _animeListItemTopLeftInfoSub ?? (_animeListItemTopLeftInfoSub = FindViewById<TextView>(Resource.Id.AnimeListItemTopLeftInfoSub));

        public LinearLayout AnimeListItemTopLeftInfo => _animeListItemTopLeftInfo ?? (_animeListItemTopLeftInfo = FindViewById<LinearLayout>(Resource.Id.AnimeListItemTopLeftInfo));

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