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
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.Android.BindingConverters;
using MALClient.Android.BindingInformation;
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
        private DroppyMenuPopup _displayMenu;

        protected override void InitBindings()
        {
            _gridVievColumnHelper = new GridViewColumnHelper(AnimeListPageGridView);

            Bindings.Add(Resource.Id.AnimeListPageLoadingSpinner, new List<Binding>());
            Bindings[Resource.Id.AnimeListPageLoadingSpinner].Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => AnimeListPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            ViewModel.PropertyChanged += AnimeListOnPropertyChanged;
            AnimeListPageReloadButton.SetCommand("Click",ViewModel.RefreshCommand);
            AnimeListPageFilterMenu.SetCommand("Click",new RelayCommand(ShowFilterMenu));
            AnimeListPageSortMenu.SetCommand("Click", new RelayCommand(ShowSortMenu));
            AnimeListPageDisplayMenu.SetCommand("Click", new RelayCommand(ShowDisplayMenu));
        }



        private void ShowSortMenu()
        {
            _sortingMenu = AnimeListPageFlyoutBuilder.BuildForAnimeSortingSelection(MainActivity.CurrentContext,
                AnimeListPageSortMenu,
                OnSortingMenuSelectionChanged, ViewModel.SortOption);
            _sortingMenu.Show();
        }

        private void ShowFilterMenu()
        {
            _filterMenu = AnimeListPageFlyoutBuilder.BuildForAnimeStatusSelection(MainActivity.CurrentContext,
                AnimeListPageFilterMenu,
                OnFilterMenuSelectionChanged, (AnimeStatus) ViewModel.CurrentStatus,ViewModel.IsMangaWorkMode);
            _filterMenu.Show();
        }

        private void ShowDisplayMenu()
        {
            _displayMenu = AnimeListPageFlyoutBuilder.BuildForAnimeListDisplayModeSelection(MainActivity.CurrentContext,
                AnimeListPageDisplayMenu, ViewModel.DisplayModes, OnDisplayModeSelectionChanged, ViewModel.DisplayMode);
            _displayMenu.Show();
        }

        private void OnDisplayModeSelectionChanged(AnimeListDisplayModes animeListDisplayModes)
        {
            ViewModel.CurrentlySelectedDisplayMode = new Tuple<AnimeListDisplayModes, string>(animeListDisplayModes,"");
            _displayMenu.Dismiss(true);
            _displayMenu = null;
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
                if (ViewModelLocator.AnimeList.AnimeGridItems != null)
                {
                    AnimeListPageGridView.Adapter = new AnimeListItemsAdapter(Activity,
                        Resource.Layout.AnimeGridItem, ViewModelLocator.AnimeList.AnimeGridItems,
                        (model, view) =>
                            new AnimeGridItemBindingInfo(view, model)
                            {
                                OnItemClickAction = AnimeListPageGridViewOnItemClick
                            });
                    if (_prevArgs != null)
                    {
                        var pos = _prevArgs.SelectedItemIndex;
                        await Task.Delay(300);
                        AnimeListPageGridView.RequestFocusFromTouch();
                        AnimeListPageGridView.SetSelection(pos);
                        AnimeListPageGridView.RequestFocus();
                        _prevArgs = null;
                    }
                    AnimeListPageListView.Adapter = null;
                    AnimeListPageCompactListView.Adapter = null;
                }
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.AnimeList.AnimeListItems))
            {
                if (ViewModelLocator.AnimeList.AnimeListItems != null)
                {
                    AnimeListPageListView.Adapter = new AnimeListItemsAdapter(Activity,
                        Resource.Layout.AnimeListItem, ViewModelLocator.AnimeList.AnimeListItems,(model, view) => new AnimeListItemBindingInfo(view,model)
                        {
                            OnItemClickAction = AnimeListPageGridViewOnItemClick
                        });
                    if (_prevArgs != null)
                    {
                        AnimeListPageListView.SmoothScrollToPosition(_prevArgs.SelectedItemIndex);
                        _prevArgs = null;
                    }
                    AnimeListPageGridView.Adapter = null;
                    AnimeListPageCompactListView.Adapter = null;
                }
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.AnimeList.AnimeCompactItems))
            {
                if (ViewModelLocator.AnimeList.AnimeCompactItems != null)
                {
                    //AnimeListPageCompactListView.Adapter = new AnimeListItemsAdapter(Context as Activity,
                    //    Resource.Layout.AnimeGridItem, ViewModelLocator.AnimeList.AnimeCompactItems);

                    AnimeListPageListView.Adapter = null;
                    AnimeListPageGridView.Adapter = null;
                }
            }
            else if(propertyChangedEventArgs.PropertyName == nameof(ViewModel.DisplayMode))
            {
                switch (ViewModel.DisplayMode)
                {
                    case AnimeListDisplayModes.IndefiniteList:
                        AnimeListPageListView.Visibility = ViewStates.Visible;

                        AnimeListPageGridView.Visibility = ViewStates.Gone;
                        AnimeListPageCompactListView.Visibility = ViewStates.Gone;
                        break;
                    case AnimeListDisplayModes.IndefiniteGrid:
                        AnimeListPageGridView.Visibility = ViewStates.Visible;

                        AnimeListPageListView.Visibility = ViewStates.Gone;
                        AnimeListPageCompactListView.Visibility = ViewStates.Gone;
                        break;
                    case AnimeListDisplayModes.IndefiniteCompactList:
                        AnimeListPageCompactListView.Visibility = ViewStates.Visible;

                        AnimeListPageListView.Visibility = ViewStates.Gone;
                        AnimeListPageGridView.Visibility = ViewStates.Gone;
                        break;
                }
            }
        }

        private GridView _animeListPageGridView;
        private ListView _animeListPageListView;
        private ListView _animeListPageCompactListView;
        private ProgressBar _animeListPageLoadingSpinner;
        private ImageButton _animeListPageReloadButton;
        private ImageButton _animeListPageDisplayMenu;
        private ImageButton _animeListPageFilterMenu;
        private ImageButton _animeListPageSortMenu;

        public GridView AnimeListPageGridView => _animeListPageGridView ?? (_animeListPageGridView = FindViewById<GridView>(Resource.Id.AnimeListPageGridView));

        public ListView AnimeListPageListView => _animeListPageListView ?? (_animeListPageListView = FindViewById<ListView>(Resource.Id.AnimeListPageListView));

        public ListView AnimeListPageCompactListView => _animeListPageCompactListView ?? (_animeListPageCompactListView = FindViewById<ListView>(Resource.Id.AnimeListPageCompactListView));

        public ProgressBar AnimeListPageLoadingSpinner => _animeListPageLoadingSpinner ?? (_animeListPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeListPageLoadingSpinner));

        public ImageButton AnimeListPageReloadButton => _animeListPageReloadButton ?? (_animeListPageReloadButton = FindViewById<ImageButton>(Resource.Id.AnimeListPageReloadButton));

        public ImageButton AnimeListPageDisplayMenu => _animeListPageDisplayMenu ?? (_animeListPageDisplayMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageDisplayMenu));

        public ImageButton AnimeListPageFilterMenu => _animeListPageFilterMenu ?? (_animeListPageFilterMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageFilterMenu));

        public ImageButton AnimeListPageSortMenu => _animeListPageSortMenu ?? (_animeListPageSortMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageSortMenu));









    }
}