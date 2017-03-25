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
    class AnimeDetailsPageCharactersTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;
        private GridViewColumnHelper _gridHelper;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void InitBindings()
        {
            _gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1);
            Bindings.Add(this.SetBinding(() => ViewModel.AnimeStaffData).WhenSourceChanges(() =>
            {
                if (ViewModel.AnimeStaffData == null)
                    AnimeDetailsPageCharactersTabGridView.Adapter = null;
                else
                    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeCharacterPairs,DataTemplateFull,DataTemplateFling,ContainerTemplate  );
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingCharactersVisibility,
                    () => AnimeDetailsPageCharactersTabLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            SetUpForOrientation(Activity.Resources.Configuration.Orientation);
        }

        private View ContainerTemplate()
        {
            var view =  Activity.LayoutInflater.Inflate(Resource.Layout.CharacterActorPairItem, null);
            return view;
        }

        private void ItemPersonOnClick(object sender, EventArgs eventArgs)
        {

        }

        private void ItemCharacterOnClick(object sender, EventArgs eventArgs)
        {
            var item = (sender as View).Tag.Unwrap<FavouriteBase>();
            ViewModel.NavigateCharacterDetailsCommand.Execute(item);
        }

        private void DataTemplateFling(View view, AnimeDetailsPageViewModel.AnimeStaffDataViewModels.AnimeCharacterStaffModelViewModel models)
        {
            var itemCharacter = view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemCharacter);
            var itemPerson = view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemActor);
                
            itemCharacter.BindModel(models.AnimeCharacter, true);
            itemPerson.BindModel(models.AnimeStaffPerson, true);
        }

        private void DataTemplateFull(View view, AnimeDetailsPageViewModel.AnimeStaffDataViewModels.AnimeCharacterStaffModelViewModel models)
        {
            var itemCharacter = view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemCharacter);
            var itemPerson = view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemActor);

            bool firstRun = !itemCharacter.Initialized;
            itemCharacter.BindModel(models.AnimeCharacter,false);
            itemPerson.BindModel(models.AnimeStaffPerson,false);

            if (firstRun)
            {
                itemCharacter.Click += ItemCharacterOnClick;
                itemPerson.Click += ItemPersonOnClick;
            }
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;


        public override void OnConfigurationChanged(Configuration newConfig)
        {
            SetUpForOrientation(newConfig.Orientation);
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

        #region Views

        private GridView _animeDetailsPageCharactersTabGridView;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public GridView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<GridView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion
    }
}