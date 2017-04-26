using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
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

        private static DialogPlus _textInputDialog;
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
            _textInputDialog = dialogBuilder.Create();
            var dialogView = _textInputDialog.HolderView;

            dialogView.FindViewById<TextView>(Resource.Id.TextInputDialogTitle).Text = title;
            var textBox = dialogView.FindViewById<EditText>(Resource.Id.TextInputDialogTextBox);
            textBox.Hint = hint;

            dialogView.FindViewById(Resource.Id.TextInputDialogAcceptButton).SetOnClickListener(new OnClickListener(view => CleanupTextInputDialog()));

            _textInputDialog.Show();

            await _semaphoreTextInput.WaitAsync();

            return textBox.Text;
        }

        private static void CleanupTextInputDialog()
        {
            if(_textInputDialog == null)
                return;

            _semaphoreTextInput.Release();
            _textInputDialog.Dismiss();
            _textInputDialog.Dispose();
            _textInputDialog = null;
        }

        #endregion

        public enum ForumPostTextInputContext
        {
            Reply,
            Edit,
        }

        private static DialogPlus _forumTextInputDialog;
        private static readonly SemaphoreSlim _semaphoreForumTextInput = new SemaphoreSlim(0);
        private static bool _success;

        public static async Task<string> BuildForumPostTextInputDialog(Context context, ForumPostTextInputContext displayContext, string content)
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

            var acceptButton = dialogView.FindViewById<Button>(Resource.Id.ForumPostTextDialogAcceptButton);
            switch (displayContext)
            {
                case ForumPostTextInputContext.Reply:
                    dialogView.FindViewById<TextView>(Resource.Id.ForumPostTextDialogTitle).Text = "New Reply";
                    acceptButton.SetCompoundDrawablesWithIntrinsicBounds(context.Resources.GetDrawable(Resource.Drawable.icon_send, context.Theme),null,null,null);
                    acceptButton.Text = "Send";
                    break;
                case ForumPostTextInputContext.Edit:
                    dialogView.FindViewById<TextView>(Resource.Id.ForumPostTextDialogTitle).Text = "Edit Message";
                    acceptButton.Text = "Edit";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(displayContext), displayContext, null);
            }
            acceptButton.SetOnClickListener(new OnClickListener(view => CleanupForumPostTextInputDialog(true)));

            var textEditor = new BBCodeEditor(context);        
            textEditor.TextChanged += OnTextChanged;
            textEditor.Text = content;

            dialogView.FindViewById<LinearLayout>(Resource.Id.ForumPostTextDialogInputSection).AddView(textEditor, 0);

            dialogView.FindViewById<WebView>(Resource.Id.ForumPostTextDialogPreview).SetBackgroundColor(Color.Transparent);

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
            if(_forumTextInputDialog==null)
                return;

            _success = success;
            _semaphoreForumTextInput.Release();
            _forumTextInputDialog.Dismiss();
            _forumTextInputDialog.Dispose();
            _forumTextInputDialog = null;
        }

    }
}