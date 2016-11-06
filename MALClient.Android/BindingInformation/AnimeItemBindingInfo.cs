using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Extensions;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Flyouts;
using MALClient.Android.Managers;
using MALClient.Models.Enums.Enums;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;

namespace MALClient.Android.BindingInformation
{
    public class AnimeItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        private bool _bindingsInitialized;
        private bool _oneTimeBindingsInitialized;
        private bool _moreFlyoutMenuInitialized;

        private static FlyoutMenuView.IAdapter _moreFlyoutAdapter;
        private enum MoreFlyoutButtons
        {
            [EnumUtilities.Description("Copy link")]
            CopyLink,
            [EnumUtilities.Description("Open in browser")]
            OpenInBrowser
        }

        public AnimeItemBindingInfo(View container, AnimeItemViewModel viewModel) : base(container, viewModel) {}

        protected override void InitBindings()
        {
            if (_bindingsInitialized)
                return;

            _bindingsInitialized = true;

            ViewModel.AnimeItemDisplayContext = AnimeItemDisplayContext.AirDay;

            var typeView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemType);
            Bindings.Add(Resource.Id.AnimeGridItemType,new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemType].Add(new Binding<string, string>(
                ViewModel,
                () => ViewModel.Type,
                typeView,
                () => typeView.Text));

            var topLeftView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo);
            Bindings.Add(Resource.Id.AnimeGridItemToLeftInfo, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemToLeftInfo].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Text));
            Bindings[Resource.Id.AnimeGridItemToLeftInfo].Add(new Binding<string, ViewStates>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Visibility).ConvertSourceToTarget(Converters.IsStringEmptyToVisibility));

            var statusView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemCurrentWatchingStatus);
            Bindings.Add(Resource.Id.AnimeGridItemCurrentWatchingStatus, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemCurrentWatchingStatus].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyStatusBindShort,
                    statusView,
                    () => statusView.Text));

            var watchedView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus);
            Bindings.Add(Resource.Id.AnimeGridItemWatchedStatus, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemWatchedStatus].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyEpisodesBindShort,
                    watchedView,
                    () => watchedView.Text));

            var scoreView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemScore);
            Bindings.Add(Resource.Id.AnimeGridItemScore, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemScore].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyScoreBindShort,
                    scoreView,
                    () => scoreView.Text));
        }

        private void InitializeMoreFlyout()
        {
            if(_moreFlyoutMenuInitialized)
                return;
            _moreFlyoutMenuInitialized = true;

            if (_moreFlyoutAdapter == null)
                _moreFlyoutAdapter =
                    new FlyoutMenuView.ArrayAdapter(
                        new[] {MoreFlyoutButtons.CopyLink, MoreFlyoutButtons.OpenInBrowser}.Select(
                            item => new TextFlyoutItem((int) item, item.GetDescription())).ToList());

            AnimeGridItemMoreFlyout.Layout = new FlyoutMenuView.GridLayout(1, FlyoutMenuView.GridLayout.Unspecified);
            AnimeGridItemMoreFlyout.Adapter = _moreFlyoutAdapter;

            AnimeGridItemMoreFlyout.Visibility = ViewStates.Visible;
        }

        protected override void InitOneTimeBindings()
        {
            if(_oneTimeBindingsInitialized)
                return;
            _oneTimeBindingsInitialized = true;


            var img = Container.FindViewById<ImageViewAsync>(Resource.Id.AnimeGridItemImage);
            ImageService.Instance.LoadUrl(ViewModel.ImgUrl, TimeSpan.FromDays(7)).FadeAnimation(true,true).Into(img);

            InitializeMoreFlyout();

            Container.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle).Text = ViewModel.Title;
        }

        protected override void DetachBindings()
        {
            base.DetachBindings();

            _bindingsInitialized = false;
            _oneTimeBindingsInitialized = false;
        }

        private ImageButton _animeGridItemMoreButton;
        private FlyoutMenuView _animeGridItemMoreFlyout;

        public ImageButton AnimeGridItemMoreButton => _animeGridItemMoreButton ?? (_animeGridItemMoreButton = Container.FindViewById<ImageButton>(Resource.Id.AnimeGridItemMoreButton));

        public FlyoutMenuView AnimeGridItemMoreFlyout => _animeGridItemMoreFlyout ?? (_animeGridItemMoreFlyout = Container.FindViewById<FlyoutMenuView>(Resource.Id.AnimeGridItemMoreFlyout));

    }
}