using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MALClient.Android.Activities;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageFavouritesTabFragment : MalFragmentBase
    {
        private int _currentTab;
        private GridViewColumnHelper _helper;

        private ProfilePageViewModel ViewModel = ViewModelLocator.ProfilePage;

        public ProfilePageFavouritesTabFragment() : base(true, false)
        {
            _currentTab = Resource.Id.ProfilePageFavouritesTabAnimeToggleButton;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavAnime))
            {
                if(_currentTab == Resource.Id.ProfilePageFavouritesTabAnimeToggleButton)
                    UpdateGridView();
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavManga))
            {
                if (_currentTab == Resource.Id.ProfilePageFavouritesTabMangaToggleButton)
                    UpdateGridView();
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavouriteCharacters))
            {
                if (_currentTab == Resource.Id.ProfilePageFavouritesTabCharactersToggleButton)
                    UpdateGridView();
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavouriteStaff))
            {
                if (_currentTab == Resource.Id.ProfilePageFavouritesTabPeopleToggleButton)
                    UpdateGridView();
            }
        }

        protected override void InitBindings()
        {
            _helper = new GridViewColumnHelper(ProfilePageFavouritesTabGridView,null,2,3);

            var listener = new OnClickListener(OnTabSelected);
            ProfilePageFavouritesTabAnimeToggleButton.SetOnClickListener(listener);
            ProfilePageFavouritesTabMangaToggleButton.SetOnClickListener(listener);
            ProfilePageFavouritesTabCharactersToggleButton.SetOnClickListener(listener);
            ProfilePageFavouritesTabPeopleToggleButton.SetOnClickListener(listener);
            UpdateGridView();

            
        }

        private void OnTabSelected(View view)
        {
            _currentTab = view.Id;
            UpdateGridView();
        }


        private void UpdateGridView()
        {
            ProfilePageFavouritesTabGridView.ClearFlingAdapter();
            switch (_currentTab)
            {
                case Resource.Id.ProfilePageFavouritesTabAnimeToggleButton:
                    if (ViewModel.FavAnime?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.InjectAnimeListAdapter(Context, ViewModel.FavAnime,
                            AnimeListDisplayModes.IndefiniteGrid, OnItemClickAction,false);

                    ProfilePageFavouritesTabAnimeToggleButton.Checked = true;
                    ProfilePageFavouritesTabMangaToggleButton.Checked =
                        ProfilePageFavouritesTabCharactersToggleButton.Checked =
                            ProfilePageFavouritesTabPeopleToggleButton.Checked = false;
                    break;
                case Resource.Id.ProfilePageFavouritesTabMangaToggleButton:
                    if (ViewModel.FavManga?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.InjectAnimeListAdapter(Context, ViewModel.FavManga,
                            AnimeListDisplayModes.IndefiniteGrid, OnItemClickAction,false);

                    ProfilePageFavouritesTabMangaToggleButton.Checked = true;
                    ProfilePageFavouritesTabAnimeToggleButton.Checked =
                        ProfilePageFavouritesTabCharactersToggleButton.Checked =
                            ProfilePageFavouritesTabPeopleToggleButton.Checked = false;
                    break;
                case Resource.Id.ProfilePageFavouritesTabCharactersToggleButton:
                    if (ViewModel.FavouriteCharacters?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.Adapter =
                            ViewModel.FavouriteCharacters.GetAdapter(GetTemplateDelegate);

                    ProfilePageFavouritesTabCharactersToggleButton.Checked = true;
                    ProfilePageFavouritesTabMangaToggleButton.Checked =
                        ProfilePageFavouritesTabAnimeToggleButton.Checked =
                            ProfilePageFavouritesTabPeopleToggleButton.Checked = false;
                    break;
                case Resource.Id.ProfilePageFavouritesTabPeopleToggleButton:
                    if (ViewModel.FavouriteStaff?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.Adapter =
                            ViewModel.FavouriteStaff.GetAdapter(GetTemplateDelegate);

                    ProfilePageFavouritesTabPeopleToggleButton.Checked = true;
                    ProfilePageFavouritesTabMangaToggleButton.Checked =
                        ProfilePageFavouritesTabCharactersToggleButton.Checked =
                            ProfilePageFavouritesTabAnimeToggleButton.Checked = false;
                    break;
            }
        }

        private void OnItemClickAction(AnimeItemViewModel animeItemViewModel)
        {
            ViewModel.TemporarilySelectedAnimeItem = animeItemViewModel;
        }

        private View GetTemplateDelegate(int i, FavouriteViewModel favouriteViewModel, View convertView)
        {

            var view = convertView;
            if (view == null)
            {
                view = new FavouriteItem(Activity);
                ((FavouriteItem) view).BindModel(favouriteViewModel, false);
                ((FavouriteItem)view).Click += FavItemOnClick;
            }
            else
            {
                ((FavouriteItem)view).BindModel(favouriteViewModel, false);
            }
            return view;
        }

        private void FavItemOnClick(object sender, EventArgs eventArgs)
        {
            var model = (sender as View).Tag.Unwrap<FavouriteViewModel>();
            if (model.Data.Type == FavouriteType.Character)
            {
                ViewModel.NavigateCharacterDetailsCommand.Execute(model.Data);
            }
            else
            {
                ViewModel.NavigateStaffDetailsCommand.Execute(model.Data);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _helper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }


        public override int LayoutResourceId => Resource.Layout.ProfilePageFavouritesTab;

        #region Views

        private GridView _profilePageFavouritesTabGridView;
        private ToggleButton _profilePageFavouritesTabAnimeToggleButton;
        private ToggleButton _profilePageFavouritesTabMangaToggleButton;
        private ToggleButton _profilePageFavouritesTabCharactersToggleButton;
        private ToggleButton _profilePageFavouritesTabPeopleToggleButton;

        public GridView ProfilePageFavouritesTabGridView => _profilePageFavouritesTabGridView ?? (_profilePageFavouritesTabGridView = FindViewById<GridView>(Resource.Id.ProfilePageFavouritesTabGridView));

        public ToggleButton ProfilePageFavouritesTabAnimeToggleButton => _profilePageFavouritesTabAnimeToggleButton ?? (_profilePageFavouritesTabAnimeToggleButton = FindViewById<ToggleButton>(Resource.Id.ProfilePageFavouritesTabAnimeToggleButton));

        public ToggleButton ProfilePageFavouritesTabMangaToggleButton => _profilePageFavouritesTabMangaToggleButton ?? (_profilePageFavouritesTabMangaToggleButton = FindViewById<ToggleButton>(Resource.Id.ProfilePageFavouritesTabMangaToggleButton));

        public ToggleButton ProfilePageFavouritesTabCharactersToggleButton => _profilePageFavouritesTabCharactersToggleButton ?? (_profilePageFavouritesTabCharactersToggleButton = FindViewById<ToggleButton>(Resource.Id.ProfilePageFavouritesTabCharactersToggleButton));

        public ToggleButton ProfilePageFavouritesTabPeopleToggleButton => _profilePageFavouritesTabPeopleToggleButton ?? (_profilePageFavouritesTabPeopleToggleButton = FindViewById<ToggleButton>(Resource.Id.ProfilePageFavouritesTabPeopleToggleButton));


        #endregion
    }
}