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
        public new event EventHandler Click
        {
            add { _rootContainer.Click += value; }
            remove { _rootContainer.Click -= value; }
        }

        private FrameLayout _rootContainer;
        private List<Binding> Bindings = new List<Binding>();
        private FavouriteViewModel ViewModel;

        #region Contructors

        public FavouriteItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        public FavouriteItem(Context context) : base(context)
        {
            Init();
        }

        public FavouriteItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public FavouriteItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public FavouriteItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }

        #endregion

        private void Init()
        {
            _rootContainer = (Context as Activity).LayoutInflater.Inflate(Resource.Layout.FavouriteItem, this) as FrameLayout;
            AddView(_rootContainer);
        }

        public void BindModel(FavouriteViewModel model)
        {
            ViewModel = model;

            FavouriteItemFavButton.BindModel(model);

            FavouriteItemImage.Into(model.Data.ImgUrl);
            FavouriteItemName.Text = model.Data.Name;
            FavouriteItemRole.Text = model.Data.Notes;

            FavouriteItemImage.SetScaleType(model.Data.Type == FavouriteType.Person
                ? ImageView.ScaleType.FitCenter
                : ImageView.ScaleType.CenterCrop);
        }

        #region Views

        private FavouriteButton _favouriteItemFavButton;
        private ImageViewAsync _favouriteItemImage;
        private ImageView _favouriteItemImagePlaceholder;
        private TextView _favouriteItemName;
        private TextView _favouriteItemRole;

        public FavouriteButton FavouriteItemFavButton => _favouriteItemFavButton ?? (_favouriteItemFavButton = _rootContainer.FindViewById<FavouriteButton>(Resource.Id.FavouriteItemFavButton));

        public ImageViewAsync FavouriteItemImage => _favouriteItemImage ?? (_favouriteItemImage = _rootContainer.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage));

        public ImageView FavouriteItemImagePlaceholder => _favouriteItemImagePlaceholder ?? (_favouriteItemImagePlaceholder = _rootContainer.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImagePlaceholder));

        public TextView FavouriteItemName => _favouriteItemName ?? (_favouriteItemName = _rootContainer.FindViewById<TextView>(Resource.Id.FavouriteItemName));

        public TextView FavouriteItemRole => _favouriteItemRole ?? (_favouriteItemRole = _rootContainer.FindViewById<TextView>(Resource.Id.FavouriteItemRole));

        #endregion
    }
}