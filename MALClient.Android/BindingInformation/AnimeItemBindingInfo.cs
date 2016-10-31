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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Managers;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BindingInformation
{
    public class AnimeItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        private bool _bindingsInitialized;
        private bool _oneTimeBindingsInitialized;

        public AnimeItemBindingInfo(View container, AnimeItemViewModel viewModel) : base(container, viewModel) {}

        protected override void InitBindings()
        {
            if (_bindingsInitialized)
                return;

            _bindingsInitialized = true;

            var titleView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle);
            Bindings.Add(Resource.Id.AnimeGridItemTitle,
                new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.Title,
                    titleView,
                    () => titleView.Text));

            var typeView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemType);
            Bindings.Add(Resource.Id.AnimeGridItemType,
                new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.Type,
                    typeView,
                    () => typeView.Text));

            var topLeftView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo);
            Bindings.Add(Resource.Id.AnimeGridItemToLeftInfo,
                new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Text));

            var statusView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemCurrentWatchingStatus);
            Bindings.Add(Resource.Id.AnimeGridItemCurrentWatchingStatus,
                new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyStatusBindShort,
                    statusView,
                    () => statusView.Text));

            var watchedView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus);
            Bindings.Add(Resource.Id.AnimeGridItemWatchedStatus,
                new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyEpisodesBind,
                    watchedView,
                    () => watchedView.Text));

            var scoreView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemScore);
            Bindings.Add(Resource.Id.AnimeGridItemScore,
                new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyScoreBindShort,
                    scoreView,
                    () => scoreView.Text));
        }

        protected override void InitOneTimeBindings()
        {
            if(_oneTimeBindingsInitialized)
                return;
            _oneTimeBindingsInitialized = true;

            var img = Container.FindViewById<ImageViewAsync>(Resource.Id.AnimeGridItemImage);
            ImageService.Instance.LoadUrl(ViewModel.ImgUrl, TimeSpan.FromDays(7)).Into(img);
        }

        protected override void DetachBindings()
        {
            base.DetachBindings();

            _bindingsInitialized = false;
            _oneTimeBindingsInitialized = false;
        }
    }
}