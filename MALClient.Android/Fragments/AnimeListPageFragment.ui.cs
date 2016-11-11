using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Models.Enums;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;

namespace MALClient.Android.Fragments
{
    public partial class AnimeListPageFragment : MalFragmentBase
    {
        private DroppyMenuPopup _sortingMenu;
        private DroppyMenuPopup _filterMenu;

        protected override void InitBindings()
        {






            Bindings = new Dictionary<int, List<Binding>>();

            Bindings.Add(Resource.Id.AnimeListPageLoadingSpinner, new List<Binding>());
            Bindings[Resource.Id.AnimeListPageLoadingSpinner].Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => AnimeListPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            ViewModel.PropertyChanged += AnimeListOnPropertyChanged;
            AnimeListPageReloadButton.SetCommand("Click",ViewModel.RefreshCommand);
            AnimeListPageFilterMenu.SetCommand("Click",new RelayCommand(ShowFilterMenu));
            AnimeListPageSortMenu.SetCommand("Click", new RelayCommand(ShowSortMenu));
        }

        private void ShowSortMenu()
        {
            _sortingMenu = DroppyFlyoutBuilder.BuildForAnimeSortingSelection(MainActivity.CurrentContext,
                AnimeListPageSortMenu,
                OnSortingMenuSelectionChanged, ViewModel.SortOption);
            _sortingMenu.Show();
        }

        private void ShowFilterMenu()
        {
            _filterMenu = DroppyFlyoutBuilder.BuildForAnimeStatusSelection(MainActivity.CurrentContext,
                AnimeListPageFilterMenu,
                OnFilterMenuSelectionChanged, (AnimeStatus) ViewModel.CurrentStatus,ViewModel.IsMangaWorkMode);
            _filterMenu.Show();
        }

        private void OnSortingMenuSelectionChanged(SortOptions option)
        {
            ViewModel.SetSortOrder(option);
            ViewModel.RefreshList();
            _sortingMenu.Dismiss(true);
            _sortingMenu = null;
        }

        private void OnFilterMenuSelectionChanged(AnimeStatus status)
        {
            ViewModel.CurrentStatus = (int)status;
            ViewModel.RefreshList();
            _filterMenu.Dismiss(true);
            _filterMenu = null;
        }

        private async void AnimeListOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.AnimeList.AnimeGridItems))
            {
                if(ViewModelLocator.AnimeList.AnimeGridItems != null)
                    AnimeListPageGridView.Adapter = new AnimeListItemsAdapter(Context as Activity, Resource.Layout.AnimeGridItem, ViewModelLocator.AnimeList.AnimeGridItems);
            }
        }

        private GridView _animeListPageGridView;
        private ProgressBar _animeListPageLoadingSpinner;
        private ImageButton _animeListPageReloadButton;
        private ImageButton _animeListPageFilterMenu;
        private ImageButton _animeListPageSortMenu;

        public GridView AnimeListPageGridView => _animeListPageGridView ?? (_animeListPageGridView = FindViewById<GridView>(Resource.Id.AnimeListPageGridView));

        public ProgressBar AnimeListPageLoadingSpinner => _animeListPageLoadingSpinner ?? (_animeListPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeListPageLoadingSpinner));

        public ImageButton AnimeListPageReloadButton => _animeListPageReloadButton ?? (_animeListPageReloadButton = FindViewById<ImageButton>(Resource.Id.AnimeListPageReloadButton));

        public ImageButton AnimeListPageFilterMenu => _animeListPageFilterMenu ?? (_animeListPageFilterMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageFilterMenu));

        public ImageButton AnimeListPageSortMenu => _animeListPageSortMenu ?? (_animeListPageSortMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageSortMenu));







    }
}