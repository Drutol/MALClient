using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using FFImageLoading;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Android.BindingConverters;
using MALClient.Android.Dialogs;
using MALClient.Android.Flyouts;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments
{
    public partial class AnimeDetailsPageFragment : MalFragmentBase
    {
        private static AnimeDetailsPageNavigationArgs _navArgs;
        private AnimeDetailsPageViewModel ViewModel;
        private DroppyMenuPopup _menu;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;
            ViewModel.RegisterOneTimeOnPropertyChangedAction(nameof(ViewModel.AnimeMode), SetupForAnimeMode);
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            ViewModel.Init(_navArgs);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.DetailImage))
            {
                if (AnimeDetailsPageShowCoverImage != null)
                    ImageService.Instance.LoadUrl(ViewModel.DetailImage, TimeSpan.FromDays(7))
                        .FadeAnimation(false).Success(() => AnimeDetailsPageShowCoverImage.AnimateFadeIn())
                        .Into(AnimeDetailsPageShowCoverImage);
            }
        }

        protected override void InitBindings()
        {
            AnimeDetailsPagePivot.Adapter = new AnimeDetailsPagerAdapter(FragmentManager);
            AnimeDetailsPageTabStrip.SetViewPager(AnimeDetailsPagePivot);
           
            Bindings = new Dictionary<int, List<Binding>>();

            Bindings.Add(AnimeDetailsPageScoreButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageScoreButton.Id].Add(
                this.SetBinding(() => ViewModel.MyScoreBind,
                    () => AnimeDetailsPageScoreButton.Text));

            Bindings.Add(AnimeDetailsPageStatusButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageStatusButton.Id].Add(
                this.SetBinding(() => ViewModel.MyStatusBind,
                    () => AnimeDetailsPageStatusButton.Text));

            Bindings.Add(AnimeDetailsPageWatchedButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageWatchedButton.Id].Add(
                this.SetBinding(() => ViewModel.MyEpisodesBind,
                    () => AnimeDetailsPageWatchedButton.Text));

            Bindings.Add(AnimeDetailsPageReadVolumesButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageReadVolumesButton.Id].Add(
                this.SetBinding(() => ViewModel.MyVolumesBind,
                    () => AnimeDetailsPageReadVolumesButton.Text));

            Bindings.Add(AnimeDetailsPageLoadingOverlay.Id, new List<Binding>());
            Bindings[AnimeDetailsPageLoadingOverlay.Id].Add(
                this.SetBinding(() => ViewModel.LoadingGlobal,
                    () => AnimeDetailsPageLoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(AnimeDetailsPageAddSection.Id, new List<Binding>());
            Bindings[AnimeDetailsPageAddSection.Id].Add(
                this.SetBinding(() => ViewModel.AddAnimeVisibility,
                    () => AnimeDetailsPageAddSection.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(AnimeDetailsPageIncrementButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageIncrementButton.Id].Add(
                this.SetBinding(() => ViewModel.IsIncrementButtonEnabled,
                    () => AnimeDetailsPageIncrementButton.Enabled));

            Bindings.Add(AnimeDetailsPageDecrementButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageDecrementButton.Id].Add(
                this.SetBinding(() => ViewModel.IsDecrementButtonEnabled,
                    () => AnimeDetailsPageDecrementButton.Enabled));

            Bindings.Add(AnimeDetailsPageUpdateSection.Id, new List<Binding>());
            Bindings[AnimeDetailsPageUpdateSection.Id].Add(
                this.SetBinding(() => ViewModel.AddAnimeVisibility,
                    () => AnimeDetailsPageUpdateSection.Visibility).ConvertSourceToTarget(Converters.BoolToVisibilityInverted));

            Bindings.Add(AnimeDetailsPageIncDecSection.Id, new List<Binding>());
            Bindings[AnimeDetailsPageIncDecSection.Id].Add(
                this.SetBinding(() => ViewModel.AddAnimeVisibility,
                    () => AnimeDetailsPageIncDecSection.Visibility).ConvertSourceToTarget(Converters.BoolToVisibilityInverted));

            Bindings.Add(AnimeDetailsPagePivot.Id, new List<Binding>());
            Bindings[AnimeDetailsPagePivot.Id].Add(
                this.SetBinding(() => ViewModel.DetailsPivotSelectedIndex).WhenSourceChanges(() => AnimeDetailsPagePivot.SetCurrentItem(ViewModel.DetailsPivotSelectedIndex,true)));

            AnimeDetailsPageIncrementButton.SetCommand("Click",ViewModel.IncrementEpsCommand);
            AnimeDetailsPageDecrementButton.SetCommand("Click",ViewModel.DecrementEpsCommand);
            AnimeDetailsPageMoreButton.Click +=
                (sender, args) =>
                {
                   _menu = AnimeDetailsPageMoreFlyoutBuilder.BuildForAnimeDetailsPage(Activity, AnimeDetailsPageMoreButton,
                        ViewModel);
                   _menu.Show();
                };
            AnimeDetailsPageAddButton.SetCommand("Click",ViewModel.AddAnimeCommand);

            
            //OneTime

            AnimeDetailsPageWatchedLabel.Text = ViewModel.WatchedEpsLabel;
            


            //

            //Events
            AnimeDetailsPageStatusButton.Click += AnimeDetailsPageStatusButtonOnClick;
            AnimeDetailsPageScoreButton.Click += AnimeDetailsPageScoreButtonOnClick;
            AnimeDetailsPageWatchedButton.Click += AnimeDetailsPageWatchedButtonOnClick;
            AnimeDetailsPageReadVolumesButton.Click += AnimeDetailsPageVolumesButtonOnClick;


        }

        private void AnimeDetailsPageWatchedButtonOnClick(object sender, EventArgs eventArgs)
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel.AnimeItemReference as AnimeItemViewModel,
                (model, s) =>
                {
                    ViewModel.WatchedEpsInput = s;
                    ViewModel.ChangeWatchedCommand.Execute(null);
                });
        }

        private void AnimeDetailsPageScoreButtonOnClick(object sender, EventArgs eventArgs)
        {
            AnimeUpdateDialogBuilder.BuildScoreDialog(ViewModel.AnimeItemReference, i =>
            {
                ViewModel.ChangeScoreCommand.Execute(i);
            });
        }

        private void AnimeDetailsPageStatusButtonOnClick(object sender, EventArgs eventArgs)
        {
            AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel.AnimeItemReference,ViewModel.AnimeMode, status =>
            {
                ViewModel.ChangeStatus(status);
            });
        }

        private void AnimeDetailsPageVolumesButtonOnClick(object sender, EventArgs e)
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel.AnimeItemReference as AnimeItemViewModel,
                (model, s) =>
                {
                    ViewModel.ReadVolumesInput = s;
                    ViewModel.ChangeVolumesCommand.Execute(null);
                },true);
        }

        private void SetupForAnimeMode()
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
        }

        protected override void Cleanup()
        {
            AnimeDetailsPageStatusButton.Click -= AnimeDetailsPageStatusButtonOnClick;
            AnimeDetailsPageScoreButton.Click -= AnimeDetailsPageScoreButtonOnClick;
            AnimeDetailsPageWatchedButton.Click -= AnimeDetailsPageWatchedButtonOnClick;
            AnimeDetailsPageReadVolumesButton.Click -= AnimeDetailsPageVolumesButtonOnClick;
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPage;

        public static AnimeDetailsPageFragment BuildInstance(object args)
        {
            _navArgs = args as AnimeDetailsPageNavigationArgs;
            return new AnimeDetailsPageFragment();
        }
    }
}