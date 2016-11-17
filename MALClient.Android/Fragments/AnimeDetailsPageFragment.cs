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

            ViewModel.RegisterOneTimeOnPropertyChangedAction(nameof(ViewModel.DetailImage),
                () => ImageService.Instance.LoadUrl(ViewModel.DetailImage, TimeSpan.FromDays(7))
                    .FadeAnimation(true, true)
                    .Into(AnimeDetailsPageShowCoverImage));
            ViewModel.RegisterOneTimeOnPropertyChangedAction(nameof(ViewModel.AnimeMode), SetupForAnimeMode);
            ViewModel.Init(_navArgs);
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
            //OneTime

            AnimeDetailsPageWatchedLabel.Text = ViewModel.WatchedEpsLabel;

            Bindings.Add(AnimeDetailsPageReadVolumesButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageReadVolumesButton.Id].Add(
                this.SetBinding(() => ViewModel.MyVolumesBind,
                    () => AnimeDetailsPageReadVolumesButton.Text));
            //

            //Events
            AnimeDetailsPageStatusButton.Click += AnimeDetailsPageStatusButtonOnClick;
            AnimeDetailsPageScoreButton.Click += AnimeDetailsPageScoreButtonOnClick;
            AnimeDetailsPageWatchedButton.Click += AnimeDetailsPageWatchedButtonOnClick;


        }

        private void AnimeDetailsPageWatchedButtonOnClick(object sender, EventArgs eventArgs)
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel.AnimeItemReference as AnimeItemViewModel);
        }

        private void AnimeDetailsPageScoreButtonOnClick(object sender, EventArgs eventArgs)
        {
            
        }

        private void AnimeDetailsPageStatusButtonOnClick(object sender, EventArgs eventArgs)
        {
            AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel.AnimeItemReference,ViewModel.AnimeMode);
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
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPage;

        public static AnimeDetailsPageFragment BuildInstance(object args)
        {
            _navArgs = args as AnimeDetailsPageNavigationArgs;
            return new AnimeDetailsPageFragment();
        }
    }
}