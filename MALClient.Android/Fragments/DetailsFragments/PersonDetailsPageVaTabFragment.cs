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
            AnimeDetailsPageCharactersTabGridView.DisableAdjust = true;
            _gridViewColumnHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1);
            AnimeDetailsPageCharactersTabLoadingSpinner.Visibility = ViewStates.Gone;

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if(ViewModel.Data == null)
                    return;

                AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.Data.ShowCharacterPairs,DataTemplateFull,DataTemplateFling, ContainerTemplate );
            }));
        }

        private View ContainerTemplate()
        {
            var view = new LinearLayout(Activity);

            var param = new LinearLayout.LayoutParams(0, -2) {Weight = 1};

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

        private void CharacterEntryOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateCharacterDetailsCommand.Execute((sender as View).Tag.Unwrap<AnimeCharacter>());
        }

        private void AnimeEntryOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateAnimeDetailsCommand.Execute((sender as View).Tag.Unwrap<AnimeLightEntry>());
        }

        private void DataTemplateFling(View view, ShowCharacterPair showCharacterPair)
        {
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Visible;
            view.FindViewById(Resource.Id.AnimeLightItemImage).Visibility = ViewStates.Invisible;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text =
                showCharacterPair.AnimeLightEntry.Title;
            view.FindViewById(Resource.Layout.AnimeLightItem).Tag = showCharacterPair.AnimeLightEntry.Wrap();

            view.FindViewById(Resource.Id.FavouriteItemImage).Visibility = ViewStates.Invisible;
            view.FindViewById(Resource.Id.FavouriteItemImagePlaceholder).Visibility = ViewStates.Visible;
            view.FindViewById<TextView>(Resource.Id.FavouriteItemName).Text =
                showCharacterPair.AnimeCharacter.Name;
            view.FindViewById<TextView>(Resource.Id.FavouriteItemRole).Text =
                showCharacterPair.AnimeCharacter.Notes;
            view.FindViewById(Resource.Layout.FavouriteItem).Tag = showCharacterPair.AnimeCharacter.Wrap();
        }

        private void DataTemplateFull(View view, ShowCharacterPair showCharacterPair)
        {
            var image = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (image.Tag == null || (string) image.Tag != showCharacterPair.AnimeLightEntry.ImgUrl)
            {
                image.Into(showCharacterPair.AnimeLightEntry.ImgUrl);
                image.Tag = showCharacterPair.AnimeLightEntry.ImgUrl;
            }
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text =
                showCharacterPair.AnimeLightEntry.Title;
            view.FindViewById(Resource.Layout.AnimeLightItem).Tag = showCharacterPair.AnimeLightEntry.Wrap();

            image = view.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage);
            if (image.Tag == null || (string)image.Tag != showCharacterPair.AnimeCharacter.ImgUrl)
            {
                view.FindViewById(Resource.Id.FavouriteItemImagePlaceholder).Visibility = ViewStates.Gone;
                image.Into(showCharacterPair.AnimeCharacter.ImgUrl, null, img => img.HandleScaling());
                image.Tag = showCharacterPair.AnimeCharacter.ImgUrl;
            }

            view.FindViewById<TextView>(Resource.Id.FavouriteItemName).Text =
                showCharacterPair.AnimeCharacter.Name;
            view.FindViewById<TextView>(Resource.Id.FavouriteItemRole).Text =
                showCharacterPair.AnimeCharacter.Notes;
            view.FindViewById(Resource.Layout.FavouriteItem).Tag = showCharacterPair.AnimeCharacter.Wrap();
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;

        #region Views

        private HeightAdjustingGridView _animeDetailsPageCharactersTabGridView;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public HeightAdjustingGridView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<HeightAdjustingGridView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion
    }
}