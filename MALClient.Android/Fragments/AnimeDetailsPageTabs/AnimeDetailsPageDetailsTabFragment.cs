using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.Android.BindingConverters;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    internal class AnimeDetailsPageDetailsTabFragment : MalFragmentBase
    {
        private readonly Activity _activity;

        private readonly AnimeDetailsPageViewModel ViewModel;

        private AnimeDetailsPageDetailsTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
            _activity = MainActivity.CurrentContext;
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageDetailsTab;

        public static AnimeDetailsPageDetailsTabFragment Instance => new AnimeDetailsPageDetailsTabFragment();

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.OnDetailsLoaded += CreateDetailsAdapters;
        }

        private void CreateDetailsAdapters()
        {
            AnimeDetailsPageDetailsTabLeftGenresList.SetAdapter(new GenresAdapter(_activity, ViewModel.LeftGenres, true));
            AnimeDetailsPageDetailsTabRightGenresList.SetAdapter(new GenresAdapter(_activity , ViewModel.RightGenres, true));
            AnimeDetailsPageDetailsTabInformationList.SetAdapter(new DetailsAdapter(_activity, ViewModel.Information, true));
            AnimeDetailsPageDetailsTabStatsList.SetAdapter(new DetailsAdapter(_activity, ViewModel.Stats, true));
            AnimeDetailsPageDetailsTabOPsList.SetAdapter(new GenresAdapter(_activity, ViewModel.OPs, true));
            AnimeDetailsPageDetailsTabEDsList.SetAdapter(new GenresAdapter(_activity, ViewModel.EDs, true));
        }

        protected override void InitBindings()
        {
            CreateDetailsAdapters();

            Bindings.Add(LoadingOverlay.Id, new List<Binding>());
            Bindings[LoadingOverlay.Id].Add(
                this.SetBinding(() => ViewModel.LoadingDetails,
                    () => LoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        protected override void Cleanup()
        {
            ViewModel.OnDetailsLoaded -= CreateDetailsAdapters;
        }

        private class GenresAdapter : AlternatingListCollectionAdapter<string>
        {
            public GenresAdapter(Activity context, IEnumerable<string> items, bool startLight)
                : base(context, Resource.Layout.GenreItemView, items, startLight)
            {
            }

            protected override void InitializeView(View view, string model)
            {
                view.FindViewById<TextView>(Resource.Id.GenreItemTextView).Text = model;
            }
        }

        private class DetailsAdapter : AlternatingListCollectionAdapter<Tuple<string, string>>
        {
            public DetailsAdapter(Activity context, IEnumerable<Tuple<string, string>> items, bool startLight)
                : base(context, Resource.Layout.DetailItemView, items, startLight)
            {
            }

            protected override void InitializeView(View view, Tuple<string, string> model)
            {
                view.FindViewById<TextView>(Resource.Id.DetailItemCategoryTextView).Text = model.Item1;
                view.FindViewById<TextView>(Resource.Id.DetailItemContentTextView).Text = model.Item2;
            }
        }

        #region Views

        private LinearLayout _animeDetailsPageDetailsTabLeftGenresList;
        private LinearLayout _animeDetailsPageDetailsTabRightGenresList;
        private LinearLayout _animeDetailsPageDetailsTabInformationList;
        private LinearLayout _animeDetailsPageDetailsTabStatsList;
        private LinearLayout _animeDetailsPageDetailsTabOPsList;
        private LinearLayout _animeDetailsPageDetailsTabEDsList;

        public LinearLayout AnimeDetailsPageDetailsTabLeftGenresList => _animeDetailsPageDetailsTabLeftGenresList ?? (_animeDetailsPageDetailsTabLeftGenresList = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageDetailsTabLeftGenresList));

        public LinearLayout AnimeDetailsPageDetailsTabRightGenresList => _animeDetailsPageDetailsTabRightGenresList ?? (_animeDetailsPageDetailsTabRightGenresList = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageDetailsTabRightGenresList));

        public LinearLayout AnimeDetailsPageDetailsTabInformationList => _animeDetailsPageDetailsTabInformationList ?? (_animeDetailsPageDetailsTabInformationList = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageDetailsTabInformationList));

        public LinearLayout AnimeDetailsPageDetailsTabStatsList => _animeDetailsPageDetailsTabStatsList ?? (_animeDetailsPageDetailsTabStatsList = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageDetailsTabStatsList));

        public LinearLayout AnimeDetailsPageDetailsTabOPsList => _animeDetailsPageDetailsTabOPsList ?? (_animeDetailsPageDetailsTabOPsList = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageDetailsTabOPsList));

        public LinearLayout AnimeDetailsPageDetailsTabEDsList => _animeDetailsPageDetailsTabEDsList ?? (_animeDetailsPageDetailsTabEDsList = FindViewById<LinearLayout>(Resource.Id.AnimeDetailsPageDetailsTabEDsList));





        private RelativeLayout _loadingOverlay;
        public RelativeLayout LoadingOverlay => _loadingOverlay ?? (_loadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageDetailsTabLoadingOverlay));

        #endregion
    }
}