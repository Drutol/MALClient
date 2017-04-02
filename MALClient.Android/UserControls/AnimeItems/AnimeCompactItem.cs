using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.UserControls.AnimeItems
{
    public class AnimeCompactItem : UserControlBase<AnimeItemViewModel,FrameLayout>
    {
        private int _position;
        private readonly Action<AnimeItemViewModel> _onClick;

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #region Constructors

        public AnimeCompactItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnimeCompactItem(Context context,int position, Action<AnimeItemViewModel> onClick) : base(context)
        {
            _position = position;
            _onClick = onClick;
        }

        public AnimeCompactItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public AnimeCompactItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public AnimeCompactItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }


        #endregion

        protected override int ResourceId => Resource.Layout.AnimeCompactItem;

        protected override void BindModelFling()
        {
           
        }

        protected override void BindModelFull()
        {
            AnimeCompactItemGlobalScore.Text = ViewModel.GlobalScoreBind;
            AnimeCompactItemTopLeftInfo.Text = ViewModel.TopLeftInfoBind;

            AnimeCompactItemScoreLabel.Text = ViewModel.MyScoreBind;
            AnimeCompactItemStatusLabel.Text = ViewModel.MyStatusBind;
            AnimeCompactItemWatchedButton.Text = ViewModel.MyEpisodesBind;
            AnimeCompactItemIncButton.Visibility =
                ViewModel.IncrementEpsVisibility ? ViewStates.Visible : ViewStates.Gone;
            AnimeCompactItemDecButton.Visibility =
                ViewModel.DecrementEpsVisibility ? ViewStates.Visible : ViewStates.Gone;

            AnimeCompactItemIncButton.SetOnClickListener(new OnClickListener(view => ViewModel.IncrementWatchedCommand.Execute(null)));
            AnimeCompactItemDecButton.SetOnClickListener(new OnClickListener(view => ViewModel.DecrementWatchedCommand.Execute(null)));

            if (ViewModel.Auth)
                AnimeCompactItemWatchedButton.SetOnClickListener(new OnClickListener(view => ShowWatchedDialog()));

            RootContainer.SetOnClickListener(new OnClickListener(v => ContainerOnClick()));

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;

            AnimeCompactItemWatchedButton.Text = ViewModel.MyStatusBind;
            AnimeCompactItemStatusLabel.Text = ViewModel.MyEpisodesBind;
            AnimeCompactItemScoreLabel.Text = ViewModel.MyScoreBind;
            AnimeCompactItemIncButton.Visibility = ViewModel.IncrementEpsVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;
            AnimeCompactItemDecButton.Visibility = ViewModel.DecrementEpsVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;
        }

        protected override void BindModelBasic()
        {
            RootContainer.SetBackgroundResource(_position % 2 == 0
                ? ResourceExtension.BrushRowAlternate1Res
                : ResourceExtension.BrushRowAlternate2Res);

            AnimeCompactItemType.Text = ViewModel.PureType;
            AnimeCompactItemTitle.Text = ViewModel.Title;

            AnimeCompactItemFavouriteIndicator.Visibility =
                ViewModel.IsFavouriteVisibility ? ViewStates.Visible : ViewStates.Gone;
            AnimeCompactItemTagsButton.Visibility = ViewModel.TagsControlVisibility
                ? ViewStates.Visible
                : ViewStates.Gone;


        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(ViewModel.MyStatusBindShort):
                    AnimeCompactItemWatchedButton.Text = ViewModel.MyStatusBind;
                    break;
                case nameof(ViewModel.MyEpisodesBindShort):
                    AnimeCompactItemStatusLabel.Text = ViewModel.MyEpisodesBind;
                    break;
                case nameof(ViewModel.MyScoreBindShort):
                    AnimeCompactItemScoreLabel.Text = ViewModel.MyScoreBind;
                    break;
                case nameof(ViewModel.IncrementEpsVisibility):
                    AnimeCompactItemIncButton.Visibility = ViewModel.IncrementEpsVisibility
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                    break;
                case nameof(ViewModel.DecrementEpsVisibility):
                    AnimeCompactItemDecButton.Visibility = ViewModel.DecrementEpsVisibility
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                    break;
            }
        }

        protected override void CleanupPreviousModel()
        {
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private DroppyMenuPopup _tagsMenu;
        private void OnTagsButtonClick(View view)
        {
            _tagsMenu = AnimeItemFlyoutBuilder.BuildForAnimeItemTags(Context, view, ViewModel,
                () => _tagsMenu.Dismiss(true));
            _tagsMenu.Show();
        }

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
            if (_onClick != null)
                _onClick.Invoke(ViewModel);
            else
                ViewModel.NavigateDetailsCommand.Execute(null);
        }

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            var general = AnimeCompactItemGeneralSection;
            var edit = AnimeCompactItemEditSection;
            var stretcherLeft = AnimeCompactItemAdaptiveItemLeft;
            var stretcherRight = AnimeCompactItemAdaptiveItemRight;
            var titleLabel = AnimeCompactItemTitle;

            var parameter = general.LayoutParameters;
            var editParam = edit.LayoutParameters as RelativeLayout.LayoutParams;
            var titleParam = titleLabel.LayoutParameters as LinearLayout.LayoutParams;

            if (newConfig.ScreenWidthDp > 590)
            {
                if (parameter.Width == -2)
                    return;

                parameter.Width = -2;
                stretcherLeft.Visibility = ViewStates.Visible;
                stretcherRight.Visibility = ViewStates.Gone;
                titleParam.Width = -1;
                titleParam.Weight = 0;

                editParam.RemoveRule(LayoutRules.Below);
                editParam.AddRule(LayoutRules.RightOf, Resource.Id.AnimeCompactItemGeneralSection);
            }
            else
            {
                if (parameter.Width == -1)
                    return;

                parameter.Width = -1;
                stretcherLeft.Visibility = ViewStates.Gone;
                stretcherRight.Visibility = ViewStates.Visible;
                titleParam.Width = 0;
                titleParam.Weight = 1;

                editParam.RemoveRule(LayoutRules.RightOf);
                editParam.AddRule(LayoutRules.Below, Resource.Id.AnimeCompactItemGeneralSection);
            }

            titleLabel.LayoutParameters = titleParam;
            edit.LayoutParameters = editParam;
            general.LayoutParameters = parameter;
        }

        #region Views

        private TextView _animeCompactItemGlobalScore;
        private TextView _animeCompactItemType;
        private TextView _animeCompactItemTitle;
        private TextView _animeCompactItemTopLeftInfo;
        private ImageView _animeCompactItemFavouriteIndicator;
        private FrameLayout _animeCompactItemTagsButton;
        private LinearLayout _animeCompactItemGeneralSection;
        private View _animeCompactItemAdaptiveItemLeft;
        private TextView _animeCompactItemScoreLabel;
        private FrameLayout _animeCompactItemScoreButton;
        private TextView _animeCompactItemStatusLabel;
        private FrameLayout _animeCompactItemStatusButton;
        private View _animeCompactItemAdaptiveItemRight;
        private FrameLayout _animeCompactItemDecButton;
        private FrameLayout _animeCompactItemIncButton;
        private Button _animeCompactItemWatchedButton;
        private LinearLayout _animeCompactItemEditSection;

        public TextView AnimeCompactItemGlobalScore => _animeCompactItemGlobalScore ?? (_animeCompactItemGlobalScore = FindViewById<TextView>(Resource.Id.AnimeCompactItemGlobalScore));

        public TextView AnimeCompactItemType => _animeCompactItemType ?? (_animeCompactItemType = FindViewById<TextView>(Resource.Id.AnimeCompactItemType));

        public TextView AnimeCompactItemTitle => _animeCompactItemTitle ?? (_animeCompactItemTitle = FindViewById<TextView>(Resource.Id.AnimeCompactItemTitle));

        public TextView AnimeCompactItemTopLeftInfo => _animeCompactItemTopLeftInfo ?? (_animeCompactItemTopLeftInfo = FindViewById<TextView>(Resource.Id.AnimeCompactItemTopLeftInfo));

        public ImageView AnimeCompactItemFavouriteIndicator => _animeCompactItemFavouriteIndicator ?? (_animeCompactItemFavouriteIndicator = FindViewById<ImageView>(Resource.Id.AnimeCompactItemFavouriteIndicator));

        public FrameLayout AnimeCompactItemTagsButton => _animeCompactItemTagsButton ?? (_animeCompactItemTagsButton = FindViewById<FrameLayout>(Resource.Id.AnimeCompactItemTagsButton));

        public LinearLayout AnimeCompactItemGeneralSection => _animeCompactItemGeneralSection ?? (_animeCompactItemGeneralSection = FindViewById<LinearLayout>(Resource.Id.AnimeCompactItemGeneralSection));

        public View AnimeCompactItemAdaptiveItemLeft => _animeCompactItemAdaptiveItemLeft ?? (_animeCompactItemAdaptiveItemLeft = FindViewById<View>(Resource.Id.AnimeCompactItemAdaptiveItemLeft));

        public TextView AnimeCompactItemScoreLabel => _animeCompactItemScoreLabel ?? (_animeCompactItemScoreLabel = FindViewById<TextView>(Resource.Id.AnimeCompactItemScoreLabel));

        public FrameLayout AnimeCompactItemScoreButton => _animeCompactItemScoreButton ?? (_animeCompactItemScoreButton = FindViewById<FrameLayout>(Resource.Id.AnimeCompactItemScoreButton));

        public TextView AnimeCompactItemStatusLabel => _animeCompactItemStatusLabel ?? (_animeCompactItemStatusLabel = FindViewById<TextView>(Resource.Id.AnimeCompactItemStatusLabel));

        public FrameLayout AnimeCompactItemStatusButton => _animeCompactItemStatusButton ?? (_animeCompactItemStatusButton = FindViewById<FrameLayout>(Resource.Id.AnimeCompactItemStatusButton));

        public View AnimeCompactItemAdaptiveItemRight => _animeCompactItemAdaptiveItemRight ?? (_animeCompactItemAdaptiveItemRight = FindViewById<View>(Resource.Id.AnimeCompactItemAdaptiveItemRight));

        public FrameLayout AnimeCompactItemDecButton => _animeCompactItemDecButton ?? (_animeCompactItemDecButton = FindViewById<FrameLayout>(Resource.Id.AnimeCompactItemDecButton));

        public FrameLayout AnimeCompactItemIncButton => _animeCompactItemIncButton ?? (_animeCompactItemIncButton = FindViewById<FrameLayout>(Resource.Id.AnimeCompactItemIncButton));

        public Button AnimeCompactItemWatchedButton => _animeCompactItemWatchedButton ?? (_animeCompactItemWatchedButton = FindViewById<Button>(Resource.Id.AnimeCompactItemWatchedButton));

        public LinearLayout AnimeCompactItemEditSection => _animeCompactItemEditSection ?? (_animeCompactItemEditSection = FindViewById<LinearLayout>(Resource.Id.AnimeCompactItemEditSection));



        #endregion
    }
}