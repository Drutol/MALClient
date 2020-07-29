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
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.DetailsFragments
{
    public class CharacterDetailsPageFragment : MalFragmentBase
    {
        private readonly CharacterDetailsNavigationArgs _args;
        private CharacterDetailsViewModel ViewModel;
        private readonly GridViewColumnHelper _gridViewColumnHelper = new GridViewColumnHelper(null,true) {MinColumns = 2, PrefferedItemWidth = 170};

        public CharacterDetailsPageFragment(CharacterDetailsNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.CharacterDetails;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            CharacterDetailsPageAnimeographyGrid.AlwaysAdjust =
                CharacterDetailsPageMangaographyGrid.AlwaysAdjust =
                    CharacterDetailsPageVoiceActorsGrid.AlwaysAdjust = true;

            _gridViewColumnHelper.RegisterGrid(CharacterDetailsPageVoiceActorsGrid);
            _gridViewColumnHelper.RegisterGrid(CharacterDetailsPageMangaographyGrid);
            _gridViewColumnHelper.RegisterGrid(CharacterDetailsPageAnimeographyGrid);

            Bindings.Add(this.SetBinding(() => ViewModel.AnimeographyVisibility).WhenSourceChanges(() =>
            {
                if (ViewModel.AnimeographyVisibility)
                {
                    CharacterDetailsPageAnimeographyGrid.Visibility = ViewStates.Visible;
                    CharacterDetailsPageAnimeographyEmptyNotice.Visibility = ViewStates.Gone;
                }
                else
                {
                    CharacterDetailsPageAnimeographyGrid.Visibility = ViewStates.Gone;
                    CharacterDetailsPageAnimeographyEmptyNotice.Visibility = ViewStates.Visible;
                }
            }));
            Bindings.Add(this.SetBinding(() => ViewModel.MangaographyVisibility).WhenSourceChanges(() =>
            {
                if (ViewModel.MangaographyVisibility)
                {
                    CharacterDetailsPageMangaographyGrid.Visibility = ViewStates.Visible;
                    CharacterDetailsPageMangaographyEmptyNotice.Visibility = ViewStates.Gone;
                }                       
                else                    
                {                        
                    CharacterDetailsPageMangaographyGrid.Visibility = ViewStates.Gone;
                    CharacterDetailsPageMangaographyEmptyNotice.Visibility = ViewStates.Visible;
                }
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => CharacterDetailsPageLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.VoiceActors).WhenSourceChanges(() =>
            {
                if (ViewModel.VoiceActors == null)
                    CharacterDetailsPageVoiceActorsGrid.Adapter = null;
                else
                    CharacterDetailsPageVoiceActorsGrid.Adapter = ViewModel.VoiceActors.GetAdapter(GetTemplateDelegate);
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if (ViewModel.VoiceActors == null)
                    CharacterDetailsPageVoiceActorsGrid.Adapter = null;
                else
                    CharacterDetailsPageVoiceActorsGrid.Adapter = ViewModel.VoiceActors.GetAdapter(GetTemplateDelegate);
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.SpoilerButtonVisibility,
                    () => CharacterDetailsPageSpoilerButton.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if(ViewModel.Data == null)
                    return;

                CharacterDetailsPageDescription.Text = ViewModel.Data.Content;
                if (string.IsNullOrWhiteSpace(ViewModel.Data.ImgUrl))
                    CharactersDetailsPageNoImgNotice.Visibility = ViewStates.Visible;
                else
                {
                    CharactersDetailsPageNoImgNotice.Visibility = ViewStates.Gone;
                    CharacterDetailsPageImage.Into(ViewModel.Data.ImgUrl);
                }
                CharactersPageFavButton.BindModel(ViewModel.FavouriteViewModel);

                if(ViewModel.Data.Animeography.Any())
                    CharacterDetailsPageAnimeographyGrid.Adapter =
                        ViewModel.Data.Animeography.GetAdapter(GetTemplateDelegate);
                if (ViewModel.Data.Mangaography.Any())
                    CharacterDetailsPageMangaographyGrid.Adapter =
                        ViewModel.Data.Mangaography.GetAdapter(GetTemplateDelegate);
            }));

            CharacterDetailsPageVoiceActorsGrid.EmptyView = CharacterDetailsPageVoiceActorsEmptyNotice;

            CharacterDetailsPageSpoilerButton.SetOnClickListener(new OnClickListener(view => CharacterDetailsPageSpoilerButtonOnClick()));
            CharacterDetailsPageLinkButton.SetOnClickListener(new OnClickListener(view => ViewModel.OpenInMalCommand.Execute(null)));
        }

        private void CharacterDetailsPageSpoilerButtonOnClick()
        {
            ResourceLocator.MessageDialogProvider.ShowMessageDialog(ViewModel.Data.SpoilerContent,"Spoiler"); 
        }

        private View GetTemplateDelegate(int i, AnimeLightEntry animeLightEntry, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeLightItem, null);
                view.Click += LightItemOnClick;
            }
            view.Tag = animeLightEntry.Wrap();

            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = animeLightEntry.Title;
            view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage).Into(animeLightEntry.ImgUrl);

            return view;
        }

        private void LightItemOnClick(object sender, EventArgs eventArgs)
        {
            var model = (sender as View).Tag.Unwrap<AnimeLightEntry>();
            if(model.IsAnime)
                ViewModel.NavigateAnimeDetailsCommand.Execute(model);
            else
                ViewModel.NavigateMangaDetailsCommand.Execute(model);
        }

        private View GetTemplateDelegate(int i, FavouriteViewModel favouriteViewModel, View arg3)
        {
            var view = arg3;
            var firstRun = false;
            if (view == null)
            {
                view = new FavouriteItem(Activity,true);
                firstRun = true;
            }

            (view as FavouriteItem).BindModel(favouriteViewModel,false);
            if (firstRun)
            {
                ((FavouriteItem)view).Click += PersonOnClick;
            }
            return view;
        }

        private void PersonOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateStaffDetailsCommand.Execute((sender as View).Tag.Unwrap<FavouriteViewModel>().Data);
        }

        public override int LayoutResourceId => Resource.Layout.CharacterDetailsPage;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        #region Views

        private ImageView _charactersDetailsPageNoImgNotice;
        private ImageViewAsync _characterDetailsPageImage;
        private ImageButton _characterDetailsPageLinkButton;
        private FavouriteButton _charactersPageFavButton;
        private TextView _characterDetailsPageDescription;
        private Button _characterDetailsPageSpoilerButton;
        private HeightAdjustingGridView _characterDetailsPageVoiceActorsGrid;
        private TextView _characterDetailsPageVoiceActorsEmptyNotice;
        private HeightAdjustingGridView _characterDetailsPageAnimeographyGrid;
        private TextView _characterDetailsPageAnimeographyEmptyNotice;
        private HeightAdjustingGridView _characterDetailsPageMangaographyGrid;
        private TextView _characterDetailsPageMangaographyEmptyNotice;
        private RelativeLayout _characterDetailsPageLoadingSpinner;

        public ImageView CharactersDetailsPageNoImgNotice => _charactersDetailsPageNoImgNotice ?? (_charactersDetailsPageNoImgNotice = FindViewById<ImageView>(Resource.Id.CharactersDetailsPageNoImgNotice));

        public ImageViewAsync CharacterDetailsPageImage => _characterDetailsPageImage ?? (_characterDetailsPageImage = FindViewById<ImageViewAsync>(Resource.Id.CharacterDetailsPageImage));

        public ImageButton CharacterDetailsPageLinkButton => _characterDetailsPageLinkButton ?? (_characterDetailsPageLinkButton = FindViewById<ImageButton>(Resource.Id.CharacterDetailsPageLinkButton));

        public FavouriteButton CharactersPageFavButton => _charactersPageFavButton ?? (_charactersPageFavButton = FindViewById<FavouriteButton>(Resource.Id.CharactersPageFavButton));

        public TextView CharacterDetailsPageDescription => _characterDetailsPageDescription ?? (_characterDetailsPageDescription = FindViewById<TextView>(Resource.Id.CharacterDetailsPageDescription));

        public Button CharacterDetailsPageSpoilerButton => _characterDetailsPageSpoilerButton ?? (_characterDetailsPageSpoilerButton = FindViewById<Button>(Resource.Id.CharacterDetailsPageSpoilerButton));

        public HeightAdjustingGridView CharacterDetailsPageVoiceActorsGrid => _characterDetailsPageVoiceActorsGrid ?? (_characterDetailsPageVoiceActorsGrid = FindViewById<HeightAdjustingGridView>(Resource.Id.CharacterDetailsPageVoiceActorsGrid));

        public TextView CharacterDetailsPageVoiceActorsEmptyNotice => _characterDetailsPageVoiceActorsEmptyNotice ?? (_characterDetailsPageVoiceActorsEmptyNotice = FindViewById<TextView>(Resource.Id.CharacterDetailsPageVoiceActorsEmptyNotice));

        public HeightAdjustingGridView CharacterDetailsPageAnimeographyGrid => _characterDetailsPageAnimeographyGrid ?? (_characterDetailsPageAnimeographyGrid = FindViewById<HeightAdjustingGridView>(Resource.Id.CharacterDetailsPageAnimeographyGrid));

        public TextView CharacterDetailsPageAnimeographyEmptyNotice => _characterDetailsPageAnimeographyEmptyNotice ?? (_characterDetailsPageAnimeographyEmptyNotice = FindViewById<TextView>(Resource.Id.CharacterDetailsPageAnimeographyEmptyNotice));

        public HeightAdjustingGridView CharacterDetailsPageMangaographyGrid => _characterDetailsPageMangaographyGrid ?? (_characterDetailsPageMangaographyGrid = FindViewById<HeightAdjustingGridView>(Resource.Id.CharacterDetailsPageMangaographyGrid));

        public TextView CharacterDetailsPageMangaographyEmptyNotice => _characterDetailsPageMangaographyEmptyNotice ?? (_characterDetailsPageMangaographyEmptyNotice = FindViewById<TextView>(Resource.Id.CharacterDetailsPageMangaographyEmptyNotice));

        public RelativeLayout CharacterDetailsPageLoadingSpinner => _characterDetailsPageLoadingSpinner ?? (_characterDetailsPageLoadingSpinner = FindViewById<RelativeLayout>(Resource.Id.CharacterDetailsPageLoadingSpinner));



        #endregion
    }


}