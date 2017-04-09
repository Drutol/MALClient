using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    internal class AnimeDetailsPageDetailsTabFragment : MalFragmentBase
    {

        private readonly AnimeDetailsPageViewModel ViewModel;

        private AnimeDetailsPageDetailsTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageDetailsTab;

        public static AnimeDetailsPageDetailsTabFragment Instance => new AnimeDetailsPageDetailsTabFragment();

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingDetails,
                    () => LoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.LoadingDetails).WhenSourceChanges(() =>
            {
                if (ViewModel.LoadingDetails)
                    return;

                AnimeDetailsPageDetailsTabLeftGenresList.SetAdapter(
                    ViewModel.RightGenres.GetAdapter(GetSingleDetailTemplateDelegate));          
                AnimeDetailsPageDetailsTabRightGenresList.SetAdapter(
                    ViewModel.LeftGenres.GetAdapter(GetSingleDetailTemplateDelegate));
                AnimeDetailsPageDetailsTabInformationList.SetAdapter(
                    ViewModel.Information.GetAdapter(GetDetailsTemplateDelegate));
                AnimeDetailsPageDetailsTabStatsList.SetAdapter(ViewModel.Stats.GetAdapter(GetDetailsTemplateDelegate));

                if (ViewModel.AnimeMode)
                {
                    AnimeDetailsPageDetailsTabOPsList.Visibility =
                        AnimeDetailsPageDetailsTabEDsList.Visibility = ViewStates.Visible;
                    AnimeDetailsPageDetailsTabOPsList.SetAdapter(ViewModel.OPs.GetAdapter(GetSingleDetailTemplateDelegate));
                    AnimeDetailsPageDetailsTabEDsList.SetAdapter(ViewModel.EDs.GetAdapter(GetSingleDetailTemplateDelegate));
                }
                else
                {
                    AnimeDetailsPageDetailsTabOPsList.Visibility =
                        AnimeDetailsPageDetailsTabEDsList.Visibility = ViewStates.Gone;
                }              
            }));
        }

        private View GetSingleDetailTemplateDelegate(int i, string s, View arg3)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.GenreItemView, null);
            view.FindViewById<TextView>(Resource.Id.GenreItemTextView).Text = s;
            view.SetBackgroundColor(
                new Color(i % 2 == 0
                    ? ResourceExtension.BrushRowAlternate1
                    : ResourceExtension.BrushRowAlternate2));

            return view;
        }

        private View GetDetailsTemplateDelegate(int i, Tuple<string, string> tuple, View arg3)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.DetailItemView, null);
            view.FindViewById<TextView>(Resource.Id.DetailItemCategoryTextView).Text = tuple.Item1;
            view.FindViewById<TextView>(Resource.Id.DetailItemContentTextView).Text = tuple.Item2;            view.SetBackgroundColor(
                new Color(i % 2 == 0
                    ? ResourceExtension.BrushRowAlternate1
                    : ResourceExtension.BrushRowAlternate2));

            return view;
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