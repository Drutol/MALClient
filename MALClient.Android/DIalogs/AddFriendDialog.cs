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
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MALClient.Android.Fragments.ProfilePageFragments;
using MALClient.Android.Listeners;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Dialogs
{
    public class AddFriendDialog
    {
        #region Singleton

        private static AddFriendDialog _instance;
        private DialogPlus _dialog;

        private AddFriendDialog()
        {

        }

        public static AddFriendDialog Instance => 
            _instance ?? (_instance = new AddFriendDialog());


        #endregion

        public void ShowDialog(Context context,ProfileData data,ProfilePageGeneralTabFragment parent)
        {
            var dialogBuilder = DialogPlus.NewDialog(context);
            dialogBuilder.SetGravity((int)GravityFlags.Center);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.FriendRequestDialog));
            dialogBuilder.SetContentBackgroundResource(global::Android.Resource.Color.Transparent);
            dialogBuilder.SetCancelable(true);

            _dialog = dialogBuilder.Create();

            var dialogView = _dialog.HolderView;

            dialogView.FindViewById<ImageViewAsync>(Resource.Id.Image).Into(data.User.ImgUrl,new CircleTransformation());
            
            dialogView.FindViewById(Resource.Id.SubmitButton).SetOnClickListener(new OnClickListener(view =>
            {
                ViewModelLocator.ProfilePage.SendFriendRequestCommand.Execute(dialogView
                    .FindViewById<EditText>(Resource.Id.TextBox).Text);
                _dialog.Dismiss();
                parent.ProfilePageGeneralTabSendRequestButton.Visibility = ViewStates.Gone;
            }));
            _dialog.Show();
        }
    }
}