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
        public static void BuildStatusDialog(IAnimeData model,bool anime,Action<AnimeStatus> action = null)
        {
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetAdapter(new StatusDialogAdapter(MainActivity.CurrentContext,
                !anime, model.IsRewatching, model.MyStatus));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.BrushFlyoutBackground);
            dialogBuilder.SetOnItemClickListener(new EnumDialogListener<AnimeStatus>
            {
                OnItemClickAction = (d, status) =>
                {
                    if(action == null)
                        model.MyStatus = (int)status;
                    else
                        action.Invoke(status);
                    CleanupStatusDialog();
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
        private static Action<AnimeItemViewModel,string> _watchedDialogAction;

        public static void BuildWatchedDialog(AnimeItemViewModel ViewModel,Action<AnimeItemViewModel, string> action = null,bool volumes = false)
        {
            _watchedDialogContext = ViewModel;
            _watchedDialogAction = action;
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.AnimeItemWatchedDialog));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.BrushFlyoutBackground);
            dialogBuilder.SetOnDismissListener(new DialogDismissedListener(CleanupWatchedDialog));
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupWatchedDialog));
            _watchedDialog = dialogBuilder.Create();
            var view = _watchedDialog.HolderView;


            view.FindViewById<ImageButton>(Resource.Id.AnimeItemWatchedDialogAcceptButton)
                .SetCommand("Click", new RelayCommand(
                    () =>
                    {
                        if (!volumes)
                            ViewModel.OnFlyoutEpsKeyDown.Execute(null);
                        else
                            action.Invoke(_watchedDialogContext, _watchedDialog.HolderView.FindViewById<TextView>(
                                Resource.Id.AnimeItemWatchedDialogTextInput).Text);
                        CleanupWatchedDialog();
                    }));

            var grid = view.FindViewById<GridView>(Resource.Id.AnimeItemWatchedDialogEpisodesGridView);
            if (volumes)
            {

                grid.Adapter = new WatchedDialogAdapter(MainActivity.CurrentContext, ViewModel.MyVolumes,
                    ViewModel.AllVolumes);
                view.FindViewById<TextView>(Resource.Id.AnimeItemWatchedDialogTitleTextView).Text = "Read volumes";
            }
            else
            {
                var input = view.FindViewById<EditText>(Resource.Id.AnimeItemWatchedDialogTextInput);
                _watchedDialogBindings.Add(new Binding<string, string>(ViewModel, () => ViewModel.WatchedEpsInput, input,
                    () => input.Text, BindingMode.TwoWay));
                grid.Adapter = new WatchedDialogAdapter(MainActivity.CurrentContext, ViewModel.MyEpisodesFocused,
                    ViewModel.AllEpisodesFocused);
                view.FindViewById<TextView>(Resource.Id.AnimeItemWatchedDialogTitleTextView).Text =
                    ViewModel.WatchedEpsLabel;
            }

            grid.ItemClick += GridOnItemClick;
            _watchedDialog.Show();
        }

        private static void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            if (_watchedDialogAction == null)
            {
                _watchedDialogContext.WatchedEpsInput = itemClickEventArgs.Id.ToString();
                _watchedDialogContext.OnFlyoutEpsKeyDown.Execute(null);
            }
            else
            {
                _watchedDialogAction.Invoke(_watchedDialogContext,itemClickEventArgs.Id.ToString());
            }


            CleanupWatchedDialog();
        }

        private static void CleanupWatchedDialog()
        {
            _watchedDialogBindings.ForEach(binding => binding.Detach());
            _watchedDialogBindings = new List<Binding>();
            _watchedDialog?.Dismiss();
            _watchedDialog = null;
            _watchedDialogContext = null;
            _watchedDialogAction = null;
        }


        private static DialogPlus _scoreDialog;
        public static void BuildScoreDialog(IAnimeData model,Action<int> action = null)
        {
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetAdapter(new ScoreDialogAdapter(MainActivity.CurrentContext,
                AnimeItemViewModel.ScoreFlyoutChoices,(int)model.MyScore));
            dialogBuilder.SetContentBackgroundResource(Resource.Color.BrushFlyoutBackground);
            dialogBuilder.SetOnItemClickListener(new IntegerDialogListener()
            {
                OnItemClickAction = (d, score) =>
                {
                    if(action == null)
                        model.MyScore = score;
                    else
                        action.Invoke(score);
                    CleanupScoreDialog();
                }
            });
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupScoreDialog));
            _scoreDialog = dialogBuilder.Create();
            _scoreDialog.Show();
        }

        private static void CleanupScoreDialog()
        {
            _scoreDialog?.Dismiss();
            _scoreDialog = null;
        }
    }
}