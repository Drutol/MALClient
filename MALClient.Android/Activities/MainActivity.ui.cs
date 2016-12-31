using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Holder;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using Com.Shehabic.Droppy;
using FFImageLoading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using Java.Lang;
using MALClient.Android.BindingConverters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Object = Java.Lang.Object;
using SimpleCursorAdapter = Android.Support.V4.Widget.SimpleCursorAdapter;
using SearchView = Android.Support.V7.Widget.SearchView;

namespace MALClient.Android.Activities
{
    public partial class MainActivity
    {
        private DroppyMenuPopup _upperFilterMenu;
        private SimpleCursorAdapter _searchSuggestionAdapter;
        private View _searchFrame;
        private Drawer _drawer;
        private readonly Dictionary<int, List<Binding>> Bindings = new Dictionary<int, List<Binding>>();

        private void InitBindings()
        {
            Bindings.Add(MainPageCurrentStatus.Id, new List<Binding>());
            Bindings[MainPageCurrentStatus.Id].Add(
                this.SetBinding(() => ViewModel.CurrentStatus,
                    () => MainPageCurrentStatus.Text));

            Bindings.Add(MainPageRefreshButton.Id, new List<Binding>());
            Bindings[MainPageRefreshButton.Id].Add(
                this.SetBinding(() => ViewModel.RefreshButtonVisibility,
                    () => MainPageRefreshButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(MainPageSearchView.Id, new List<Binding>());
            Bindings[MainPageSearchView.Id].Add(
                this.SetBinding(() => ViewModel.SearchToggleVisibility,
                    () => MainPageSearchView.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
            Bindings[MainPageSearchView.Id].Add(
                this.SetBinding(() => ViewModel.SearchInputVisibility).WhenSourceChanges(() =>
                {
                    MainPageSearchView.Iconified = !ViewModel.SearchInputVisibility;
                    if(ViewModel.SearchInputVisibility)
                        MainPageSearchView.SetQuery(ViewModel.CurrentSearchQuery,false);
                    MainPageSearchView.ClearFocus();
                }));

            Bindings.Add(MainPageCurrentSatusSubtitle.Id, new List<Binding>());
            Bindings[MainPageCurrentSatusSubtitle.Id].Add(
                this.SetBinding(() => ViewModel.CurrentStatusSub,
                    () => MainPageCurrentSatusSubtitle.Text));

            _searchFrame = MainPageSearchView.FindViewById(Resource.Id.search_edit_frame);
            
            Bindings[MainPageSearchView.Id].Add(this.SetBinding(() => ViewModel.SearchToggleLock).WhenSourceChanges(
                () =>
                {
                    if (ViewModel.SearchToggleLock)
                    {
                        MainPageSearchView.FindViewById(Resource.Id.search_close_btn).Alpha = 0;
                        MainPageSearchView.FindViewById(Resource.Id.search_close_btn).Clickable = false;
                    }
                    else
                    {
                        MainPageSearchView.FindViewById(Resource.Id.search_close_btn).Alpha = 1;
                        MainPageSearchView.FindViewById(Resource.Id.search_close_btn).Clickable = true;
                    }
                }));

            //MainPageSearchView.LayoutChange += MainPageSearchViewOnLayoutChange;

            //var padding = (int)(11*Resources.DisplayMetrics.Density);
            //searchBtn.SetScaleType(ImageView.ScaleType.FitEnd);
            //searchBtn.SetPadding(padding, padding, padding, padding);
            var observer = _searchFrame.ViewTreeObserver;
            observer.GlobalLayout += (sender, args) =>
            {
                MainPageCurrentStatus.Visibility = Converters.VisibilityInverterConverter(_searchFrame.Visibility);
                var param = MainPageSearchView.LayoutParameters as LinearLayout.LayoutParams;
                if (_searchFrame.Visibility == ViewStates.Visible)
                {
                    ViewModel.SearchToggleStatus = true;
                    param.Width = ViewGroup.LayoutParams.WrapContent;
                    param.Weight = 1;
                }
                else
                {
                    ViewModel.SearchToggleStatus = false;
                    param.Width = (int)DimensionsHelper.DpToPx(50);
                    param.Weight = 0;
                }
            };
                    
            _searchSuggestionAdapter = new SimpleCursorAdapter(this, Resource.Layout.SuggestionItem,
                null, new string[] {"hint"}, new int[]
                {
                    Resource.Id.SuggestionItemTextView
                });

            //
            MainPageCurrentStatus.Clickable = true;
            MainPageCurrentStatus.SetOnClickListener(new OnClickListener(view =>
            {
                if (ViewModel.CurrentMainPage == PageIndex.PageAnimeList)
                {
                    _upperFilterMenu = AnimeListPageFlyoutBuilder.BuildForAnimeStatusSelection(this, MainPageCurrentStatus,
                        OnUpperFlyoutStatusChanged, (AnimeStatus) ViewModelLocator.AnimeList.CurrentStatus,
                        ViewModelLocator.AnimeList.IsMangaWorkMode);
                    _upperFilterMenu.Show();
                }
            }));

            MainPageSearchView.SuggestionsAdapter = _searchSuggestionAdapter;
            MainPageSearchView.QueryTextChange += MainPageSearchViewOnQueryTextChange;
            MainPageSearchView.QueryTextSubmit += MainPageSearchViewOnQueryTextSubmit;
            MainPageSearchView.SuggestionClick += MainPageSearchViewOnSuggestionClick;
            MainPageSearchView.QueryTextSubmit += MainPageSearchViewOnQueryTextSubmit;
            MainPageSearchView.Visibility = ViewStates.Visible;


            MainPageHamburgerButton.Click +=  MainPageHamburgerButtonOnClick;      
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            BuildDrawer();     
            _drawer.OnDrawerItemClickListener = new HamburgerItemClickListener(OnHamburgerItemClick); 
        }

        private void OnUpperFlyoutStatusChanged(AnimeStatus animeStatus)
        {
            ViewModelLocator.AnimeList.CurrentStatus = (int)animeStatus;
            ViewModelLocator.AnimeList.RefreshList();
            _upperFilterMenu.Dismiss(true);
            _upperFilterMenu = null;
        }

        private void MainPageSearchViewOnSuggestionClick(object sender, SearchView.SuggestionClickEventArgs suggestionClickEventArgs)
        {
            MainPageSearchView.SetQuery(ViewModel.CurrentHintSet[suggestionClickEventArgs.Position],true);
            MainPageSearchView.ClearFocus();
        }

        private void MainPageSearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs queryTextSubmitEventArgs)
        {
            ViewModel.OnSearchInputSubmit();
            MainPageSearchView.ClearFocus();
        }

        private void MainPageSearchViewOnQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs queryTextChangeEventArgs)
        {
            if(!ViewModel.SearchInputVisibility)
                return;
            ViewModel.CurrentSearchQuery = queryTextChangeEventArgs.NewText;
            queryTextChangeEventArgs.Handled = true;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                //case nameof(ViewModel.SearchToggleStatus):
                //    if(ViewModel.SearchToggleStatus)
                //        MainPageSearchView.SetBackgroundColor(new Color(ResourceExtension.AccentColour));
                //    else
                //        MainPageSearchView.SetBackgroundResource(ResourceExtension.SelectableItemBackground);
                //    break;
                case nameof(ViewModel.SearchToggleLock):

                    break;
                case nameof(ViewModel.CurrentHintSet):
                    UpdateSearchSuggestions();
                    break;
            }
        }

        private void UpdateSearchSuggestions()
        {
            var matrix = new MatrixCursor(new string[] {BaseColumns.Id,"hint"});
            int i = 0;
            foreach (var hint in ViewModel.CurrentHintSet.Where(s => s != MainPageSearchView.Query))
            {
                matrix.AddRow(new Object[] {i,hint});
            }
            _searchSuggestionAdapter.ChangeCursor(matrix);
        }

        private void OnHamburgerItemClick(View view, int i, IDrawerItem arg3)
        {
            //if (page == PageIndex.PageProfile)
               // ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            var page = (PageIndex) arg3.Identifier;
            ViewModelLocator.GeneralMain.Navigate(page, GetAppropriateArgsForPage(page));
            _drawer.SetSelection(arg3, false);
            _drawer.CloseDrawer();
        }



        private void MainPageHamburgerButtonOnClick(object sender, EventArgs eventArgs)
        {
            _drawer.OpenDrawer();
        }

        private ImageButton _mainPageHamburgerButton;
        private TextView _mainPageCurrentStatus;
        private TextView _mainPageCurrentSatusSubtitle;
        private SearchView _mainPageSearchView;
        private ImageButton _mainPageRefreshButton;
        private FrameLayout _mainContentFrame;

        public ImageButton MainPageHamburgerButton => _mainPageHamburgerButton ?? (_mainPageHamburgerButton = FindViewById<ImageButton>(Resource.Id.MainPageHamburgerButton));

        public TextView MainPageCurrentStatus => _mainPageCurrentStatus ?? (_mainPageCurrentStatus = FindViewById<TextView>(Resource.Id.MainPageCurrentStatus));

        public TextView MainPageCurrentSatusSubtitle => _mainPageCurrentSatusSubtitle ?? (_mainPageCurrentSatusSubtitle = FindViewById<TextView>(Resource.Id.MainPageCurrentSatusSubtitle));

        public SearchView MainPageSearchView => _mainPageSearchView ?? (_mainPageSearchView = FindViewById<SearchView>(Resource.Id.MainPageSearchView));

        public ImageButton MainPageRefreshButton => _mainPageRefreshButton ?? (_mainPageRefreshButton = FindViewById<ImageButton>(Resource.Id.MainPageRefreshButton));

        public FrameLayout MainContentFrame => _mainContentFrame ?? (_mainContentFrame = FindViewById<FrameLayout>(Resource.Id.MainContentFrame));


    }
}