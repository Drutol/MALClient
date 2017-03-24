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
        private bool _initialized;

        #region Contructors

        public FavouriteButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public FavouriteButton(Context context) : base(context)
        {

        }

        public FavouriteButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public FavouriteButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

        }

        public FavouriteButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {

        }
        #endregion

        private void Init()
        {
            _favButton = (Context as Activity).LayoutInflater.Inflate(Resource.Layout.FavButton, null) as FrameLayout;
            _favButtonIcon = _favButton.FindViewById<ImageView>(Resource.Id.FavButtonIcon);
            _favButton.Click += FavButtonOnClick;
            AddView(_favButton);
            _initialized = true;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        public void BindModel(FavouriteViewModel model)
        {
            if (!_initialized)
                Init();

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