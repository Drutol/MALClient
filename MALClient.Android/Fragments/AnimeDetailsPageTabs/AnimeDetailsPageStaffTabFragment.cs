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
using MALClient.Android.BindingConverters;
using MALClient.Android.UserControls;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    class AnimeDetailsPageStaffTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;
        private GridViewColumnHelper _helper;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void InitBindings()
        {
            _helper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView);
            Bindings.Add(this.SetBinding(() => ViewModel.AnimeStaffData).WhenSourceChanges(() =>
            {
                if (ViewModel.AnimeStaffData == null)
                    AnimeDetailsPageCharactersTabGridView.Adapter = null;
                else
                    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeStaff,DataTemplateFull,DataTemplateFling,ContainerTemplate  );
            }));


            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingCharactersVisibility,
                    () => AnimeDetailsPageCharactersTabLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        private View ContainerTemplate()
        {
            return new FavouriteItem(Activity);
        }

        private void DataTemplateFling(View view, FavouriteViewModel model)
        {
            (view as FavouriteItem).BindModel(model, true);
        }

        private void DataTemplateFull(View view, FavouriteViewModel model)
        {
            (view as FavouriteItem).BindModel(model,false);
        }


        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;

        #region Views

        private GridView _animeDetailsPageCharactersTabGridView;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public GridView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<GridView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion
    }
}