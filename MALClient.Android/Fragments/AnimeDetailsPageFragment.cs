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
using FFImageLoading;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Android.BindingConverters;
using MALClient.Android.DIalogs;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments
{
    public partial class AnimeDetailsPageFragment : MalFragmentBase
    {
        private static AnimeDetailsPageNavigationArgs _navArgs;
        private AnimeDetailsPageViewModel ViewModel;

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
                        .FadeAnimation(true, true)
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
                ViewModel.ChangeStatusCommand.Execute((int)status);
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