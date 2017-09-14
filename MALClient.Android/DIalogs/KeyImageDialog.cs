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
using Com.Orhanobut.Dialogplus;
using FFImageLoading.Drawables;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.UserControls;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Dialogs
{
    public class KeyImageDialog
    {
        #region Singleton

        private static KeyImageDialog _instance;
        private DialogPlus _dialog;

        private KeyImageDialog()
        {

        }

        public static KeyImageDialog Instance =>
            _instance ?? (_instance = new KeyImageDialog());


        #endregion

        public void ShowDialog(Context context, string link)
        {
            var dialogBuilder = DialogPlus.NewDialog(context);
            dialogBuilder.SetGravity((int) GravityFlags.Center);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.AnimeDetailsKeyImageDialog));
            dialogBuilder.SetContentBackgroundResource(global::Android.Resource.Color.Transparent);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));

            _dialog = dialogBuilder.Create();

            var dialogView = _dialog.HolderView;

            //dialogView.FindViewById<ImageViewAsync>(Resource.Id.Image).SetImageBitmap((await ImageService.Instance.LoadUrl(link).AsBitmapDrawableAsync()).Bitmap);
            dialogView.FindViewById<ImageViewAsync>(Resource.Id.Image).Into(AnimeImageExtensions.GetImgUrl(link),null,
                async =>
                {
                    var zoomable = async as ZoomableImageView;
                    zoomable.bmHeight = (async.Drawable as SelfDisposingBitmapDrawable).Bitmap.Height;                   
                    zoomable.bmWidth = (async.Drawable as SelfDisposingBitmapDrawable).Bitmap.Width;
                });
            dialogView.FindViewById(Resource.Id.SaveButton).SetOnClickListener(
                new OnClickListener(view => ViewModelLocator.AnimeDetails.SaveImageCommand.Execute(null)));

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => _dialog.Dismiss()));
            _dialog.Show();
        }
    }
}