using System;
using System.Collections.Concurrent;
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
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.ScrappedDetails;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using static Android.Renderscripts.ScriptGroup;

namespace MALClient.Android.UserControls
{
    public class FavouriteItem : FrameLayout
    {
        private readonly bool _ignoreSmallItemsSetting;
        public bool Initialized { get; private set; }

        public new event EventHandler Click
        {
            add { _rootContainer.Click += value; }
            remove { _rootContainer.Click -= value; }
        }

        private FrameLayout _rootContainer;
        private FavouriteViewModel ViewModel;

        private static readonly ConcurrentDictionary<string, ImageView.ScaleType> ScaleTypes = new ConcurrentDictionary<string, ImageView.ScaleType>();

        #region Contructors

        public FavouriteItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public FavouriteItem(Context context,bool ignoreSmallItemsSetting = false) : base(context)
        {
            _ignoreSmallItemsSetting = ignoreSmallItemsSetting;
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

            if (!_ignoreSmallItemsSetting && Settings.MakeGridItemsSmaller)
            {
                FavouriteItemUpperSection.LayoutParameters.Height = DimensionsHelper.DpToPx(146);
                FavouriteItemLowerSection.LayoutParameters.Height = DimensionsHelper.DpToPx(45);

                FavouriteItemFavButton.LayoutParameters.Width =
                    FavouriteItemFavButton.LayoutParameters.Height = DimensionsHelper.DpToPx(35);

                FavouriteItemName.SetTextSize(ComplexUnitType.Sp, 12);
                FavouriteItemRole.SetTextSize(ComplexUnitType.Sp, 12);
            }

            Initialized = true;
        }

        public void BindModel(FavouriteViewModel model,bool fling)
        {
            if (!Initialized)
                Init();

            if (!fling && ViewModel != model)
            {
                ViewModel = model;
                _rootContainer.Tag = model.Wrap();

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
                    if (ScaleTypes.TryGetValue(model.Data.ImgUrl, out ImageView.ScaleType type))
                    {
                        FavouriteItemImage.SetScaleType(type);
                        FavouriteItemImage.Into(model.Data.ImgUrl);
                    }
                    else
                    {
                        FavouriteItemImage.Into(model.Data.ImgUrl, null, img =>
                        {
                            ScaleTypes[model.Data.ImgUrl] = img.HandleScaling();
                        });
                    }
  
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
                        if (ScaleTypes.TryGetValue(model.Data.ImgUrl, out ImageView.ScaleType type))
                            FavouriteItemImage.SetScaleType(type);
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

        }

        #region Views

        private ProgressBar _favouriteItemImgPlaceholder;
        private ImageView _favouriteItemNoImageIcon;
        private ImageViewAsync _favouriteItemImage;
        private FavouriteButton _favouriteItemFavButton;
        private RelativeLayout _favouriteItemUpperSection;
        private TextView _favouriteItemName;
        private TextView _favouriteItemRole;
        private LinearLayout _favouriteItemLowerSection;

        public ProgressBar FavouriteItemImgPlaceholder => _favouriteItemImgPlaceholder ?? (_favouriteItemImgPlaceholder = FindViewById<ProgressBar>(Resource.Id.FavouriteItemImgPlaceholder));

        public ImageView FavouriteItemNoImageIcon => _favouriteItemNoImageIcon ?? (_favouriteItemNoImageIcon = FindViewById<ImageView>(Resource.Id.FavouriteItemNoImageIcon));

        public ImageViewAsync FavouriteItemImage => _favouriteItemImage ?? (_favouriteItemImage = FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage));

        public FavouriteButton FavouriteItemFavButton => _favouriteItemFavButton ?? (_favouriteItemFavButton = FindViewById<FavouriteButton>(Resource.Id.FavouriteItemFavButton));

        public RelativeLayout FavouriteItemUpperSection => _favouriteItemUpperSection ?? (_favouriteItemUpperSection = FindViewById<RelativeLayout>(Resource.Id.FavouriteItemUpperSection));

        public TextView FavouriteItemName => _favouriteItemName ?? (_favouriteItemName = FindViewById<TextView>(Resource.Id.FavouriteItemName));

        public TextView FavouriteItemRole => _favouriteItemRole ?? (_favouriteItemRole = FindViewById<TextView>(Resource.Id.FavouriteItemRole));

        public LinearLayout FavouriteItemLowerSection => _favouriteItemLowerSection ?? (_favouriteItemLowerSection = FindViewById<LinearLayout>(Resource.Id.FavouriteItemLowerSection));



        #endregion
    }
}