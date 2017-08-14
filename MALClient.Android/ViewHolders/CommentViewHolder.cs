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
using FFImageLoading.Views;

namespace MALClient.Android.ViewHolders
{
    public class CommentViewHolder
    {
        private readonly View _view;

        public CommentViewHolder(View view)
        {
            _view = view;
        }

        private ImageViewAsync _profilePageGeneralTabCommentItemUserImg;
        private FrameLayout _profilePageGeneralTabCommentItemImgButton;
        private TextView _profilePageGeneralTabCommentItemUsername;
        private TextView _profilePageGeneralTabCommentItemDate;
        private TextView _profilePageGeneralTabCommentItemContent;
        private ImageViewAsync _image;
        private Button _profilePageGeneralTabCommentItemDeleteButton;
        private Button _profilePageGeneralTabCommentItemConvButton;

        public ImageViewAsync ProfilePageGeneralTabCommentItemUserImg => _profilePageGeneralTabCommentItemUserImg ?? (_profilePageGeneralTabCommentItemUserImg = _view.FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabCommentItemUserImg));

        public FrameLayout ProfilePageGeneralTabCommentItemImgButton => _profilePageGeneralTabCommentItemImgButton ?? (_profilePageGeneralTabCommentItemImgButton = _view.FindViewById<FrameLayout>(Resource.Id.ProfilePageGeneralTabCommentItemImgButton));

        public TextView ProfilePageGeneralTabCommentItemUsername => _profilePageGeneralTabCommentItemUsername ?? (_profilePageGeneralTabCommentItemUsername = _view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentItemUsername));

        public TextView ProfilePageGeneralTabCommentItemDate => _profilePageGeneralTabCommentItemDate ?? (_profilePageGeneralTabCommentItemDate = _view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentItemDate));

        public TextView ProfilePageGeneralTabCommentItemContent => _profilePageGeneralTabCommentItemContent ?? (_profilePageGeneralTabCommentItemContent = _view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentItemContent));

        public ImageViewAsync Image => _image ?? (_image = _view.FindViewById<ImageViewAsync>(Resource.Id.Image));

        public Button ProfilePageGeneralTabCommentItemDeleteButton => _profilePageGeneralTabCommentItemDeleteButton ?? (_profilePageGeneralTabCommentItemDeleteButton = _view.FindViewById<Button>(Resource.Id.ProfilePageGeneralTabCommentItemDeleteButton));

        public Button ProfilePageGeneralTabCommentItemConvButton => _profilePageGeneralTabCommentItemConvButton ?? (_profilePageGeneralTabCommentItemConvButton = _view.FindViewById<Button>(Resource.Id.ProfilePageGeneralTabCommentItemConvButton));
        }
    }