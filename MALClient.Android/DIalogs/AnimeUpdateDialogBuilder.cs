using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Orhanobut.Dialogplus;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.DialogAdapters;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.Models.Models.Library;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;
using Orientation = Android.Content.Res.Orientation;

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
            dialogBuilder.SetContentBackgroundResource(ResourceExtension.BrushFlyoutBackgroundRes);
            dialogBuilder.SetOnItemClickListener(new EnumDialogListener<AnimeStatus>
            {
                OnItemClickAction = (d, status) =>
                {
                    if(action == null)
                        model.MyStatus = status;
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
            ViewModelLocator.NavMgr.ResetOneTimeMainOverride();
            _statusDialog?.Dismiss();
            _statusDialog = null;
        }

        private static List<Binding> _watchedDialogBindings = new List<Binding>();
        private static DialogPlus _watchedDialog;
        private static AnimeItemViewModel _watchedDialogContext;
        private static Action<AnimeItemViewModel,string> _watchedDialogAction;

        public static void BuildWatchedDialog(AnimeItemViewModel viewModel, Action<AnimeItemViewModel, string> action = null,bool volumes = false)
        {
            try
            {
                _watchedDialogContext = viewModel;
                _watchedDialogAction = action;
                var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
                dialogBuilder.SetGravity((int)GravityFlags.Top);
                dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.AnimeItemWatchedDialog));
                dialogBuilder.SetContentBackgroundResource(ResourceExtension.BrushFlyoutBackgroundRes);
                dialogBuilder.SetOnDismissListener(new DialogDismissedListener(CleanupWatchedDialog));
                dialogBuilder.SetContentBackgroundResource(ResourceExtension.AccentColourDarkRes);
                _watchedDialog = dialogBuilder.Create();
                var view = _watchedDialog.HolderView;


                view.FindViewById<ImageButton>(Resource.Id.AnimeItemWatchedDialogAcceptButton)
                    .SetOnClickListener(new OnClickListener(
                        v =>
                        {
                            if (action == null)
                                viewModel.OnFlyoutEpsKeyDown.Execute(null);
                            else
                                action.Invoke(_watchedDialogContext, _watchedDialog.HolderView.FindViewById<TextView>(
                                    Resource.Id.AnimeItemWatchedDialogTextInput).Text);
                            CleanupWatchedDialog();
                        }));

                var grid = view.FindViewById<GridView>(Resource.Id.AnimeItemWatchedDialogEpisodesGridView);

                if (volumes)
                {
                    grid.Adapter = new WatchedDialogAdapter(MainActivity.CurrentContext, viewModel.MyVolumes,
                        viewModel.AllVolumes, null);
                    view.FindViewById<TextView>(Resource.Id.AnimeItemWatchedDialogTitleTextView).Text = "Read volumes";
                    grid.Post(() =>
                    {
                        try
                        {
                            grid.SetSelection(
                                GetRightMostIndex(
                                    (grid.Adapter as WatchedDialogAdapter).Items.IndexOf(viewModel.MyVolumes)));
                        }
                        catch (Exception)
                        {
                            //welp
                        }
                    });
                }
                else
                {
                    int? currentEp = null;
                    if (viewModel.Airing)
                    {
                        if (ResourceLocator.AiringInfoProvider.TryGetCurrentEpisode(viewModel.Id, out int ep, DateTime.Today))
                            currentEp = ep;
                    }
                    grid.Adapter = new WatchedDialogAdapter(MainActivity.CurrentContext, viewModel.MyEpisodes,
                        viewModel.AllEpisodes,currentEp);
                    view.FindViewById<TextView>(Resource.Id.AnimeItemWatchedDialogTitleTextView).Text =
                        viewModel.ParentAbstraction.RepresentsAnime ? "Watched episodes" : "Read chapters";
                    grid.Post(() =>
                    {
                        try
                        {
                            grid.SetSelection(
                                GetRightMostIndex(
                                    (grid.Adapter as WatchedDialogAdapter).Items.IndexOf(viewModel.MyEpisodes)));
                        }
                        catch (Exception)
                        {
                            //welp
                        }
                    });
                }



                if (grid.Adapter.Count > 16 && MainActivity.CurrentContext.Resources.Configuration.Orientation ==
                    Orientation.Portrait)
                {
                    grid.LayoutParameters.Height = DimensionsHelper.DpToPx(180);

                }

                if (action == null)
                {
                    var input = view.FindViewById<EditText>(Resource.Id.AnimeItemWatchedDialogTextInput);
                    _watchedDialogBindings.Add(new Binding<string, string>(viewModel, () => viewModel.WatchedEpsInput, input,
                        () => input.Text, BindingMode.TwoWay));
                }


                grid.ItemClick += GridOnItemClick;
                _watchedDialog.Show();
                MainActivity.CurrentContext.DialogToCollapseOnBack = _watchedDialog;

                int GetRightMostIndex(int source)
                {
                    var rest = (source+1) % grid.NumColumns;
                    source -= rest;
                    return source-1;
                }
            }
            catch (Exception e)
            {
               
            }
            
        }

        private static void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            if(_watchedDialog == null)
                return;

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
            if (_watchedDialog != null)
            {
                AndroidUtilities.HideKeyboard();
                _watchedDialog.Dismiss();
            }
            _watchedDialogBindings.ForEach(binding => binding.Detach());
            _watchedDialogBindings = new List<Binding>();
            _watchedDialog = null;
            _watchedDialogContext = null;
            _watchedDialogAction = null;
            MainActivity.CurrentContext.DialogToCollapseOnBack = null;
        }


        private static DialogPlus _scoreDialog;
        public static void BuildScoreDialog(IAnimeData model,Action<float> action = null)
        {
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetAdapter(new ScoreDialogAdapter(MainActivity.CurrentContext,
                AnimeItemViewModel.ScoreFlyoutChoices,model.MyScore));
            dialogBuilder.SetContentBackgroundResource(ResourceExtension.BrushFlyoutBackgroundRes);
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
            ViewModelLocator.NavMgr.ResetOneTimeMainOverride();
            _scoreDialog?.Dismiss();
            _scoreDialog = null;
        }

        private static AnimeDetailsPageViewModel _tagsDialogContext;
        private static DialogPlus _tagsDialog;
        private static List<Binding> _tagsDialogBindings;
        public static void BuildTagDialog(AnimeDetailsPageViewModel viewModel)
        {
            _tagsDialogBindings = new List<Binding>();
            _tagsDialogContext = viewModel;
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.AnimeTagsDialog));
            dialogBuilder.SetOnDismissListener(new DialogDismissedListener(CleanupTagsDialog));
            dialogBuilder.SetOnBackPressListener(new OnBackPressListener(CleanupTagsDialog));
            dialogBuilder.SetGravity((int) GravityFlags.Top);
            _tagsDialog = dialogBuilder.Create();
            var view = _tagsDialog.HolderView;

            var list = view.FindViewById<ListView>(Resource.Id.AnimeTagsDialogList);
            list.EmptyView = view.FindViewById(Resource.Id.AnimeTagsDialogEmptyNotice);
            list.Adapter = viewModel.MyTags.GetAdapter(GetTagItem);
            var editBox = view.FindViewById<AutoCompleteTextView>(Resource.Id.AnimeTagsDialogEditBox);
            editBox.Threshold = 1;
            editBox.Adapter = new ArrayAdapter<string>(MainActivity.CurrentContext,global::Android.Resource.Layout.SelectDialogItem,ViewModelLocator.GeneralMain.SearchHints);
            //editBox.SetOnEditorActionListener(new OnEditorActionListener(action =>
            //{
            //    if(action == ImeAction.Done)
            //        viewModel.AddTagCommand.Execute(null);
            //}));
            editBox.AddTextChangedListener(new OnTextEnterListener(() =>
            {
                viewModel.AddTagCommand.Execute(null);
            }));

            _tagsDialogBindings.Add(new Binding<string, string>(
                viewModel,
                () => viewModel.NewTagInput,
                editBox,
                () => editBox.Text,BindingMode.TwoWay));

            view.FindViewById<ImageButton>(Resource.Id.AnimeTagsDialogAddTagButton).SetOnClickListener(new OnClickListener(
                v =>
                {
                    viewModel.AddTagCommand.Execute(null);
                }));

            _tagsDialog.Show();
            MainActivity.CurrentContext.DialogToCollapseOnBack = _tagsDialog;
        }

        private static View GetTagItem(int i, string s, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeTagsDialogListItem, null);
                view.FindViewById<ImageButton>(Resource.Id.AnimeTagsDialogListItemDeleteButton).Click +=
                    (sender, args) =>
                    {
                        _tagsDialogContext.RemoveTagCommand.Execute((sender as View).Tag.Unwrap<string>());
                    };

            }
            view.FindViewById<ImageButton>(Resource.Id.AnimeTagsDialogListItemDeleteButton).Tag = s.Wrap();
            view.FindViewById<TextView>(Resource.Id.AnimeTagsDialogListItemTag).Text = s;
            return view;
        }

        private static void CleanupTagsDialog()
        {
            AndroidUtilities.HideKeyboard();
            _tagsDialogBindings?.ForEach(binding => binding.Detach());
            _tagsDialog?.Dismiss();
            _scoreDialog = null;
            MainActivity.CurrentContext.DialogToCollapseOnBack = null;
        }
    }
}