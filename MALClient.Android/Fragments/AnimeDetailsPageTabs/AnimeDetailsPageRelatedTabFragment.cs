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
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    class AnimeDetailsPageRelatedTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;
        private ListView _list;
        private ObservableAdapter<RelatedAnimeData> _adapter;

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
            _adapter = ViewModel.RelatedAnime.GetAdapter(RelatedItemTemplateDelegate);
            _list.Adapter = _adapter;
            _list.ItemClick += ListOnItemClick;
        }

        private void ListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var data = itemClickEventArgs.View.Tag.Unwrap<RelatedAnimeData>();
            ViewModel.NavigateDetailsCommand.Execute(data);
        }

        private View RelatedItemTemplateDelegate(int i, RelatedAnimeData relatedAnimeData, View convertView)
        {
            var view = convertView ??
                       MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeRelatedItem, null);


            view.Tag = relatedAnimeData.Wrap();
            view.FindViewById<TextView>(Resource.Id.AnimeRelatedItemContent).Text = relatedAnimeData.WholeRelation;

            return view;
        }

        protected override void Cleanup()
        {
            _list.Adapter = null;
            _adapter.Dispose();
            _list.ItemClick -= ListOnItemClick;
            base.Cleanup();
        }

        public static AnimeDetailsPageRelatedTabFragment Instance  => new AnimeDetailsPageRelatedTabFragment();

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageRelatedTab;
    }
}