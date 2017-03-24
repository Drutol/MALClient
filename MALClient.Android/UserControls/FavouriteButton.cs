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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.UserControls
{
    public class FavouriteButton : FrameLayout
    {
        private readonly List<Binding> Bindings = new List<Binding>();
        private FrameLayout _favButton;
        private ImageView _favButtonIcon;
        private FavouriteViewModel ViewModel;

        #region Contructors

        public FavouriteButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        public FavouriteButton(Context context) : base(context)
        {
            Init();
        }

        public FavouriteButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public FavouriteButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public FavouriteButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }
        #endregion

        private void Init()
        {
            _favButton = (Context as Activity).LayoutInflater.Inflate(Resource.Layout.FavButton, this) as FrameLayout;
            _favButtonIcon = _favButton.FindViewById<ImageView>(Resource.Id.FavButtonIcon);
            _favButton.Click += FavButtonOnClick;
            AddView(_favButton);
        }

        public void BindModel(FavouriteViewModel model)
        {
            ViewModel = model;

            Bindings.Add(this.SetBinding(() => ViewModel.IsFavourite).WhenSourceChanges(() =>
            {
                if (ViewModel.IsFavourite)
                {
                    _favButton.SetBackgroundResource(Resource.Color.AccentColourDark);
                    _favButtonIcon.SetImageResource(Resource.Drawable.icon_unfavourite);
                }
                else
                {
                    _favButton.SetBackgroundResource(Resource.Color.BrushOpaqueTextView);
                    _favButtonIcon.SetImageResource(Resource.Drawable.icon_favourite);
                }
            }));
            Bindings.Add(
                this.SetBinding(() => ViewModel.IsFavouriteButtonEnabled,
                    () => Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

        }


        private void FavButtonOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel?.ToggleFavouriteCommand.Execute(null);
        }

        protected override void Dispose(bool disposing)
        {
            Bindings.ForEach(binding => binding.Detach());
            base.Dispose(disposing);
        }
    }
}