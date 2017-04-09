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
            _currentTab = Resource.Id.ProfilePageFavouritesTabAnimeBtn;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavAnime))
            {
                if(_currentTab == Resource.Id.ProfilePageFavouritesTabAnimeBtn)
                    UpdateGridView();
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavManga))
            {
                if (_currentTab == Resource.Id.ProfilePageFavouritesTabMangaBtn)
                    UpdateGridView();
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavouriteCharacters))
            {
                if (_currentTab == Resource.Id.ProfilePageFavouritesTabCharsBtn)
                    UpdateGridView();
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.FavouriteStaff))
            {
                if (_currentTab == Resource.Id.ProfilePageFavouritesTabPplBtn)
                    UpdateGridView();
            }
        }

        protected override void InitBindings()
        {
            _helper = new GridViewColumnHelper(ProfilePageFavouritesTabGridView);

            var listener = new OnClickListener(OnTabSelected);
            ProfilePageFavouritesTabAnimeBtn.SetOnClickListener(listener);
            ProfilePageFavouritesTabMangaBtn.SetOnClickListener(listener);
            ProfilePageFavouritesTabCharsBtn.SetOnClickListener(listener);
            ProfilePageFavouritesTabPplBtn.SetOnClickListener(listener);
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
                case Resource.Id.ProfilePageFavouritesTabAnimeBtn:
                    if (ViewModel.FavAnime?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.InjectAnimeListAdapter(Context, ViewModel.FavAnime,
                            AnimeListDisplayModes.IndefiniteGrid, OnItemClickAction,false);
                    break;
                case Resource.Id.ProfilePageFavouritesTabMangaBtn:
                    if (ViewModel.FavManga?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.InjectAnimeListAdapter(Context, ViewModel.FavManga,
                            AnimeListDisplayModes.IndefiniteGrid, OnItemClickAction,false);
                    break;
                case Resource.Id.ProfilePageFavouritesTabCharsBtn:
                    if (ViewModel.FavouriteCharacters?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.Adapter =
                            ViewModel.FavouriteCharacters.GetAdapter(GetTemplateDelegate);
                    break;
                case Resource.Id.ProfilePageFavouritesTabPplBtn:
                    if (ViewModel.FavouriteStaff?.Any() ?? false)
                        ProfilePageFavouritesTabGridView.Adapter =
                            ViewModel.FavouriteStaff.GetAdapter(GetTemplateDelegate);
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
            var model = (sender as View).Tag.Unwrap<FavouriteBase>();
            if (model.Type == FavouriteType.Character)
            {
                ViewModel.NavigateCharacterDetailsCommand.Execute(model);
            }
            else
            {
                ViewModel.NavigateStaffDetailsCommand.Execute(model);
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
        private FrameLayout _profilePageFavouritesTabAnimeBtn;
        private FrameLayout _profilePageFavouritesTabMangaBtn;
        private FrameLayout _profilePageFavouritesTabCharsBtn;
        private FrameLayout _profilePageFavouritesTabPplBtn;

        public GridView ProfilePageFavouritesTabGridView => _profilePageFavouritesTabGridView ?? (_profilePageFavouritesTabGridView = FindViewById<GridView>(Resource.Id.ProfilePageFavouritesTabGridView));

        public FrameLayout ProfilePageFavouritesTabAnimeBtn => _profilePageFavouritesTabAnimeBtn ?? (_profilePageFavouritesTabAnimeBtn = FindViewById<FrameLayout>(Resource.Id.ProfilePageFavouritesTabAnimeBtn));

        public FrameLayout ProfilePageFavouritesTabMangaBtn => _profilePageFavouritesTabMangaBtn ?? (_profilePageFavouritesTabMangaBtn = FindViewById<FrameLayout>(Resource.Id.ProfilePageFavouritesTabMangaBtn));

        public FrameLayout ProfilePageFavouritesTabCharsBtn => _profilePageFavouritesTabCharsBtn ?? (_profilePageFavouritesTabCharsBtn = FindViewById<FrameLayout>(Resource.Id.ProfilePageFavouritesTabCharsBtn));

        public FrameLayout ProfilePageFavouritesTabPplBtn => _profilePageFavouritesTabPplBtn ?? (_profilePageFavouritesTabPplBtn = FindViewById<FrameLayout>(Resource.Id.ProfilePageFavouritesTabPplBtn));

        #endregion
    }
}