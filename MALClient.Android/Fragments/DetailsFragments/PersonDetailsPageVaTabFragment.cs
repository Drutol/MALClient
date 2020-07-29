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
using AoLibs.Adapters.Core;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.UserControls;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.ScrappedDetails;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.DetailsFragments
{
    public class PersonDetailsPageVaTabFragment : MalFragmentBase
    {
        private StaffDetailsViewModel ViewModel = ViewModelLocator.StaffDetails;
        private GridViewColumnHelper _gridViewColumnHelper;

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            //AnimeDetailsPageCharactersTabGridView.DisableAdjust = true;
            //_gridViewColumnHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1,null,true);
            AnimeDetailsPageCharactersTabLoadingSpinner.Visibility = ViewStates.Gone;

            //Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            //{
            //    if(ViewModel.Data == null)
            //        return;

            //    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.Data.ShowCharacterPairs,DataTemplateFull,DataTemplateFling, ContainerTemplate, DataTemplateBasic );
            //}));

            //_gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1);
            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                //if (ViewModel.AnimeStaffData == null)
                //    AnimeDetailsPageCharactersTabGridView.Adapter = null;
                //else
                //    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeCharacterPairs, DataTemplateFull, DataTemplateFling, ContainerTemplate);

                if (ViewModel.Data == null)
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(null);
                else
                {
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(
                        new ObservableRecyclerAdapter<
                            ShowCharacterPair, Holder>(
                            ViewModel.Data.ShowCharacterPairs,
                            DataTemplate,
                            ItemTemplate));

                    if (ViewModel.Data.StaffPositions?.Count == 0)
                    {
                        AnimeDetailsPageCharactersTabEmptyNotice.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        AnimeDetailsPageCharactersTabEmptyNotice.Visibility = ViewStates.Gone;
                    }
                }
            }));

            AnimeDetailsPageCharactersTabGridView.SetLayoutManager(new GridLayoutManager(Activity, 2));

            //AnimeDetailsPageCharactersTabGridView.EmptyView = AnimeDetailsPageCharactersTabEmptyNotice;
        }

        private View ItemTemplate(int viewtype)
        {
            var view = new LinearLayout(Activity);

            var param = new LinearLayout.LayoutParams(0, -2) { Weight = 1 };

            var animeEntry = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeLightItem, null);
            animeEntry.LayoutParameters = param;
            animeEntry.Id = Resource.Layout.AnimeLightItem;
            animeEntry.Click += AnimeEntryOnClick;

            var characterEntry = Activity.LayoutInflater.Inflate(Resource.Layout.FavouriteItem, null);
            characterEntry.LayoutParameters = param;
            characterEntry.Id = Resource.Layout.FavouriteItem;
            characterEntry.Click += CharacterEntryOnClick;

            view.AddView(animeEntry);
            view.AddView(characterEntry);

            return view;
        }

        private void DataTemplate(ShowCharacterPair item, Holder holder, int position)
        {
            var view = holder.ItemView;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text =
                item.AnimeLightEntry.Title;
            view.FindViewById(Resource.Layout.AnimeLightItem).Tag = item.AnimeLightEntry.Wrap();

            view.FindViewById<TextView>(Resource.Id.FavouriteItemName).Text =
                item.AnimeCharacter.Name;
            view.FindViewById<TextView>(Resource.Id.FavouriteItemRole).Text =
                item.AnimeCharacter.Notes;
            view.FindViewById(Resource.Layout.FavouriteItem).Tag = item.AnimeCharacter.Wrap();

            var image = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (image.Tag == null || (string)image.Tag != item.AnimeLightEntry.ImgUrl)
            {
                image.Into(item.AnimeLightEntry.ImgUrl);
            }
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;


            image = view.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage);
            if (image.Tag == null || (string)image.Tag != item.AnimeCharacter.ImgUrl)
            {
                image.Into(item.AnimeCharacter.ImgUrl);
            }
            view.FindViewById(Resource.Id.FavouriteItemImgPlaceholder).Visibility = ViewStates.Gone;
        }

        private void CharacterEntryOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateCharacterDetailsCommand.Execute((sender as View).Tag.Unwrap<AnimeCharacter>());
        }

        private void AnimeEntryOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateAnimeDetailsCommand.Execute((sender as View).Tag.Unwrap<AnimeLightEntry>());
        }


        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

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


        #region Views

        private RecyclerView _animeDetailsPageCharactersTabGridView;
        private TextView _animeDetailsPageCharactersTabEmptyNotice;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public RecyclerView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<RecyclerView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public TextView AnimeDetailsPageCharactersTabEmptyNotice => _animeDetailsPageCharactersTabEmptyNotice ?? (_animeDetailsPageCharactersTabEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeDetailsPageCharactersTabEmptyNotice));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));


        #endregion
    }
}