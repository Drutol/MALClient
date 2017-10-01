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
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    class AnimeDetailsPageRelatedTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;

        private AnimeDetailsPageRelatedTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
           
        }

        protected override void InitBindings()
        {
            AnimeDetailsPageRelatedTabsList.InjectFlingAdapter(ViewModel.RelatedAnime, DataTemplateFull,
                DataTemplateFling, ContainerTemplate, DataTemplateBasic);


            Bindings.Add(
                this.SetBinding(() => ViewModel.NoRelatedDataNoticeVisibility,
                    () => AnimeDetailsPageRelatedTabEmptyNotice.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingRelated,
                    () => AnimeDetailsPageRelatedTabLoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        private void DataTemplateBasic(View view, int i, RelatedAnimeData relatedAnimeData)
        {
            view.FindViewById<TextView>(Resource.Id.AnimeRelatedItemContent).Text = relatedAnimeData.WholeRelation + relatedAnimeData.Title;
        }

        private View ContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeRelatedItem, null);
            view.FindViewById(Resource.Id.RootContainer).Click += OnClick;
            return view;
        }

        private void OnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateDetailsCommand.Execute(((sender as View).Parent as View).Tag.Unwrap<RelatedAnimeData>());
        }

        private void DataTemplateFling(View view1, int i, RelatedAnimeData arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.Image);
            string link = null;

            if (AnimeImageQuery.IsCached(arg3.Id, arg3.Type == RelatedItemType.Anime, ref link))
            {
                if (!img.IntoIfLoaded(link))
                    img.Visibility = ViewStates.Invisible;
            }
            else
                img.Visibility = ViewStates.Invisible;

        }

        private void DataTemplateFull(View view1, int i, RelatedAnimeData arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.Image);
            string imgUrl = null;
            if (AnimeImageQuery.IsCached(arg3.Id, arg3.Type == RelatedItemType.Anime, ref imgUrl))
                img.Into(imgUrl);
            else
                img.IntoWithTask(AnimeImageQuery.GetImageUrl(arg3.Id, arg3.Type == RelatedItemType.Anime));
        }

        public static AnimeDetailsPageRelatedTabFragment Instance  => new AnimeDetailsPageRelatedTabFragment();

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageRelatedTab;

        #region Views
        private ListView _animeDetailsPageRelatedTabsList;
        private TextView _animeDetailsPageRelatedTabEmptyNotice;
        private RelativeLayout _animeDetailsPageRelatedTabLoadingOverlay;

        public ListView AnimeDetailsPageRelatedTabsList => _animeDetailsPageRelatedTabsList ?? (_animeDetailsPageRelatedTabsList = FindViewById<ListView>(Resource.Id.AnimeDetailsPageRelatedTabsList));

        public TextView AnimeDetailsPageRelatedTabEmptyNotice => _animeDetailsPageRelatedTabEmptyNotice ?? (_animeDetailsPageRelatedTabEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeDetailsPageRelatedTabEmptyNotice));

        public RelativeLayout AnimeDetailsPageRelatedTabLoadingOverlay => _animeDetailsPageRelatedTabLoadingOverlay ?? (_animeDetailsPageRelatedTabLoadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageRelatedTabLoadingOverlay));


        #endregion
    }
}