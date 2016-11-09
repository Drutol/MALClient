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
        protected override void InitBindings()
        {
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


            Bindings = new Dictionary<int, List<Binding>>();

            Bindings.Add(Resource.Id.AnimeListPageLoadingSpinner, new List<Binding>());
            Bindings[Resource.Id.AnimeListPageLoadingSpinner].Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => AnimeListPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            ViewModel.PropertyChanged += AnimeListOnPropertyChanged;
            AnimeListPageReloadButton.SetCommand("Click",ViewModel.RefreshCommand);
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
        private FlyoutMenuView _animeListPageFilterMenu;
        private FlyoutMenuView _animeListPageSortMenu;
        public FlyoutMenuView FilterFlyoutMenu => _animeListPageFilterMenu ?? (_animeListPageFilterMenu = FindViewById<FlyoutMenuView>(Resource.Id.AnimeListPageFilterMenu));
        public FlyoutMenuView SortFlyoutMenu => _animeListPageSortMenu ?? (_animeListPageSortMenu = FindViewById<FlyoutMenuView>(Resource.Id.AnimeListPageSortMenu));


        private GridView _animeListPageGridView;
        private ProgressBar _animeListPageLoadingSpinner;
        private ImageButton _animeListPageReloadButton;

        public GridView AnimeListPageGridView => _animeListPageGridView ?? (_animeListPageGridView = FindViewById<GridView>(Resource.Id.AnimeListPageGridView));

        public ProgressBar AnimeListPageLoadingSpinner => _animeListPageLoadingSpinner ?? (_animeListPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeListPageLoadingSpinner));

        public ImageButton AnimeListPageReloadButton => _animeListPageReloadButton ?? (_animeListPageReloadButton = FindViewById<ImageButton>(Resource.Id.AnimeListPageReloadButton));

        



    }
}