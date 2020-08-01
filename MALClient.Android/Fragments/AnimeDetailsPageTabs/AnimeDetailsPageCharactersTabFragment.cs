using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Android.Utilities.ImageLoading;
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
            //_gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1);
            Bindings.Add(this.SetBinding(() => ViewModel.AnimeStaffData).WhenSourceChanges(() =>
            {
                //if (ViewModel.AnimeStaffData == null)
                //    AnimeDetailsPageCharactersTabGridView.Adapter = null;
                //else
                //    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeCharacterPairs, DataTemplateFull, DataTemplateFling, ContainerTemplate);

                if (ViewModel.AnimeStaffData == null)
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(null);
                else
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(
                        new ObservableRecyclerAdapter<
                            AnimeDetailsPageViewModel.AnimeStaffDataViewModels.AnimeCharacterStaffModelViewModel,
                            Holder>(
                            ViewModel.AnimeStaffData.AnimeCharacterPairs,
                            DataTemplate,
                            LayoutInflater,
                            Resource.Layout.CharacterActorPairItem));

            }));

            AnimeDetailsPageCharactersTabGridView.SetLayoutManager(new GridLayoutManager(Activity, 2));
            AnimeDetailsPageCharactersTabGridView.AddOnScrollListener(new CustomScrollListener());

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingCharactersVisibility,
                    () => AnimeDetailsPageCharactersTabLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            SetUpForOrientation(Activity.Resources.Configuration.Orientation);
        }

        private void DataTemplate(
            AnimeDetailsPageViewModel.AnimeStaffDataViewModels.AnimeCharacterStaffModelViewModel item,
            Holder holder, int position)
        {
            var view = holder.ItemView;
            var itemCharacter = view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemCharacter);
            var itemPerson = view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemActor);

            itemCharacter.BindModel(item.AnimeCharacter, false);
            itemPerson.BindModel(item.AnimeStaffPerson, false);

            holder.CharacterActorPairItemActor.FavouriteItemImage.Into(item.AnimeStaffPerson.Data.ImgUrl);
            holder.CharacterActorPairItemCharacter.FavouriteItemImage.Into(item.AnimeCharacter.Data.ImgUrl);

            itemCharacter.RootContainer.SetOnClickListener(new OnClickListener(view1 => ItemCharacterOnClick(item.AnimeCharacter)));
            itemPerson.RootContainer.SetOnClickListener(new OnClickListener(view1 => ItemPersonOnClick(item.AnimeStaffPerson)));

        }

        private void ItemPersonOnClick(FavouriteViewModel item)
        {
            ViewModel.NavigateStaffDetailsCommand.Execute(item.Data);
        }

        private void ItemCharacterOnClick(FavouriteViewModel item)
        {
            ViewModel.NavigateCharacterDetailsCommand.Execute(item.Data);
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;


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

        private RecyclerView _animeDetailsPageCharactersTabGridView;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public RecyclerView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<RecyclerView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion

        class Holder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public Holder(View view) : base(view)
            {
                _view = view;
            }

            private FavouriteItem _characterActorPairItemCharacter;
            private FavouriteItem _characterActorPairItemActor;

            public FavouriteItem CharacterActorPairItemCharacter => _characterActorPairItemCharacter ?? (_characterActorPairItemCharacter = _view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemCharacter));
            public FavouriteItem CharacterActorPairItemActor => _characterActorPairItemActor ?? (_characterActorPairItemActor = _view.FindViewById<FavouriteItem>(Resource.Id.CharacterActorPairItemActor));
        }

    }
}