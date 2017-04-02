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
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.UserControls.ForumItems
{
    public class ForumIndexItem : UserControlBase<ForumBoardEntryViewModel,LinearLayout>
    {
        private readonly ForumIndexViewModel _parentViewModel;
        private Binding _binding;

        #region Constructors

        public ForumIndexItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ForumIndexItem(Context context,ForumIndexViewModel parentViewModel) : base(context)
        {
            _parentViewModel = parentViewModel;
        }

        public ForumIndexItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ForumIndexItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ForumIndexItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion


        protected override int ResourceId => Resource.Layout.ForumIndexPageBoardItem;

        protected override void BindModelFling()
        {
            ForumIndexPageBoardItemPeekPost1Image.Visibility = ViewStates.Invisible;
            ForumIndexPageBoardItemPeekPost2Image.Visibility = ViewStates.Invisible;
        }

        protected override void BindModelFull()
        {
            if (_binding == null)
                if (_parentViewModel.LoadingSideContentVisibility)
                {
                    ForumIndexPageBoardItemBoardProgressBar.Visibility = ViewStates.Visible;
                    ForumIndexPageBoardItemPeekPostSection.Visibility = ViewStates.Invisible;
                }
                else
                {
                    ForumIndexPageBoardItemBoardProgressBar.Visibility = ViewStates.Gone;
                    ForumIndexPageBoardItemPeekPostSection.Visibility = ViewStates.Visible;
                    OnFinishedLoading();
                }

            ForumIndexPageBoardItemRootContainer.SetOnClickListener(new OnClickListener(view =>
                _parentViewModel.NavigateBoardCommand.Execute(ViewModel.Board)));
        }

        protected override void BindModelBasic()
        {
            ForumIndexPageBoardItemIcon.SetText(DummyFontAwesomeToRealFontAwesomeConverter.Convert(ViewModel.Icon));

            ForumIndexPageBoardItemBoardName.Text =
                ViewModel.Entry.Name;
            ForumIndexPageBoardItemDecription.Text =
                ViewModel.Entry.Description;

            switch (ViewModel.Board)
            {
                case ForumBoards.Updates:
                    ForumIndexPageBoardItemListHeader.Visibility = ViewStates.Visible;
                    ForumIndexPageBoardItemListHeader.Text = "MyAnimeList";
                    break;
                case ForumBoards.NewsDisc:
                    ForumIndexPageBoardItemListHeader.Visibility = ViewStates.Visible;
                    ForumIndexPageBoardItemListHeader.Text = "Anime & Manga";
                    break;
                case ForumBoards.Intro:
                    ForumIndexPageBoardItemListHeader.Visibility = ViewStates.Visible;
                    ForumIndexPageBoardItemListHeader.Text = "General";
                    break;
                default:
                    ForumIndexPageBoardItemListHeader.Visibility = ViewStates.Gone;
                    break;
            }
        }

        protected override void RootContainerInit()
        {
            ForumIndexPageBoardItemIcon.Typeface = FontManager.GetTypeface(Context, FontManager.TypefacePath);
            if (!ViewModel.ArePeekPostsAvailable)
                _binding = this.SetBinding(() => _parentViewModel.LoadingSideContentVisibility).WhenSourceChanges(() =>
                {
                    if (_parentViewModel.LoadingSideContentVisibility)
                    {
                        ForumIndexPageBoardItemBoardProgressBar.Visibility = ViewStates.Visible;
                        ForumIndexPageBoardItemPeekPostSection.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        ForumIndexPageBoardItemBoardProgressBar.Visibility = ViewStates.Gone;
                        ForumIndexPageBoardItemPeekPostSection.Visibility = ViewStates.Visible;
                        OnFinishedLoading();
                        _binding?.Detach();
                        _binding = null;
                    }
                });
        }

        private void OnFinishedLoading()
        {
            if (ViewModel.ArePeekPostsAvailable)
            {
                var pp1 = ViewModel.Entry.PeekPosts.First();
                ForumIndexPageBoardItemPeekPost1Title.Text =
                    pp1.Title;
                ForumIndexPageBoardItemPeekPost1Date.Text =
                    $"{pp1.PostTime} by {pp1.User.Name}";
                if (ForumIndexPageBoardItemPeekPost1Image.Tag == null ||
                    (string) ForumIndexPageBoardItemPeekPost1Image.Tag != pp1.User.ImgUrl)
                {
                    ForumIndexPageBoardItemPeekPost1Image.Into(pp1.User.ImgUrl);
                    ForumIndexPageBoardItemPeekPost1Image.Tag = pp1.User.ImgUrl;
                }
                else
                {
                    ForumIndexPageBoardItemPeekPost1Image.Visibility = ViewStates.Visible;
                }

                if (ViewModel.Entry.PeekPosts.Count() == 2)
                {
                    var pp2 = ViewModel.Entry.PeekPosts.Last();
                    ForumIndexPageBoardItemPeekPost2Title.Text =
                        pp2.Title;
                    ForumIndexPageBoardItemPeekPost2Date.Text =
                        $"{pp2.PostTime} by {pp2.User.Name}";
                    if (ForumIndexPageBoardItemPeekPost2Image.Tag == null ||
                        (string)ForumIndexPageBoardItemPeekPost2Image.Tag != pp2.User.ImgUrl)
                    {
                        ForumIndexPageBoardItemPeekPost2Image.Into(pp2.User.ImgUrl);
                        ForumIndexPageBoardItemPeekPost2Image.Tag = pp2.User.ImgUrl;
                    }
                    else
                    {
                        ForumIndexPageBoardItemPeekPost2Image.Visibility = ViewStates.Visible;
                    }
                }
            }
        }

        #region Views

        private TextView _forumIndexPageBoardItemListHeader;
        private TextView _forumIndexPageBoardItemIcon;
        private TextView _forumIndexPageBoardItemBoardName;
        private TextView _forumIndexPageBoardItemDecription;
        private ImageViewAsync _forumIndexPageBoardItemPeekPost1Image;
        private TextView _forumIndexPageBoardItemPeekPost1Title;
        private TextView _forumIndexPageBoardItemPeekPost1Date;
        private ImageViewAsync _forumIndexPageBoardItemPeekPost2Image;
        private TextView _forumIndexPageBoardItemPeekPost2Title;
        private TextView _forumIndexPageBoardItemPeekPost2Date;
        private LinearLayout _forumIndexPageBoardItemPeekPostSection;
        private ProgressBar _forumIndexPageBoardItemBoardProgressBar;
        private FrameLayout _forumIndexPageBoardItemRootContainer;

        public TextView ForumIndexPageBoardItemListHeader => _forumIndexPageBoardItemListHeader ?? (_forumIndexPageBoardItemListHeader = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemListHeader));

        public TextView ForumIndexPageBoardItemIcon => _forumIndexPageBoardItemIcon ?? (_forumIndexPageBoardItemIcon = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemIcon));

        public TextView ForumIndexPageBoardItemBoardName => _forumIndexPageBoardItemBoardName ?? (_forumIndexPageBoardItemBoardName = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemBoardName));

        public TextView ForumIndexPageBoardItemDecription => _forumIndexPageBoardItemDecription ?? (_forumIndexPageBoardItemDecription = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemDecription));

        public ImageViewAsync ForumIndexPageBoardItemPeekPost1Image => _forumIndexPageBoardItemPeekPost1Image ?? (_forumIndexPageBoardItemPeekPost1Image = FindViewById<ImageViewAsync>(Resource.Id.ForumIndexPageBoardItemPeekPost1Image));

        public TextView ForumIndexPageBoardItemPeekPost1Title => _forumIndexPageBoardItemPeekPost1Title ?? (_forumIndexPageBoardItemPeekPost1Title = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost1Title));

        public TextView ForumIndexPageBoardItemPeekPost1Date => _forumIndexPageBoardItemPeekPost1Date ?? (_forumIndexPageBoardItemPeekPost1Date = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost1Date));

        public ImageViewAsync ForumIndexPageBoardItemPeekPost2Image => _forumIndexPageBoardItemPeekPost2Image ?? (_forumIndexPageBoardItemPeekPost2Image = FindViewById<ImageViewAsync>(Resource.Id.ForumIndexPageBoardItemPeekPost2Image));

        public TextView ForumIndexPageBoardItemPeekPost2Title => _forumIndexPageBoardItemPeekPost2Title ?? (_forumIndexPageBoardItemPeekPost2Title = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost2Title));

        public TextView ForumIndexPageBoardItemPeekPost2Date => _forumIndexPageBoardItemPeekPost2Date ?? (_forumIndexPageBoardItemPeekPost2Date = FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost2Date));

        public LinearLayout ForumIndexPageBoardItemPeekPostSection => _forumIndexPageBoardItemPeekPostSection ?? (_forumIndexPageBoardItemPeekPostSection = FindViewById<LinearLayout>(Resource.Id.ForumIndexPageBoardItemPeekPostSection));

        public ProgressBar ForumIndexPageBoardItemBoardProgressBar => _forumIndexPageBoardItemBoardProgressBar ?? (_forumIndexPageBoardItemBoardProgressBar = FindViewById<ProgressBar>(Resource.Id.ForumIndexPageBoardItemBoardProgressBar));

        public FrameLayout ForumIndexPageBoardItemRootContainer => _forumIndexPageBoardItemRootContainer ?? (_forumIndexPageBoardItemRootContainer = FindViewById<FrameLayout>(Resource.Id.ForumIndexPageBoardItemRootContainer));

        #endregion
    }
}