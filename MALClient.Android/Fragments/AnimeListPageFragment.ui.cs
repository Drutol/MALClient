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
using GalaSoft.MvvmLight.Helpers;
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
        protected override void InitBindings()
        {
            ViewModel.PropertyChanged += AnimeListOnPropertyChanged;


            FilterFlyoutMenu.Layout = new FlyoutMenuView.GridLayout(1, FlyoutMenuView.GridLayout.Unspecified);
            FilterFlyoutMenu.Adapter =
                new FlyoutMenuView.ArrayAdapter(
                    Enum.GetValues(typeof(AnimeStatus))
                        .Cast<AnimeStatus>()
                        .Select(status => new AnimeListFilterFlyoutItem(status))
                        .ToList());

            FilterFlyoutMenu.SelectionListener = new MenuFlyoutSelectionListener(OnFilterMenuSelectionChanged);
            FilterFlyoutMenu.SetSelectedMenuItemById(ViewModel.CurrentStatus);

            SortFlyoutMenu.Layout = new FlyoutMenuView.GridLayout(1, FlyoutMenuView.GridLayout.Unspecified);
            SortFlyoutMenu.Adapter =
                new FlyoutMenuView.ArrayAdapter(
                    Enum.GetValues(typeof(SortOptions))
                        .Cast<SortOptions>()
                        .Select(option => new AnimeListSortFlyoutItem(option))
                        .ToList());

            SortFlyoutMenu.SelectionListener = new MenuFlyoutSelectionListener(OnSortingMenuSelectionChanged);
            SortFlyoutMenu.SetSelectedMenuItemById((int)ViewModel.SortOption);

            //ViewModel.SortingSettingChanged += ViewModelOnSortingSettingChanged;
        }

        private void ViewModelOnSortingSettingChanged(SortOptions option, bool descencing)
        {
            SortFlyoutMenu.SetSelectedMenuItemById((int)option);
        }

        private void OnSortingMenuSelectionChanged(FlyoutMenuView.MenuItem menuItem)
        {
            var item = menuItem as AnimeListSortFlyoutItem;
            ViewModel.SetSortOrder(item.SortOption);
            ViewModel.RefreshList();
        }

        private void OnFilterMenuSelectionChanged(FlyoutMenuView.MenuItem menuItem)
        {
            var item = menuItem as AnimeListFilterFlyoutItem;
            ViewModel.CurrentStatus = item.Status;
            ViewModel.RefreshList();
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

        public GridView AnimeListPageGridView => _animeListPageGridView ?? (_animeListPageGridView = FindViewById<GridView>(Resource.Id.AnimeListPageGridView));

        private FlyoutMenuView _filterFlyoutMenu;

        public FlyoutMenuView FilterFlyoutMenu => _filterFlyoutMenu ?? (_filterFlyoutMenu = FindViewById<FlyoutMenuView>(Resource.Id.AnimeListPageFilterMenu));

        private FlyoutMenuView _sortFlyoutMenu;

        public FlyoutMenuView SortFlyoutMenu => _sortFlyoutMenu ?? (_sortFlyoutMenu = FindViewById<FlyoutMenuView>(Resource.Id.AnimeListPageSortMenu));

    }
}