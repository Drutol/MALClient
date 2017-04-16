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
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using static Android.Renderscripts.ScriptGroup;

namespace MALClient.Android.UserControls
{
    public class FavouriteItem : FrameLayout
    {
        public bool Initialized { get; private set; }

        public new event EventHandler Click
        {
            add { _rootContainer.Click += value; }
            remove { _rootContainer.Click -= value; }
        }

        private FrameLayout _rootContainer;
        private FavouriteViewModel ViewModel;

        #region Contructors

        public FavouriteItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public FavouriteItem(Context context) : base(context)
        {

        }

        public FavouriteItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public FavouriteItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

        }

        public FavouriteItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {

        }

        #endregion

        private void Init()
        {
            _rootContainer = (Context as Activity).LayoutInflater.Inflate(Resource.Layout.FavouriteItem, null) as FrameLayout;
            AddView(_rootContainer);
            Initialized = true;
        }

        public void BindModel(FavouriteViewModel model,bool fling)
        {
            if (!Initialized)
                Init();

            if (!fling && ViewModel != model)
            {
                ViewModel = model;
                _rootContainer.Tag = model.Data.Wrap();

                FavouriteItemFavButton.BindModel(model);

                if (string.IsNullOrWhiteSpace(model.Data.ImgUrl))
                {
                    FavouriteItemImage.Visibility = ViewStates.Invisible;
                    FavouriteItemImgPlaceholder.Visibility = ViewStates.Gone;
                    FavouriteItemNoImageIcon.Visibility = ViewStates.Visible;
                }
                else
                {
                    FavouriteItemImage.Visibility = ViewStates.Invisible;
                    FavouriteItemImgPlaceholder.Visibility = ViewStates.Gone;
                    FavouriteItemImage.Into(model.Data.ImgUrl, null, img =>
                    {
                        img.HandleScaling();
                    });
                    FavouriteItemNoImageIcon.Visibility = ViewStates.Gone;
                }

                FavouriteItemName.Text = model.Data.Name;
                FavouriteItemRole.Text = model.Data.Notes;
            }
            else if (fling)
            {
                if (string.IsNullOrWhiteSpace(model.Data.ImgUrl))
                {
                    FavouriteItemImage.Visibility = ViewStates.Invisible;
                    FavouriteItemImgPlaceholder.Visibility = ViewStates.Gone;
                    FavouriteItemNoImageIcon.Visibility = ViewStates.Visible;
                }
                else
                {
                    FavouriteItemNoImageIcon.Visibility = ViewStates.Gone;
                    if (FavouriteItemImage.IntoIfLoaded(model.Data.ImgUrl))
                    {
                        FavouriteItemImage.Visibility = ViewStates.Visible;
                        FavouriteItemImgPlaceholder.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        FavouriteItemImage.Visibility = ViewStates.Invisible;
                        FavouriteItemImgPlaceholder.Visibility = ViewStates.Visible;
                    }

                }

                FavouriteItemName.Text = model.Data.Name;
                FavouriteItemRole.Text = model.Data.Notes;
            }

        }

        #region Views

        private ProgressBar _favouriteItemImgPlaceholder;
        private ImageViewAsync _favouriteItemImage;
        private ImageView _favouriteItemNoImageIcon;
        private FavouriteButton _favouriteItemFavButton;
        private TextView _favouriteItemName;
        private TextView _favouriteItemRole;

        public ProgressBar FavouriteItemImgPlaceholder => _favouriteItemImgPlaceholder ?? (_favouriteItemImgPlaceholder = FindViewById<ProgressBar>(Resource.Id.FavouriteItemImgPlaceholder));

        public ImageViewAsync FavouriteItemImage => _favouriteItemImage ?? (_favouriteItemImage = FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage));

        public ImageView FavouriteItemNoImageIcon => _favouriteItemNoImageIcon ?? (_favouriteItemNoImageIcon = FindViewById<ImageView>(Resource.Id.FavouriteItemNoImageIcon));

        public FavouriteButton FavouriteItemFavButton => _favouriteItemFavButton ?? (_favouriteItemFavButton = FindViewById<FavouriteButton>(Resource.Id.FavouriteItemFavButton));

        public TextView FavouriteItemName => _favouriteItemName ?? (_favouriteItemName = FindViewById<TextView>(Resource.Id.FavouriteItemName));

        public TextView FavouriteItemRole => _favouriteItemRole ?? (_favouriteItemRole = FindViewById<TextView>(Resource.Id.FavouriteItemRole));



        #endregion
    }
}