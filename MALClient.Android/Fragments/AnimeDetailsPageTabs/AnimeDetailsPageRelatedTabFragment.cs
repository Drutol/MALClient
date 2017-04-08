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
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Models.AnimeScrapped;
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
            AnimeDetailsPageRelatedTabsList.Adapter = ViewModel.RelatedAnime.GetAdapter(RelatedItemTemplateDelegate);
            AnimeDetailsPageRelatedTabsList.OnItemClickListener = new OnItemClickListener<RelatedAnimeData>(data => ViewModel.NavigateDetailsCommand.Execute(data));
            AnimeDetailsPageRelatedTabsList.EmptyView = AnimeDetailsPageRelatedTabEmptyNotice;

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingRelated,
                    () => AnimeDetailsPageRelatedTabLoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }


        private View RelatedItemTemplateDelegate(int i, RelatedAnimeData relatedAnimeData, View convertView)
        {
            var view = convertView ??
                       MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeRelatedItem, null);


            view.Tag = relatedAnimeData.Wrap();
            view.FindViewById<TextView>(Resource.Id.AnimeRelatedItemContent).Text = relatedAnimeData.WholeRelation + relatedAnimeData.Title;

            return view;
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