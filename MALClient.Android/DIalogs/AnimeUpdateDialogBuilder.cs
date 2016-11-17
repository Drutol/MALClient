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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Adapters.DialogAdapters;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Models.Models.Library;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.DIalogs
{
    public static class AnimeUpdateDialogBuilder
    {
        private static DialogPlus _statusDialog;
        public static void BuildStatusDialog(IAnimeData model,bool anime)
        {
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetAdapter(new StatusDialogAdapter(MainActivity.CurrentContext,
                !anime, model.IsRewatching, model.MyStatus));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.BrushFlyoutBackground);
            dialogBuilder.SetOnItemClickListener(new EnumDialogListener<AnimeStatus>
            {
                OnItemClickAction = (d, status) =>
                {
                    model.MyStatus = (int)status;
                    d.Dismiss();
                }
            });
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupStatusDialog));
            _statusDialog = dialogBuilder.Create();
            _statusDialog.Show();
        }

        private static void CleanupStatusDialog()
        {
            _statusDialog?.Dismiss();
            _statusDialog = null;
        }

        private static List<Binding> _watchedDialogBindings = new List<Binding>();
        private static DialogPlus _watchedDialog;
        private static AnimeItemViewModel _watchedDialogContext;

        public static void BuildWatchedDialog(AnimeItemViewModel ViewModel)
        {
            _watchedDialogContext = ViewModel;
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.AnimeItemWatchedDialog));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.BrushFlyoutBackground);
            dialogBuilder.SetOnDismissListener(new DialogDismissedListener(CleanupWatchedDialog));
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupWatchedDialog));
            _watchedDialog = dialogBuilder.Create();
            var view = _watchedDialog.HolderView;

            var input = view.FindViewById<EditText>(Resource.Id.AnimeItemWatchedDialogTextInput);
            _watchedDialogBindings.Add(new Binding<string, string>(ViewModel, () => ViewModel.WatchedEpsInput, input,
                () => input.Text, BindingMode.TwoWay));
            view.FindViewById<ImageButton>(Resource.Id.AnimeItemWatchedDialogAcceptButton)
                .SetCommand("Click", new RelayCommand(
                    () =>
                    {
                        ViewModel.OnFlyoutEpsKeyDown.Execute(null);
                        CleanupWatchedDialog();
                    }));
            var grid = view.FindViewById<GridView>(Resource.Id.AnimeItemWatchedDialogEpisodesGridView);
            grid.Adapter = new WatchedDialogAdapter(MainActivity.CurrentContext, ViewModel.MyEpisodesFocused,
                ViewModel.AllEpisodesFocused);
            grid.ItemClick += GridOnItemClick;
            view.FindViewById<TextView>(Resource.Id.AnimeItemWatchedDialogTitleTextView).Text =
                ViewModel.WatchedEpsLabel;

            _watchedDialog.Show();
        }

        private static void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            _watchedDialogContext.WatchedEpsInput = itemClickEventArgs.Id.ToString();
            _watchedDialogContext.OnFlyoutEpsKeyDown.Execute(null);
            CleanupWatchedDialog();
        }

        private static void CleanupWatchedDialog()
        {
            _watchedDialogBindings.ForEach(binding => binding.Detach());
            _watchedDialogBindings = new List<Binding>();
            _watchedDialog?.Dismiss();
            _watchedDialog = null;
            _watchedDialogContext = null;
        }
    }
}