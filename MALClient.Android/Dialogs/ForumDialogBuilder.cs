using System;
using System.Collections.Generic;
using System.Globalization;
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
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Dialogs
{
    public static class ForumDialogBuilder
    {
        enum DialogResult
        {
            Cancel,
            LastPage,
            FirstPage,
            InputPage,
        }

        private static DialogPlus _goPageDialog;
        private static readonly SemaphoreSlim GoPageSemaphore = new SemaphoreSlim(0);
        private static int? _result;

        /// <summary>
        /// -1 for first page, -2 for last page , null for no choice , anything else equals page number
        /// </summary>
        public static async Task<int?> BuildGoPageDialog(Context context)
        {
            _result = null;
            var dialogBuilder = DialogPlus.NewDialog(context);
            dialogBuilder.SetGravity((int)GravityFlags.Center);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.ForumGoToPageDialog));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.Transparent);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => CleanupGoPageDialog(DialogResult.Cancel)));
            _goPageDialog = dialogBuilder.Create();
            var dialogView = _goPageDialog.HolderView;

            dialogView.FindViewById(Resource.Id.ForumGoToPageDialogAcceptButton).SetOnClickListener(new OnClickListener(view => CleanupGoPageDialog(DialogResult.InputPage)));
            dialogView.FindViewById(Resource.Id.ForumGoToPageDialogFirstPageButton).SetOnClickListener(new OnClickListener(view => CleanupGoPageDialog(DialogResult.FirstPage)));
            dialogView.FindViewById(Resource.Id.ForumGoToPageDialogLastPageButton).SetOnClickListener(new OnClickListener(view => CleanupGoPageDialog(DialogResult.LastPage)));

            _goPageDialog.Show();

            await GoPageSemaphore.WaitAsync();

            return _result;
        }

        private static void CleanupGoPageDialog(DialogResult success)
        {
            if(_goPageDialog == null)
                return;
            switch (success)
            {
                case DialogResult.Cancel:
                    _result = null;
                    break;
                case DialogResult.LastPage:
                    _result = -2;
                    break;
                case DialogResult.FirstPage:
                    _result = -1;
                    break;
                case DialogResult.InputPage:
                    var text = _goPageDialog.HolderView.FindViewById<EditText>(Resource.Id.ForumGoToPageDialogTextBox)
                        .Text;
                    if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int page))
                        _result = page;
                    else
                        _result = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(success), success, null);
            }

            GoPageSemaphore.Release();
            _goPageDialog.Dismiss();
            _goPageDialog.Dispose();
            _goPageDialog = null;
        }
    }
}