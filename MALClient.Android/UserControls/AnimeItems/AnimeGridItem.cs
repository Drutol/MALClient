using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Daimajia.Swipe;
using Com.Shehabic.Droppy;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.UserControls
{
    public class AnimeGridItem : UserControlBase<AnimeItemViewModel,SwipeLayout>
    {
        private readonly bool _allowSwipeInGivenContext;
        private readonly Action<AnimeItemViewModel> _onItemClickAction;
        private readonly bool _displayTimeTillAir;


        private bool _propertyHandlerAttached;

        #region Constructors

        public AnimeGridItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnimeGridItem(Context context,bool allowSwipeInGivenContext,Action<AnimeItemViewModel> onItemClickAction,bool displayTimeTillAir = false) : base(context)
        {
            if(Settings.EnableSwipeToIncDec)
                _allowSwipeInGivenContext = allowSwipeInGivenContext;
            _onItemClickAction = onItemClickAction;
            _displayTimeTillAir = displayTimeTillAir;
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
            if (!AnimeGridItemImage.AnimeIntoIfLoaded(ViewModel.ImgUrl))
            {
                AnimeGridItemImage.Visibility = ViewStates.Invisible;
                AnimeGridItemImgPlaceholder.Visibility = ViewStates.Visible;
            }
            else
                AnimeGridItemImgPlaceholder.Visibility = ViewStates.Gone;

        }

        protected override void BindModelFull()
        {
            if ((string)AnimeGridItemImage.Tag != ViewModel.ImgUrl)
            {
                AnimeGridItemImage.AnimeInto(ViewModel.ImgUrl, AnimeGridItemImgPlaceholder);         
            }
            else
            {
                AnimeGridItemImage.Visibility = ViewStates.Visible;
            }

            
            if (ViewModel.Auth)
            {
                var listener = new OnClickListener(view => ShowWatchedDialog());
                AnimeGridItemWatchedStatusButton.SetOnClickListener(listener);
                AnimeGridItemTopRightInfo.SetOnClickListener(listener);
            }
            else
            {
                AnimeGridItemWatchedStatusButton.Clickable = AnimeGridItemWatchedStatusButton.Focusable =
                    AnimeGridItemTopRightInfo.Clickable = AnimeGridItemTopRightInfo.Focusable = false;
            }
            
            

            if (!_propertyHandlerAttached)
            {
                ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
                _propertyHandlerAttached = true;
            }
         
            if (_allowSwipeInGivenContext && ViewModel.Auth)
            {
                RootContainer.SwipeEnabled = true;
                RootContainer.LeftSwipeEnabled = true;
                RootContainer.RightSwipeEnabled = true;
            }
            else
            {
                RootContainer.SwipeEnabled = false;
                RootContainer.LeftSwipeEnabled = false;
                RootContainer.RightSwipeEnabled = false;
            }
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(ViewModel.MyStatusBindShort):
                    AnimeGridItemCurrentWatchingStatus.Text = ViewModel.MyStatusBindShort;
                    if (ViewModelLocator.GeneralMain.CurrentMainPageKind == PageIndex.PageAnimeList)
                    {
                        var targetStatus = ViewModelLocator.AnimeList.GetDesiredStatus();
                        if (!ViewModel.IsRewatching && targetStatus != AnimeStatus.AllOrAiring && ViewModel.MyStatus != targetStatus)
                            Alpha = .6f;
                        else
                            Alpha = 1;
                    }
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
                case nameof(ViewModel.Auth):
                    BindModelFull();
                    BindModelBasic();
                    break;
            }
        }

        protected override void CleanupPreviousModel()
        {
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            _propertyHandlerAttached = false;
        }

        protected override void BindModelBasic()
        {
            ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;

            AnimeGridItemTitle.Text = ViewModel.Title;
            AnimeGridItemFavouriteIndicator.Visibility = ViewModel.IsFavouriteVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;
            AnimeGridItemTopRightInfo.Visibility = ViewModel.UpdateButtonsVisibility ? ViewStates.Visible : ViewStates.Gone;

            AnimeGridItemCurrentWatchingStatus.Text = ViewModel.MyStatusBindShort;
            AnimeGridItemWatchedStatus.Text = ViewModel.MyEpisodesBindShort;
            AnimeGridItemScore.Text = ViewModel.MyScoreBindShort;
            AnimeGridItemAddToListButton.Visibility = ViewModel.AddToListVisibility ? ViewStates.Visible : ViewStates.Gone;
            if (string.IsNullOrEmpty(ViewModel.TopLeftInfoBind))
            {
                AnimeGridItemTopLeftInfo.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeGridItemTopLeftInfo.Visibility = ViewStates.Visible;
                AnimeGridItemTopLeftInfoMain.Text = ViewModel.TopLeftInfoBind;
                if (ViewModel.AirDayBrush == true && ViewModel.AnimeItemDisplayContext != AnimeItemDisplayContext.Index)
                {
                    AnimeGridItemTopLeftInfoMain.SetTextColor(new Color(110, 110, 110)); //gray
                }
                else
                {
                    AnimeGridItemTopLeftInfoMain.SetTextColor(new Color(255, 255, 255));
                }

                if (!string.IsNullOrEmpty(ViewModel.AirDayTillBind))
                {
                    AnimeGridItemTopLeftInfoSub.Text = ViewModel.AirDayTillBind;
                    AnimeGridItemTopLeftInfoSub.Visibility = ViewStates.Visible;
                }
                else
                {
                    AnimeGridItemTopLeftInfoSub.Visibility = ViewStates.Gone;
                }
            }



            if (_displayTimeTillAir)
            {
                if(string.IsNullOrEmpty(ViewModel.TimeTillNextAirCache))
                   AnimeGridItemTimeTillAir.Visibility = ViewStates.Gone;
                else
                {
                    AnimeGridItemTimeTillAir.Visibility = ViewStates.Visible;
                    AnimeGridItemTimeTillAir.Text = ViewModel.TimeTillNextAirCache;
                }

            }


            if (string.IsNullOrEmpty(ViewModel.Type))
            {
                AnimeGridItemType.Visibility = ViewStates.Gone;
            }
            else
            {
                AnimeGridItemType.Visibility = ViewStates.Visible;
                AnimeGridItemType.Text = ViewModel.Type;
            }

            if (ViewModelLocator.GeneralMain.CurrentMainPageKind == PageIndex.PageAnimeList)
            {
                var targetStatus = ViewModelLocator.AnimeList.GetDesiredStatus();
                if (!ViewModel.IsRewatching && targetStatus != AnimeStatus.AllOrAiring && ViewModel.MyStatus != targetStatus)
                    Alpha = .6f;
                else
                    Alpha = 1;
            }
            else
            {
                Alpha = 1;
            }


            AnimeGridItemTagsButton.Visibility = ViewModel.TagsControlVisibility ? ViewStates.Visible : ViewStates.Invisible;
        }

        protected override void RootContainerInit()
        {
            RootContainer.SetOnClickListener(new OnClickListener(view => ContainerOnClick()));
            AnimeGridItemMoreButton.SetOnClickListener(new OnClickListener(view => MoreButtonOnClick()));

            AnimeGridItemTagsButton.SetOnClickListener(new OnClickListener(OnTagsButtonClick));

            AnimeGridItemAddToListButton.SetOnClickListener(new OnClickListener(view => ViewModel.AddAnimeCommand.Execute(null)));

            if (_allowSwipeInGivenContext)
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

            if (Settings.MakeGridItemsSmaller)
            {
                AnimeGridItemUpperSection.LayoutParameters.Height = DimensionsHelper.DpToPx(174);
                AnimeGridItemLowerSection.LayoutParameters.Height = DimensionsHelper.DpToPx(44);

                AnimeGridItemTagsButton.LayoutParameters.Height =
                    AnimeGridItemTagsButton.LayoutParameters.Width = DimensionsHelper.DpToPx(30);

                AnimeGridItemBackSurfaceAdd.LayoutParameters.Width = DimensionsHelper.DpToPx(66);
                AnimeGridItemBackSurfaceSubtract.LayoutParameters.Width = DimensionsHelper.DpToPx(72);

                AnimeGridItemBackSurfaceAdd.GetChildAt(0).TranslationX /= 1.2f;

                AnimeGridItemCurrentWatchingStatus.SetTextSize(ComplexUnitType.Sp, 13);
                AnimeGridItemWatchedStatus.SetTextSize(ComplexUnitType.Sp, 13);
                AnimeGridItemScore.SetTextSize(ComplexUnitType.Sp, 13);
                AnimeGridItemTopLeftInfoMain.SetTextSize(ComplexUnitType.Sp, 13);
                AnimeGridItemTopLeftInfoSub.SetTextSize(ComplexUnitType.Sp, 11);
                AnimeGridItemType.SetTextSize(ComplexUnitType.Sp, 11);

                AnimeGridItemTitle.SetTextSize(ComplexUnitType.Sp, 13);
                AnimeGridItemMoreButton.ScaleX = AnimeGridItemMoreButton.ScaleY = .85f;
                AnimeGridItemFavouriteIndicator.ScaleX = AnimeGridItemMoreButton.ScaleY = .85f;
                AnimeGridItemTagIcon.ScaleX = AnimeGridItemTagIcon.ScaleY = .85f;
            }

            if (Settings.ReverseSwipingDirection)
            {
                AnimeGridItemBackSurfaceAdd.SetBackgroundColor(new Color(ResourceExtension.BrushFlyoutBackground));
                SurfaceAddIcon.SetImageResource(Resource.Drawable.icon_minus);
                SurfaceAddIcon.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));

                AnimeGridItemBackSurfaceSubtract.SetBackgroundColor(new Color(ResourceExtension.AccentColour));
                SurfaceSubtractIcon.SetImageResource(Resource.Drawable.icon_add);
                SurfaceSubtractIcon.ImageTintList = ColorStateList.ValueOf(Color.White);
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
                case AnimeGridItemMoreFlyoutButtons.CopyTitle:
                    ViewModel.CopyTitleToClipboardCommand.Execute(null);
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
            {
                if (Settings.ReverseSwipingDirection)
                    ViewModel.DecrementWatchedCommand.Execute(null);
                else
                    ViewModel.IncrementWatchedCommand.Execute(null);
            }
            else if (edge == SwipeLayout.DragEdge.Left)
            {
                if (Settings.ReverseSwipingDirection)
                    ViewModel.IncrementWatchedCommand.Execute(null);
                else
                    ViewModel.DecrementWatchedCommand.Execute(null);
            }
            await Task.Delay(350);

            sender.Close();

            _swipeCooldown = false;
        }


        #endregion

        #region Dialogs
        private void ShowStatusDialog()
        {
            if (ViewModel.Auth)
                AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel, ViewModel.ParentAbstraction.RepresentsAnime,ViewModel.ChangeStatus);
        }
        private void ShowWatchedDialog()
        {
            if (ViewModel.Auth)
                AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel,null,ViewModel.ParentAbstraction.RepresentsAnime ? false : Settings.MangaFocusVolumes);
        }
        private void ShowRatingDialog()
        {
            if (ViewModel.Auth)
                AnimeUpdateDialogBuilder.BuildScoreDialog(ViewModel,f => ViewModel.ChangeScore(f.ToString()));
        }
        #endregion

        #region Views

        private ImageView _surfaceAddIcon;
        private RelativeLayout _animeGridItemBackSurfaceAdd;
        private ImageView _surfaceSubtractIcon;
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
        private TextView _animeGridItemTimeTillAir;
        private ImageView _animeGridItemTagIcon;
        private FrameLayout _animeGridItemTagsButton;
        private ImageView _imageView;
        private FrameLayout _animeGridItemAddToListButton;
        private RelativeLayout _animeGridItemUpperSection;
        private TextView _animeGridItemTitle;
        private ImageButton _animeGridItemMoreButton;
        private LinearLayout _animeGridItemLowerSection;

        public ImageView SurfaceAddIcon => _surfaceAddIcon ?? (_surfaceAddIcon = FindViewById<ImageView>(Resource.Id.SurfaceAddIcon));   

        public RelativeLayout AnimeGridItemBackSurfaceAdd => _animeGridItemBackSurfaceAdd ?? (_animeGridItemBackSurfaceAdd = FindViewById<RelativeLayout>(Resource.Id.AnimeGridItemBackSurfaceAdd));

        public ImageView SurfaceSubtractIcon => _surfaceSubtractIcon ?? (_surfaceSubtractIcon = FindViewById<ImageView>(Resource.Id.SurfaceSubtractIcon));

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

        public TextView AnimeGridItemTimeTillAir => _animeGridItemTimeTillAir ?? (_animeGridItemTimeTillAir = FindViewById<TextView>(Resource.Id.AnimeGridItemTimeTillAir));

        public ImageView AnimeGridItemTagIcon => _animeGridItemTagIcon ?? (_animeGridItemTagIcon = FindViewById<ImageView>(Resource.Id.AnimeGridItemTagIcon));

        public FrameLayout AnimeGridItemTagsButton => _animeGridItemTagsButton ?? (_animeGridItemTagsButton = FindViewById<FrameLayout>(Resource.Id.AnimeGridItemTagsButton));

        public ImageView ImageView => _imageView ?? (_imageView = FindViewById<ImageView>(Resource.Id.imageView));

        public FrameLayout AnimeGridItemAddToListButton => _animeGridItemAddToListButton ?? (_animeGridItemAddToListButton = FindViewById<FrameLayout>(Resource.Id.AnimeGridItemAddToListButton));

        public RelativeLayout AnimeGridItemUpperSection => _animeGridItemUpperSection ?? (_animeGridItemUpperSection = FindViewById<RelativeLayout>(Resource.Id.AnimeGridItemUpperSection));

        public TextView AnimeGridItemTitle => _animeGridItemTitle ?? (_animeGridItemTitle = FindViewById<TextView>(Resource.Id.AnimeGridItemTitle));

        public ImageButton AnimeGridItemMoreButton => _animeGridItemMoreButton ?? (_animeGridItemMoreButton = FindViewById<ImageButton>(Resource.Id.AnimeGridItemMoreButton));

        public LinearLayout AnimeGridItemLowerSection => _animeGridItemLowerSection ?? (_animeGridItemLowerSection = FindViewById<LinearLayout>(Resource.Id.AnimeGridItemLowerSection));



        #endregion
    }
}