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
        private ListView _list;

        private AnimeDetailsPageRelatedTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
           
        }

        protected override void InitBindings()
        {
            _list = FindViewById<ListView>(Resource.Id.AnimeDetailsPageRelatedTabsList);
            _list.Adapter = ViewModel.RelatedAnime.GetAdapter(RelatedItemTemplateDelegate);
            _list.OnItemClickListener = new OnItemClickListener<RelatedAnimeData>(data => ViewModel.NavigateDetailsCommand.Execute(data));

            Bindings.Add(LoadingOverlay.Id, new List<Binding>());
            Bindings[LoadingOverlay.Id].Add(
                this.SetBinding(() => ViewModel.LoadingRelated,
                    () => LoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }


        private View RelatedItemTemplateDelegate(int i, RelatedAnimeData relatedAnimeData, View convertView)
        {
            var view = convertView ??
                       MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeRelatedItem, null);


            view.Tag = relatedAnimeData.Wrap();
            view.FindViewById<TextView>(Resource.Id.AnimeRelatedItemContent).Text = relatedAnimeData.WholeRelation;

            return view;
        }


        public static AnimeDetailsPageRelatedTabFragment Instance  => new AnimeDetailsPageRelatedTabFragment();

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageRelatedTab;

        #region Views

        private RelativeLayout _loadingOverlay;
        public RelativeLayout LoadingOverlay => _loadingOverlay ?? (_loadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageRelatedTabLoadingOverlay));

        #endregion
    }
}