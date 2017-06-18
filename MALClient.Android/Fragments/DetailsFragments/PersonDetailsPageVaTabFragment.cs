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
            _gridViewColumnHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1,null,true);
            AnimeDetailsPageCharactersTabLoadingSpinner.Visibility = ViewStates.Gone;

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if(ViewModel.Data == null)
                    return;

                AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.Data.ShowCharacterPairs,DataTemplateFull,DataTemplateFling, ContainerTemplate, DataTemplateBasic );
            }));

            AnimeDetailsPageCharactersTabGridView.EmptyView = AnimeDetailsPageCharactersTabEmptyNotice;
        }

        private void DataTemplateBasic(View view, int i, ShowCharacterPair showCharacterPair)
        {

            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text =
                showCharacterPair.AnimeLightEntry.Title;
            view.FindViewById(Resource.Layout.AnimeLightItem).Tag = showCharacterPair.AnimeLightEntry.Wrap();

            view.FindViewById<TextView>(Resource.Id.FavouriteItemName).Text =
                showCharacterPair.AnimeCharacter.Name;
            view.FindViewById<TextView>(Resource.Id.FavouriteItemRole).Text =
                showCharacterPair.AnimeCharacter.Notes;
            view.FindViewById(Resource.Layout.FavouriteItem).Tag = showCharacterPair.AnimeCharacter.Wrap();
        }

        private View ContainerTemplate(int i)
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

        private void DataTemplateFling(View view, int i, ShowCharacterPair showCharacterPair)
        {

            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (img.IntoIfLoaded(showCharacterPair.AnimeLightEntry.ImgUrl))
            {
                view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;
            }
            else
            {
                img.Visibility = ViewStates.Invisible;
                view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Visible;
            }


            img = view.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage);
            if (img.IntoIfLoaded(showCharacterPair.AnimeCharacter.ImgUrl))
            {
                view.FindViewById(Resource.Id.FavouriteItemImgPlaceholder).Visibility = ViewStates.Gone;
            }
            else
            {
                img.Visibility = ViewStates.Invisible;
                view.FindViewById(Resource.Id.FavouriteItemImgPlaceholder).Visibility = ViewStates.Visible;

            }
        }

        private void DataTemplateFull(View view, int i, ShowCharacterPair showCharacterPair)
        {
            var image = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (image.Tag == null || (string) image.Tag != showCharacterPair.AnimeLightEntry.ImgUrl)
            {
                image.Into(showCharacterPair.AnimeLightEntry.ImgUrl);
            }
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;


            image = view.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage);
            if (image.Tag == null || (string) image.Tag != showCharacterPair.AnimeCharacter.ImgUrl)
            {
                image.Into(showCharacterPair.AnimeCharacter.ImgUrl, null, img => img.HandleScaling());
            }
            view.FindViewById(Resource.Id.FavouriteItemImgPlaceholder).Visibility = ViewStates.Gone;
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        #region Views

        private HeightAdjustingGridView _animeDetailsPageCharactersTabGridView;
        private TextView _animeDetailsPageCharactersTabEmptyNotice;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public HeightAdjustingGridView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<HeightAdjustingGridView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public TextView AnimeDetailsPageCharactersTabEmptyNotice => _animeDetailsPageCharactersTabEmptyNotice ?? (_animeDetailsPageCharactersTabEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeDetailsPageCharactersTabEmptyNotice));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));


        #endregion
    }
}