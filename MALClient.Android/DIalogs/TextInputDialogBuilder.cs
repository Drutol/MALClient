using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Orhanobut.Dialogplus;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Activities;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.DIalogs
{
    public static class TextInputDialogBuilder
    {
        private static DialogPlus _changelogDialog;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public static async Task<string> BuildInputTextDialog(Context context,string title,string hint)
        {
            var dialogBuilder = DialogPlus.NewDialog(context);
            dialogBuilder.SetGravity((int)(GravityFlags.Center));
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.TextInputDialog));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.Transparent);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupTextInputDialog));
            _changelogDialog = dialogBuilder.Create();
            var dialogView = _changelogDialog.HolderView;

            dialogView.FindViewById<TextView>(Resource.Id.TextInputDialogTitle).Text = title;
            var textBox = dialogView.FindViewById<EditText>(Resource.Id.TextInputDialogTextBox);
            textBox.Hint = hint;

            dialogView.FindViewById(Resource.Id.TextInputDialogAcceptButton).SetOnClickListener(new OnClickListener(view => CleanupTextInputDialog()));

            _changelogDialog.Show();

            await _semaphore.WaitAsync();

            return textBox.Text;
        }

        private static void CleanupTextInputDialog()
        {
            _semaphore.Release();
            _changelogDialog.Dismiss();            
            _changelogDialog.Dispose();
        }
    }
}