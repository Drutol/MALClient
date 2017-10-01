using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.UserControls;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;
using Orientation = Android.Content.Res.Orientation;


namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    class AnimeDetailsPageStaffTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;
        private GridViewColumnHelper _gridHelper;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void InitBindings()
        {
            _gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,null,2,3);
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

        private View ContainerTemplate(int i)
        {
            return new FavouriteItem(Activity);
        }

        private void DataTemplateFling(View view, int i, FavouriteViewModel model)
        {
            (view as FavouriteItem).BindModel(model, true);
        }

        private void DataTemplateFull(View view, int i, FavouriteViewModel model)
        {
            var item = view as FavouriteItem;
            var firstRun = !item.Initialized;
            item.BindModel(model,false);

            if (firstRun)
            {
                item.Click += PersonOnClick;
            }
        }

        private void PersonOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateStaffDetailsCommand.Execute((sender as View).Tag.Unwrap<FavouriteViewModel>().Data);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            //SetUpForOrientation(newConfig.Orientation);
            _gridHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        private void SetUpForOrientation(Orientation orientation)
        {
            ViewGroup.LayoutParams param;
            switch (orientation)
            {
                case Orientation.Landscape:
                    param = RootView.LayoutParameters;
                    param.Height = ViewGroup.LayoutParams.WrapContent;
                    RootView.LayoutParameters = param;
                    break;
                case Orientation.Portrait:
                case Orientation.Square:
                case Orientation.Undefined:
                    param = RootView.LayoutParameters;
                    param.Height = ViewGroup.LayoutParams.MatchParent;
                    RootView.LayoutParameters = param;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
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