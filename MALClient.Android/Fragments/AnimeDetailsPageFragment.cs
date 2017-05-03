using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using FFImageLoading;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.PagerAdapters;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments
{
    public partial class AnimeDetailsPageFragment : MalFragmentBase
    {
        private AnimeDetailsPageNavigationArgs _navArgs;
        private AnimeDetailsPageViewModel ViewModel;
        private DroppyMenuPopup _menu;

        public AnimeDetailsPageFragment(AnimeDetailsPageNavigationArgs navArgs)
        {
            _navArgs = navArgs;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;            
            ViewModel.Init(_navArgs,false);
        }




        protected override void InitBindings()
        {
            AnimeDetailsPagePivot.Adapter = new AnimeDetailsPagerAdapter(ChildFragmentManager);
            AnimeDetailsPageTabStrip.SetViewPager(AnimeDetailsPagePivot);
            AnimeDetailsPageTabStrip.CenterTabs();
            AnimeDetailsPagePivot.SetCurrentItem(_navArgs.SourceTabIndex, false);
            AnimeDetailsPagePivot.OffscreenPageLimit = 7;

            AnimeDetailsPagePivot.AddOnPageChangeListener(
                new OnPageChangedListener(i => ViewModel.DetailsPivotSelectedIndex = i));

            Bindings.Add(
                this.SetBinding(() => ViewModel.MyScoreBind,
                    () => AnimeDetailsPageScoreButton.Text));
            Bindings.Add(
                this.SetBinding(() => ViewModel.MyStatusBind,
                    () => AnimeDetailsPageStatusButton.Text));
            Bindings.Add(
                this.SetBinding(() => ViewModel.MyEpisodesBind,
                    () => AnimeDetailsPageWatchedButton.Text));
            Bindings.Add(
                this.SetBinding(() => ViewModel.MyVolumesBind,
                    () => AnimeDetailsPageReadVolumesButton.Text));
            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingGlobal,
                        () => AnimeDetailsPageLoadingOverlay.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.IsIncrementButtonEnabled,
                    () => AnimeDetailsPageIncrementButton.Enabled));
            Bindings.Add(
                this.SetBinding(() => ViewModel.IsDecrementButtonEnabled,
                    () => AnimeDetailsPageDecrementButton.Enabled));

            Bindings.Add(this.SetBinding(() => ViewModel.AddAnimeVisibility)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.AddAnimeVisibility)
                    {
                        AnimeDetailsPageIncDecSection.Visibility = ViewStates.Gone;
                        AnimeDetailsPageUpdateSection.Visibility = ViewStates.Gone;
                        AnimeDetailsPageAddSection.Visibility = ViewStates.Visible;
                        AnimeDetailsPageFavouriteButton.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        AnimeDetailsPageIncDecSection.Visibility = ViewStates.Visible;
                        AnimeDetailsPageUpdateSection.Visibility = ViewStates.Visible;
                        AnimeDetailsPageAddSection.Visibility = ViewStates.Gone;
                        AnimeDetailsPageFavouriteButton.Visibility = ViewStates.Visible;
                    }
                }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.DetailsPivotSelectedIndex)
                    .WhenSourceChanges(
                        () => AnimeDetailsPagePivot.SetCurrentItem(ViewModel.DetailsPivotSelectedIndex, true)));


            Bindings.Add(
                this.SetBinding(() => ViewModel.IsFavourite)
                    .WhenSourceChanges(() =>
                    {
                        if (ViewModel.IsFavourite)
                        {
                            AnimeDetailsPageFavouriteButton.ImageTintList = ColorStateList.ValueOf(Color.White);
                            AnimeDetailsPageFavouriteButton.SetImageResource(Resource.Drawable.icon_unfavourite);
                            AnimeDetailsPageFavouriteButton.SetBackgroundResource(ResourceExtension.AccentColourRes);
                        }
                        else
                        {
                            AnimeDetailsPageFavouriteButton.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));
                            AnimeDetailsPageFavouriteButton.SetImageResource(Resource.Drawable.icon_fav_outline);
                            AnimeDetailsPageFavouriteButton.SetBackgroundColor(Color.Transparent);
                        }

                    }));


            Bindings.Add(this.SetBinding(() => ViewModel.AnimeMode)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.AnimeMode)
                    {
                        AnimeDetailsPageReadVolumesButton.Visibility =
                            AnimeDetailsPageReadVolumesLabel.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        AnimeDetailsPageReadVolumesButton.Visibility =
                            AnimeDetailsPageReadVolumesLabel.Visibility = ViewStates.Visible;
                    }
                }));

            Bindings.Add(this.SetBinding(() => ViewModel.DetailImage)
                .WhenSourceChanges(() =>
                {
                    AnimeDetailsPageShowCoverImage.Visibility = ViewStates.Invisible;
                    AnimeDetailsPageShowCoverImage.Into(ViewModel.DetailImage);
                }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingUpdate,
                        () => AnimeDetailsPageLoadingUpdateSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.IsAddAnimeButtonEnabled,
                    () => AnimeDetailsPageAddButton.Enabled));

            AnimeDetailsPageFavouriteButton.SetOnClickListener(
                new OnClickListener(view => ViewModel.ToggleFavouriteCommand.Execute(null)));
            AnimeDetailsPageIncrementButton.SetOnClickListener(
                new OnClickListener(view => ViewModel.IncrementEpsCommand.Execute(null)));
            AnimeDetailsPageDecrementButton.SetOnClickListener(
                new OnClickListener(view => ViewModel.DecrementEpsCommand.Execute(null)));
            AnimeDetailsPageAddButton.SetOnClickListener(
                new OnClickListener(view => ViewModel.AddAnimeCommand.Execute(null)));
            AnimeDetailsPageMoreButton.SetOnClickListener(new OnClickListener(view =>
            {
                _menu = AnimeDetailsPageMoreFlyoutBuilder.BuildForAnimeDetailsPage(Activity, ViewModel,
                    AnimeDetailsPageMoreButton,
                    OnMoreFlyoutClick);
                _menu.Show();
            }));



            //OneTime

            AnimeDetailsPageWatchedLabel.Text = ViewModel.WatchedEpsLabel;



            //

            //Events
            AnimeDetailsPageStatusButton.SetOnClickListener(
                new OnClickListener(view => AnimeDetailsPageStatusButtonOnClick()));
            AnimeDetailsPageScoreButton.SetOnClickListener(
                new OnClickListener(view => AnimeDetailsPageScoreButtonOnClick()));
            AnimeDetailsPageWatchedButton.SetOnClickListener(
                new OnClickListener(view => AnimeDetailsPageWatchedButtonOnClick()));
            AnimeDetailsPageReadVolumesButton.SetOnClickListener(
                new OnClickListener(view => AnimeDetailsPageVolumesButtonOnClick()));


        }

        private void OnMoreFlyoutClick(int i)
        {
            switch (i)
            {
                case 0:
                    ViewModel.NavigateForumBoardCommand.Execute(null);
                    break;
                case 1:
                    AnimeDetailsPageDialogBuilder.BuildPromotionalVideoDialog(ViewModel);
                    break;
                case 2:
                    AnimeUpdateDialogBuilder.BuildTagDialog(ViewModel);
                    break;
                case 3:
                    ViewModel.CopyToClipboardCommand.Execute(null);
                    break;
                case 4:
                    ViewModel.OpenInMalCommand.Execute(null);
                    break;
                case 5:
                    ViewModel.RemoveAnimeCommand.Execute(null);
                    break;
                case 6:
                    ViewModel.IsRewatching = !ViewModel.IsRewatching;
                    break;
            }
            _menu?.Dismiss(true);
            _menu = null;
        }

        private void AnimeDetailsPageWatchedButtonOnClick()
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel.AnimeItemReference as AnimeItemViewModel,
                (model, s) =>
                {
                    ViewModel.WatchedEpsInput = s;
                    ViewModel.ChangeWatchedCommand.Execute(null);
                });
        }

        private void AnimeDetailsPageScoreButtonOnClick()
        {
            AnimeUpdateDialogBuilder.BuildScoreDialog(ViewModel.AnimeItemReference, i =>
            {
                ViewModel.ChangeScoreCommand.Execute(i.ToString());
            });
        }

        private void AnimeDetailsPageStatusButtonOnClick()
        {
            AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel.AnimeItemReference,ViewModel.AnimeMode, status =>
            {
                ViewModel.ChangeStatus(status);
            });
        }

        private void AnimeDetailsPageVolumesButtonOnClick()
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel.AnimeItemReference as AnimeItemViewModel,
                (model, s) =>
                {
                    ViewModel.ReadVolumesInput = s;
                    ViewModel.ChangeVolumesCommand.Execute(null);
                },true);
        }


        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPage;
    }
}