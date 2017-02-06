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
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
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
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;
using static MALClient.Android.Flyouts.AnimeListPageFlyoutBuilder;

namespace MALClient.Android.Fragments
{
    public partial class AnimeListPageFragment : MalFragmentBase
    {
        private DroppyMenuPopup _sortingMenu;
        private DroppyMenuPopup _filterMenu;
        private DroppyMenuPopup _displayMenu;
        private DroppyMenuPopup _seasonMenu;

        protected override void InitBindings()
        {
            var swipeRefresh = RootView as SwipeRefreshLayout;

            Bindings.Add(Resource.Id.AnimeListPageLoadingSpinner, new List<Binding>());
            Bindings[Resource.Id.AnimeListPageLoadingSpinner].Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => AnimeListPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(AnimeListPageSeasonMenu.Id, new List<Binding>());
            Bindings[AnimeListPageSeasonMenu.Id].Add(
                this.SetBinding(() => ViewModel.AppbarBtnPinTileVisibility,
                    () => AnimeListPageSeasonMenu.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(AnimeListPageSortMenu.Id, new List<Binding>());
            Bindings[AnimeListPageSortMenu.Id].Add(
                this.SetBinding(() => ViewModel.AppBtnSortingVisibility,
                    () => AnimeListPageSortMenu.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            ViewModel.PropertyChanged += AnimeListOnPropertyChanged;
           
            AnimeListPageFilterMenu.SetCommand("Click",new RelayCommand(ShowFilterMenu));
            AnimeListPageSortMenu.SetCommand("Click", new RelayCommand(ShowSortMenu));
            AnimeListPageDisplayMenu.SetCommand("Click", new RelayCommand(ShowDisplayMenu));
            AnimeListPageSeasonMenu.SetCommand("Click",new RelayCommand(ShowSeasonMenu));

            swipeRefresh.NestedScrollingEnabled = true;
            swipeRefresh.Refresh += (sender, args) =>
            {
                swipeRefresh.Refreshing = false;

                ViewModel.RefreshCommand.Execute(null);
            };
        }

        private void ShowSeasonMenu()
        {
            _seasonMenu = AnimeListPageFlyoutBuilder.BuildForAnimeSeasonSelection(Activity, AnimeListPageSeasonMenu,
                SelectSeason, ViewModel);
            _seasonMenu.Show();
            var spinnerYear = _seasonMenu.MenuView.FindViewById<Spinner>(Resource.Id.SeasonSelectionPopupYearComboBox);
            var spinnerSeason = _seasonMenu.MenuView.FindViewById<Spinner>(Resource.Id.SeasonSelectionPopupSeasonComboBox);
            spinnerYear.Adapter = ViewModel.SeasonYears.GetAdapter((i, s, arg3) =>
            {
                var view = arg3 ?? BuildBaseItem(Activity, s, ResourceExtension.BrushAnimeItemInnerBackground, null, false);
                view.Tag = s.Wrap();

                return view;
            });
            spinnerYear.ItemSelected += (sender, args) =>
            {
                ViewModel.CurrentlySelectedCustomSeasonYear = (sender as Spinner).SelectedView.Tag.Unwrap<string>();
            };
            spinnerSeason.Adapter = ViewModel.SeasonSeasons.GetAdapter((i, s, arg3) =>
            {
                var view = arg3 ?? BuildBaseItem(Activity, s,ResourceExtension.BrushAnimeItemInnerBackground,null,false);
                view.Tag = s.Wrap();

                return view;
            });
            spinnerSeason.ItemSelected += (sender, args) =>
            {
                ViewModel.CurrentlySelectedCustomSeasonSeason = (sender as Spinner).SelectedView.Tag.Unwrap<string>();
            };
            _seasonMenu.MenuView.FindViewById(Resource.Id.SeasonSelectionPopupAcceptButton).SetCommand("Click",ViewModel.GoToCustomSeasonCommand);

        }

        private void SelectSeason(int season)
        {
            ViewModel.SeasonalUrlsSelectedIndex = season;
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
                    _animeListItemsAdapter = new AnimeListItemsAdapter(Activity,
                        Resource.Layout.AnimeGridItem, ViewModelLocator.AnimeList.AnimeGridItems,
                        (model, view) =>
                            new AnimeGridItemBindingInfo(view, model)
                            {
                                OnItemClickAction = AnimeListPageGridViewOnItemClick
                            });
                    AnimeListPageGridView.Adapter = _animeListItemsAdapter;
                    _gridViewColumnHelper = new GridViewColumnHelper(AnimeListPageGridView);
                    if (_prevArgs != null)
                    {
                        var pos = _prevArgs.SelectedItemIndex;
                        await Task.Delay(300);
                        AnimeListPageGridView.RequestFocusFromTouch();
                        AnimeListPageGridView.SetSelection(pos);
                        AnimeListPageGridView.RequestFocus();
                        _prevArgs = null;
                    }

                    SwipeRefreshLayout.ScrollingView = AnimeListPageGridView;

                    AnimeListPageListView.Adapter = null;
                    AnimeListPageCompactListView.Adapter = null;
                }
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.AnimeList.AnimeListItems))
            {
                if (ViewModelLocator.AnimeList.AnimeListItems != null)
                {
                    _animeListItemsAdapter = new AnimeListItemsAdapter(Activity,
                        Resource.Layout.AnimeListItem, ViewModelLocator.AnimeList.AnimeListItems,(model, view) => new AnimeListItemBindingInfo(view,model)
                        {
                            OnItemClickAction = AnimeListPageGridViewOnItemClick
                        });
                    AnimeListPageListView.Adapter = _animeListItemsAdapter;
                    if (_prevArgs != null)
                    {
                        AnimeListPageListView.SmoothScrollToPosition(_prevArgs.SelectedItemIndex);
                        _prevArgs = null;
                    }

                    SwipeRefreshLayout.ScrollingView = AnimeListPageListView;

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

                    SwipeRefreshLayout.ScrollingView = AnimeListPageCompactListView;

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

        public ScrollableSwipeToRefreshLayout SwipeRefreshLayout => RootView as ScrollableSwipeToRefreshLayout;

        #region Views

        private GridView _animeListPageGridView;
        private ListView _animeListPageListView;
        private ListView _animeListPageCompactListView;
        private ImageButton _animeListPageDisplayMenu;
        private ImageButton _animeListPageSeasonMenu;
        private ImageButton _animeListPageFilterMenu;
        private ImageButton _animeListPageSortMenu;
        private RelativeLayout _animeListPageLoadingSpinner;

        public GridView AnimeListPageGridView => _animeListPageGridView ?? (_animeListPageGridView = FindViewById<GridView>(Resource.Id.AnimeListPageGridView));

        public ListView AnimeListPageListView => _animeListPageListView ?? (_animeListPageListView = FindViewById<ListView>(Resource.Id.AnimeListPageListView));

        public ListView AnimeListPageCompactListView => _animeListPageCompactListView ?? (_animeListPageCompactListView = FindViewById<ListView>(Resource.Id.AnimeListPageCompactListView));

        public ImageButton AnimeListPageDisplayMenu => _animeListPageDisplayMenu ?? (_animeListPageDisplayMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageDisplayMenu));

        public ImageButton AnimeListPageSeasonMenu => _animeListPageSeasonMenu ?? (_animeListPageSeasonMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageSeasonMenu));

        public ImageButton AnimeListPageFilterMenu => _animeListPageFilterMenu ?? (_animeListPageFilterMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageFilterMenu));

        public ImageButton AnimeListPageSortMenu => _animeListPageSortMenu ?? (_animeListPageSortMenu = FindViewById<ImageButton>(Resource.Id.AnimeListPageSortMenu));

        public RelativeLayout AnimeListPageLoadingSpinner => _animeListPageLoadingSpinner ?? (_animeListPageLoadingSpinner = FindViewById<RelativeLayout>(Resource.Id.AnimeListPageLoadingSpinner));


        #endregion





    }
}