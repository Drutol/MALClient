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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Managers;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BindingInformation
{
    public class AnimeItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        public AnimeItemBindingInfo(View container, AnimeItemViewModel viewModel) : base(container, viewModel)
        {

        }

        protected override void InitBindings()
        {
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemTitle))
            {
                var titleView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle);
                Bindings.Add(Resource.Id.AnimeGridItemTitle,
                    new Binding<string, string>(ViewModel, () => ViewModel.Title, titleView, () => titleView.Text));
            }
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemType))
            {
                var typeView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemType);
                Bindings.Add(Resource.Id.AnimeGridItemType,
                    new Binding<string, string>(ViewModel, () => ViewModel.Type, typeView, () => typeView.Text));
            }
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemToLeftInfo))
            {
                var topLeftView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo);
                Bindings.Add(Resource.Id.AnimeGridItemToLeftInfo,
                    new Binding<string, string>(ViewModel, () => ViewModel.TopLeftInfoBind, topLeftView,
                        () => topLeftView.Text));
            }
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemWatchedStatus))
            {
                var watchedView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus);
                Bindings.Add(Resource.Id.AnimeGridItemWatchedStatus,
                    new Binding<string, string>(ViewModel, () => ViewModel.MyStatusBindShort, watchedView,
                        () => watchedView.Text));
            }
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemWatchedStatus))
            {
                var watchedView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus);
                Bindings.Add(Resource.Id.AnimeGridItemWatchedStatus,
                    new Binding<string, string>(ViewModel, () => ViewModel.MyEpisodesBind, watchedView,
                        () => watchedView.Text));
            }
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemScore))
            {
                var scoreView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemScore);
                Bindings.Add(Resource.Id.AnimeGridItemScore,
                    new Binding<string, string>(ViewModel, () => ViewModel.MyScoreBindShort, scoreView,
                        () => scoreView.Text));
            }
            
        }

        protected override async void Setup()
        {
            if (!Bindings.ContainsKey(Resource.Id.AnimeGridItemImage))
            {
                Bindings.Add(Resource.Id.AnimeGridItemImage, null);
                var img = Container.FindViewById<ImageView>(Resource.Id.AnimeGridItemImage);
                img.SetImageBitmap(await ImageCache.GetImageBitmapFromUrl(ViewModel.ImgUrl));
            }
        }

        public override void DetachBindings()
        {
            foreach (var binding in Bindings)
            {
                binding.Value?.Detach();
            }
            Bindings = new Dictionary<int, Binding>();
        }
    }
}