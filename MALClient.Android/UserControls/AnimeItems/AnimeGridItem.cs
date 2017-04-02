using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Daimajia.Swipe;
using Com.Shehabic.Droppy;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.UserControls
{
    public class AnimeGridItem : UserControlBase<AnimeItemViewModel,SwipeLayout>
    {
        private readonly bool _allowSwipeInGivenContext;
        private readonly Action<AnimeItemViewModel> _onItemClickAction;

        #region Constructors

        public AnimeGridItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnimeGridItem(Context context,bool allowSwipeInGivenContext,Action<AnimeItemViewModel> onItemClickAction) : base(context)
        {
            _allowSwipeInGivenContext = allowSwipeInGivenContext;
            _onItemClickAction = onItemClickAction;
        }

        public AnimeGridItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public AnimeGridItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public AnimeGridItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        protected override int ResourceId => Resource.Layout.AnimeGridItem;

        protected override void BindModelFling()
        {           
            AnimeGridItemImage.Visibility = ViewStates.Invisible;
            AnimeGridItemImgPlaceholder.Visibility = ViewStates.Visible;
        }

        protected override void BindModelFull()
        {
            if ((string)AnimeGridItemImage.Tag != ViewModel.ImgUrl)
            {
                AnimeGridItemImage.AnimeInto(ViewModel.ImgUrl);
                AnimeGridItemImage.Tag = ViewModel.ImgUrl;
            }
            else
            {
                AnimeGridItemImage.Visibility = ViewStates.Visible;
            }

            AnimeGridItemImgPlaceholder.Visibility = ViewStates.Gone;

            RootContainer.SetOnClickListener(new OnClickListener(view => ContainerOnClick()));
            AnimeGridItemMoreButton.SetOnClickListener(new OnClickListener(view => MoreButtonOnClick()));

            AnimeGridItemFavouriteIndicator.Visibility = ViewModel.IsFavouriteVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;


            if (string.IsNullOrEmpty(ViewModel.TopLeftInfoBind))
            {
                AnimeGridItemTopLeftInfo.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeGridItemTopLeftInfo.Visibility = ViewStates.Visible;
                AnimeGridItemTopLeftInfoMain.Text = ViewModel.TopLeftInfoBind;
                if (ViewModel.AirDayBrush == true)
                {
                    AnimeGridItemTopLeftInfoMain.SetTextColor(new Color(80,80,80)); //gray
                    AnimeGridItemTopLeftInfoSub.Text = ViewModel.AirDayTillBind;
                    AnimeGridItemTopLeftInfoSub.Visibility = ViewStates.Visible;
                }
                else
                {
                    AnimeGridItemTopLeftInfoMain.SetTextColor(new Color(255, 255, 255));
                    AnimeGridItemTopLeftInfoSub.Visibility = ViewStates.Gone;
                }
            }

            AnimeGridItemTagsButton.Visibility = ViewModel.TagsControlVisibility ? ViewStates.Visible : ViewStates.Invisible;
            AnimeGridItemTagsButton.SetOnClickListener(new OnClickListener(OnTagsButtonClick));

            AnimeGridItemTopRightInfo.Visibility = ViewModel.Auth ? ViewStates.Visible : ViewStates.Gone;
            AnimeGridItemAddToListButton.SetOnClickListener(new OnClickListener(view => ViewModel.AddAnimeCommand.Execute(null)));
            AnimeGridItemWatchedStatusButton.SetOnClickListener(new OnClickListener(view => ShowWatchedDialog()));
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;


            AnimeGridItemCurrentWatchingStatus.Text = ViewModel.MyStatusBindShort;
            AnimeGridItemWatchedStatus.Text = ViewModel.MyEpisodesBindShort;
            AnimeGridItemScore.Text = ViewModel.MyScoreBindShort;
            AnimeGridItemAddToListButton.Visibility = ViewModel.AddToListVisibility ? ViewStates.Visible : ViewStates.Gone;

        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(ViewModel.MyStatusBindShort):
                    AnimeGridItemCurrentWatchingStatus.Text = ViewModel.MyStatusBindShort;
                    break;
                case nameof(ViewModel.MyEpisodesBindShort):
                    AnimeGridItemWatchedStatus.Text = ViewModel.MyEpisodesBindShort;
                    break;
                case nameof(ViewModel.MyScoreBindShort):
                    AnimeGridItemScore.Text = ViewModel.MyScoreBindShort;
                    break;
                case nameof(ViewModel.AddToListVisibility):
                    AnimeGridItemAddToListButton.Visibility = ViewModel.AddToListVisibility ? ViewStates.Visible : ViewStates.Gone;
                    break;
            }
        }

        protected override void CleanupPreviousModel()
        {
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        protected override void BindModelBasic()
        {
            ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;

            AnimeGridItemTitle.Text = ViewModel.Title;


            if (string.IsNullOrEmpty(ViewModel.Type))
            {
                AnimeGridItemType.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeGridItemType.Visibility = ViewStates.Visible;
                AnimeGridItemType.Text = ViewModel.Type;
            }
        }

        protected override void RootContainerInit()
        {
            if (_allowSwipeInGivenContext && ViewModel.Auth)
            {
                RootContainer.SwipeEnabled = true;

                RootContainer.SetShowMode(SwipeLayout.ShowMode.LayDown);

                RootContainer.LeftSwipeEnabled = true;
                RootContainer.RightSwipeEnabled = true;
                RootContainer.ClickToClose = false;

                _swipeListener = new SwipeLayoutListener();
                RootContainer.SwipeListener = _swipeListener;

                RootContainer.AddDrag(SwipeLayout.DragEdge.Right,
                    FindViewById(Resource.Id.AnimeGridItemBackSurfaceAdd));
                RootContainer.AddDrag(SwipeLayout.DragEdge.Left,
                    FindViewById(Resource.Id.AnimeGridItemBackSurfaceSubtract));

                _swipeListener.OnOpenAction = SwipeOnOpenEvent;
            }
            else
            {
                RootContainer.SwipeEnabled = false;
                RootContainer.LeftSwipeEnabled = false;
                RootContainer.RightSwipeEnabled = false;
            }
        }

        private void ContainerOnClick()
        {
            if (_swipeListener?.IsSwiping ?? false)
                return;
            if (_onItemClickAction != null)
                _onItemClickAction.Invoke(ViewModel);
            else
                ViewModel.NavigateDetailsCommand.Execute(null);
        }

        #region Flyouts

        private DroppyMenuPopup _tagsMenu;
        private DroppyMenuPopup _menu;

        private void OnTagsButtonClick(View view)
        {
            _tagsMenu = AnimeItemFlyoutBuilder.BuildForAnimeItemTags(Context, view, ViewModel,
                () => _tagsMenu.Dismiss(true));
            _tagsMenu.Show();
        }

        private void MoreButtonOnClick()
        {
            _menu = AnimeItemFlyoutBuilder.BuildForAnimeItem(Context, AnimeGridItemMoreButton,
                ViewModel,
                MenuOnMenuItemClick);
            _menu.Show();
        }

        private void MenuOnMenuItemClick(AnimeGridItemMoreFlyoutButtons btn)
        {
            switch (btn)
            {
                case AnimeGridItemMoreFlyoutButtons.CopyLink:
                    ViewModel.CopyLinkToClipboardCommand.Execute(null);
                    break;
                case AnimeGridItemMoreFlyoutButtons.OpenInBrowser:
                    ViewModel.OpenInMALCommand.Execute(null);
                    break;
                case AnimeGridItemMoreFlyoutButtons.SetStatus:
                    ShowStatusDialog();
                    break;
                case AnimeGridItemMoreFlyoutButtons.SetRating:
                    ShowRatingDialog();
                    break;
                case AnimeGridItemMoreFlyoutButtons.SetWatched:
                    ShowWatchedDialog();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(btn), btn, null);
            }
            _menu.Dismiss(true);
        }
        #endregion



        #region Swipe

        private SwipeLayoutListener _swipeListener;
        private bool _swipeCooldown;

        private async void SwipeOnOpenEvent(SwipeLayout sender)
        {
            if (_swipeCooldown)
                return;
            _swipeCooldown = true;

            var edge = sender.GetDragEdge();
            if (edge == SwipeLayout.DragEdge.Right)
                ViewModel.IncrementWatchedCommand.Execute(null);
            else if (edge == SwipeLayout.DragEdge.Left)
                ViewModel.DecrementWatchedCommand.Execute(null);
            await Task.Delay(350);

            sender.Close();

            _swipeCooldown = false;
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

        #region Views

        private RelativeLayout _animeGridItemBackSurfaceAdd;
        private RelativeLayout _animeGridItemBackSurfaceSubtract;
        private ProgressBar _animeGridItemImgPlaceholder;
        private ImageViewAsync _animeGridItemImage;
        private TextView _animeGridItemTopLeftInfoMain;
        private TextView _animeGridItemTopLeftInfoSub;
        private LinearLayout _animeGridItemTopLeftInfo;
        private TextView _animeGridItemCurrentWatchingStatus;
        private TextView _animeGridItemWatchedStatus;
        private FrameLayout _animeGridItemWatchedStatusButton;
        private TextView _animeGridItemScore;
        private ImageView _animeGridItemFavouriteIndicator;
        private LinearLayout _animeGridItemTopRightInfo;
        private TextView _animeGridItemType;
        private FrameLayout _animeGridItemTagsButton;
        private ImageView _imageView;
        private FrameLayout _animeGridItemAddToListButton;
        private TextView _animeGridItemTitle;
        private ImageButton _animeGridItemMoreButton;

        public RelativeLayout AnimeGridItemBackSurfaceAdd => _animeGridItemBackSurfaceAdd ?? (_animeGridItemBackSurfaceAdd = FindViewById<RelativeLayout>(Resource.Id.AnimeGridItemBackSurfaceAdd));

        public RelativeLayout AnimeGridItemBackSurfaceSubtract => _animeGridItemBackSurfaceSubtract ?? (_animeGridItemBackSurfaceSubtract = FindViewById<RelativeLayout>(Resource.Id.AnimeGridItemBackSurfaceSubtract));

        public ProgressBar AnimeGridItemImgPlaceholder => _animeGridItemImgPlaceholder ?? (_animeGridItemImgPlaceholder = FindViewById<ProgressBar>(Resource.Id.AnimeGridItemImgPlaceholder));

        public ImageViewAsync AnimeGridItemImage => _animeGridItemImage ?? (_animeGridItemImage = FindViewById<ImageViewAsync>(Resource.Id.AnimeGridItemImage));

        public TextView AnimeGridItemTopLeftInfoMain => _animeGridItemTopLeftInfoMain ?? (_animeGridItemTopLeftInfoMain = FindViewById<TextView>(Resource.Id.AnimeGridItemTopLeftInfoMain));

        public TextView AnimeGridItemTopLeftInfoSub => _animeGridItemTopLeftInfoSub ?? (_animeGridItemTopLeftInfoSub = FindViewById<TextView>(Resource.Id.AnimeGridItemTopLeftInfoSub));

        public LinearLayout AnimeGridItemTopLeftInfo => _animeGridItemTopLeftInfo ?? (_animeGridItemTopLeftInfo = FindViewById<LinearLayout>(Resource.Id.AnimeGridItemTopLeftInfo));

        public TextView AnimeGridItemCurrentWatchingStatus => _animeGridItemCurrentWatchingStatus ?? (_animeGridItemCurrentWatchingStatus = FindViewById<TextView>(Resource.Id.AnimeGridItemCurrentWatchingStatus));

        public TextView AnimeGridItemWatchedStatus => _animeGridItemWatchedStatus ?? (_animeGridItemWatchedStatus = FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus));

        public FrameLayout AnimeGridItemWatchedStatusButton => _animeGridItemWatchedStatusButton ?? (_animeGridItemWatchedStatusButton = FindViewById<FrameLayout>(Resource.Id.AnimeGridItemWatchedStatusButton));

        public TextView AnimeGridItemScore => _animeGridItemScore ?? (_animeGridItemScore = FindViewById<TextView>(Resource.Id.AnimeGridItemScore));

        public ImageView AnimeGridItemFavouriteIndicator => _animeGridItemFavouriteIndicator ?? (_animeGridItemFavouriteIndicator = FindViewById<ImageView>(Resource.Id.AnimeGridItemFavouriteIndicator));

        public LinearLayout AnimeGridItemTopRightInfo => _animeGridItemTopRightInfo ?? (_animeGridItemTopRightInfo = FindViewById<LinearLayout>(Resource.Id.AnimeGridItemTopRightInfo));

        public TextView AnimeGridItemType => _animeGridItemType ?? (_animeGridItemType = FindViewById<TextView>(Resource.Id.AnimeGridItemType));

        public FrameLayout AnimeGridItemTagsButton => _animeGridItemTagsButton ?? (_animeGridItemTagsButton = FindViewById<FrameLayout>(Resource.Id.AnimeGridItemTagsButton));

        public ImageView ImageView => _imageView ?? (_imageView = FindViewById<ImageView>(Resource.Id.imageView));

        public FrameLayout AnimeGridItemAddToListButton => _animeGridItemAddToListButton ?? (_animeGridItemAddToListButton = FindViewById<FrameLayout>(Resource.Id.AnimeGridItemAddToListButton));

        public TextView AnimeGridItemTitle => _animeGridItemTitle ?? (_animeGridItemTitle = FindViewById<TextView>(Resource.Id.AnimeGridItemTitle));

        public ImageButton AnimeGridItemMoreButton => _animeGridItemMoreButton ?? (_animeGridItemMoreButton = FindViewById<ImageButton>(Resource.Id.AnimeGridItemMoreButton));




        #endregion
    }
}