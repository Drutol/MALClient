using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Orhanobut.Dialogplus;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Adapters;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.DIalogs
{
    public static class ChangelogDialog
    {
        private static DialogPlus _changelogDialog;

        public static void BuildChangelogDialog(IChangeLogProvider changeLogProvider)
        {
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetGravity((int)(GravityFlags.Top));
            dialogBuilder.SetMargin(DimensionsHelper.DpToPx(40), DimensionsHelper.DpToPx(75), DimensionsHelper.DpToPx(2), 0);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.ChangelogDialog));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.Transparent);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupChangelogDialog));
            _changelogDialog = dialogBuilder.Create();
            var dialogView = _changelogDialog.HolderView;

            dialogView.FindViewById<TextView>(Resource.Id.ChangelogDialogHeader).Text = changeLogProvider.DateWithVersion;
            dialogView.FindViewById(Resource.Id.ChangelogDialogCloseButton).SetOnClickListener(new OnClickListener(view => _changelogDialog.Dismiss()));
            dialogView.FindViewById<LinearLayout>(Resource.Id.ChangelogDialogChangesList).SetAdapter(changeLogProvider.Changelog.GetAdapter(
                (i, s, arg3) =>
                {
                    var view = new TextView(MainActivity.CurrentContext);
                    view.SetTextColor(new Color(ResourceExtension.BrushText));
                    view.Text = $"• {s}";
                    return view;
                }));

            _changelogDialog.Show();
        }

        private static void CleanupChangelogDialog()
        {
            _changelogDialog.Dismiss();
        }
    }
}