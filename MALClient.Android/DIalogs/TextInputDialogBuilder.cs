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
using Android.Webkit;
using Android.Widget;
using Com.Orhanobut.Dialogplus;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Activities;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.UserControls;
using MALClient.BBCode;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.DIalogs
{
    public static class TextInputDialogBuilder
    {
        #region GenericTextInput

        private static DialogPlus _changelogDialog;
        private static readonly SemaphoreSlim _semaphoreTextInput = new SemaphoreSlim(0);


        public static async Task<string> BuildInputTextDialog(Context context, string title, string hint)
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

            await _semaphoreTextInput.WaitAsync();

            return textBox.Text;
        }

        private static void CleanupTextInputDialog()
        {
            _semaphoreTextInput.Release();
            _changelogDialog.Dismiss();
            _changelogDialog.Dispose();
        }

        #endregion

        private static DialogPlus _forumTextInputDialog;
        private static readonly SemaphoreSlim _semaphoreForumTextInput = new SemaphoreSlim(0);
        private static bool _success;

        public static async Task<string> BuildForumPostTextInputDialog(Context context, string title, string content)
        {
            var dialogBuilder = DialogPlus.NewDialog(context);
            dialogBuilder.SetGravity((int)GravityFlags.Center);
            var margin = DimensionsHelper.DpToPx(10);
            dialogBuilder.SetMargin(margin, DimensionsHelper.DpToPx(30), margin, margin);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.ForumPostTextDialog));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.Transparent);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => CleanupForumPostTextInputDialog(false)));
            _forumTextInputDialog = dialogBuilder.Create();
            var dialogView = _forumTextInputDialog.HolderView;

            dialogView.FindViewById<TextView>(Resource.Id.ForumPostTextDialogTitle).Text = title;
            var textEditor = dialogView.FindViewById<BBCodeEditor>(Resource.Id.ForumPostTextDialogTextBox);
            textEditor.TextChanged += OnTextChanged;

            dialogView.FindViewById(Resource.Id.ForumPostTextDialogAcceptButton).SetOnClickListener(new OnClickListener(view => CleanupForumPostTextInputDialog(true)));

            _forumTextInputDialog.Show();

            await _semaphoreForumTextInput.WaitAsync();
            if(_success)
                return textEditor.Text;
            _success = false;
            return null;
        }

        private static void OnTextChanged(object sender, string s)
        {
            _forumTextInputDialog.HolderView.FindViewById<WebView>(Resource.Id.ForumPostTextDialogPreview)
                .LoadDataWithBaseURL(null, ResourceLocator.CssManager.WrapWithCss(BBCode.BBCode.ToHtml(s)),
                    "text/html; charset=utf-8", "UTF-8", null);
        }

        private static void CleanupForumPostTextInputDialog(bool success)
        {
            _success = success;
            _semaphoreForumTextInput.Release();
            _forumTextInputDialog.HolderView.FindViewById<BBCodeEditor>(Resource.Id.ForumPostTextDialogTextBox).TextChanged -= OnTextChanged;
            _forumTextInputDialog.Dismiss();
            _forumTextInputDialog.Dispose();
        }

    }
}