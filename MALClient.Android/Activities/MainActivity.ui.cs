using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
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
using GalaSoft.MvvmLight.Ioc;
using Java.Lang;
using MALClient.Adapters;
using MALClient.Android.BindingConverters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Debug = System.Diagnostics.Debug;
using Object = Java.Lang.Object;
using Orientation = Android.Widget.Orientation;
using SimpleCursorAdapter = Android.Support.V4.Widget.SimpleCursorAdapter;
using SearchView = Android.Support.V7.Widget.SearchView;
using Settings = MALClient.XShared.Utils.Settings;
using Uri = Android.Net.Uri;

namespace MALClient.Android.Activities
{
    [IntentFilter(new[] { "android.intent.action.VIEW" },
        Categories = new[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" },
        DataSchemes = new[] { "http", "https" },
        DataHosts = new[] { "www.myanimelist.net", "myanimelist.net" },
        DataPathPatterns = new[]
        {
            "/forum/.*",
            "/news",
            "/featured",
            "/mymessages.php",
            "/forum",
            "/forum/",
            "/anime.php",
            "/anime/.*",
            "/manga/.*",
            "/profile/.*",
            "/character/.*",
            "/people/.*",
        }
    )]
    public partial class MainActivity
    {
        private DroppyMenuPopup _upperFilterMenu;
        private SimpleCursorAdapter _searchSuggestionAdapter;
        private View _searchFrame;
        private Drawer _drawer;
        private readonly List<Binding> Bindings = new List<Binding>();

        private void InitBindings()
        {

            Bindings.Add(
                this.SetBinding(() => ViewModel.CurrentStatus,
                    () => MainPageCurrentStatus.Text));

            Bindings.Add(
                this.SetBinding(() => ViewModel.RefreshButtonVisibility,
                    () => MainPageRefreshButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
            MainPageRefreshButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModel.RefreshDataCommand.Execute(null);
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.SearchToggleVisibility,
                    () => MainPageSearchView.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
            Bindings.Add(
                this.SetBinding(() => ViewModel.SearchInputVisibility).WhenSourceChanges(() =>
                {
                    MainPageSearchView.Iconified = !ViewModel.SearchInputVisibility;
                    if(ViewModel.SearchInputVisibility)
                        MainPageSearchView.SetQuery(ViewModel.CurrentSearchQuery,false);
                   
                    MainPageSearchView.ClearFocus();
                }));

            Bindings.Add(this.SetBinding(() => ViewModel.CurrentMainPage).WhenSourceChanges(() =>
            {
                if (ViewModel.CurrentMainPage == PageIndex.PageAnimeList ||
                    ViewModel.CurrentMainPage == PageIndex.PageMangaList)
                    MainPageSearchView.QueryHint = "Search in current list";
                else if (ViewModel.CurrentMainPage == PageIndex.PageSearch)
                    MainPageSearchView.QueryHint = "Search in whole database";
                else
                    MainPageSearchView.QueryHint = string.Empty;
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.CurrentStatusSub).WhenSourceChanges(() =>
            {
                MainPageCurrentSatusSubtitle.Text = ViewModel.CurrentStatusSub;
                if (string.IsNullOrEmpty(ViewModel.CurrentStatusSub))
                {
                    MainPageCurrentSatusSubtitle.Visibility = ViewStates.Gone;
                    MainPageCurrentStatus.SetMaxLines(2);
                }
                else
                {
                    MainPageCurrentSatusSubtitle.Visibility = ViewStates.Visible;
                    MainPageCurrentStatus.SetMaxLines(1);
                }

            }));

            _searchFrame = MainPageSearchView.FindViewById(Resource.Id.search_edit_frame);

            Bindings.Add(this.SetBinding(() => ViewModel.SearchToggleLock).WhenSourceChanges(
                () =>
                {
                    if (ViewModel.SearchToggleLock)
                    {
                        MainPageSearchView.FindViewById(Resource.Id.search_close_btn).Alpha = 0;
                        MainPageSearchView.FindViewById(Resource.Id.search_close_btn).Clickable = false;
                        if (ViewModel.CurrentMainPage == PageIndex.PageSearch)
                        {
                            InputMethodManager imm = (InputMethodManager) GetSystemService(Context.InputMethodService);
                            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.NotAlways);
                        }

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
            var prevVisibility = _searchFrame.Visibility;
            observer.GlobalLayout += (sender, args) =>
            {
                if(prevVisibility == _searchFrame.Visibility)
                    return;
                prevVisibility = _searchFrame.Visibility;
                MainPageCurrentStatus.Visibility = Converters.VisibilityInverterConverter(_searchFrame.Visibility);
                var param = MainPageSearchView.LayoutParameters as LinearLayout.LayoutParams;
                Debug.WriteLine(_searchFrame.Visibility);
                if (_searchFrame.Visibility == ViewStates.Visible)
                {
                    var diff = ViewModel.SearchToggleStatus != true;
                    ViewModel.SearchToggleStatus = true;
                    param.Width = ViewGroup.LayoutParams.MatchParent;
                    param.SetMargins(0,0,DimensionsHelper.DpToPx(20),0);
                    param.Weight = 1;
                    if (diff)
                    {
                        MainPageSearchView.RequestFocusFromTouch();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.None);
                    }

                }
                else
                {
                    var diff = ViewModel.SearchToggleStatus != false;
                    ViewModel.SearchToggleStatus = false;
                    param.Width = (int)DimensionsHelper.DpToPx(50);
                    param.SetMargins(0,0,0,0);
                    param.Weight = 0;
                    if (diff)
                    {
                        InputMethodManager imm = (InputMethodManager) GetSystemService(Context.InputMethodService);
                        imm.HideSoftInputFromWindow(MainPageSearchView.WindowToken, HideSoftInputFlags.None);
                    }
                }
            };
                    
            _searchSuggestionAdapter = new SimpleCursorAdapter(this, Resource.Layout.SuggestionItem,
                null, new string[] {"hint"}, new int[]
                {
                    Resource.Id.SuggestionItemTextView
                });

            //
            MainPageStatusContainer.SetOnClickListener(new OnClickListener(view =>
            {
                if (ViewModel.CurrentMainPage == PageIndex.PageAnimeList)
                {
                    if (ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.SeasonalAnime)
                    {
                        _upperFilterMenu = FlyoutMenuBuilder.BuildGenericFlyout(this, MainPageCurrentStatus,
                            ViewModelLocator.AnimeList.SeasonSelection.Select(season => season.Name).ToList(),
                            OnUpperStatusSeasonSelected);
                    }
                    else
                    {
                        _upperFilterMenu = AnimeListPageFlyoutBuilder.BuildForAnimeStatusSelection(this, MainPageCurrentStatus,
                            OnUpperFlyoutStatusChanged, (AnimeStatus)ViewModelLocator.AnimeList.CurrentStatus,
                            ViewModelLocator.AnimeList.IsMangaWorkMode);
                    }

                    _upperFilterMenu.Show();
                }
            }));


            Bindings.Add(this.SetBinding(() => ViewModel.MediaElementVisibility)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.MediaElementVisibility)
                    {
                        MainPageVideoViewContainer.Visibility = ViewStates.Visible;
                        MainPageVideoView.Visibility = ViewStates.Visible;
                        MainUpperNavBar.Visibility = ViewStates.Gone;
                        MainPageVideoView.SetZOrderOnTop(true);
                        _drawer?.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                    }
                    else
                    {
                        MainPageVideoViewContainer.Visibility = ViewStates.Gone;
                        MainPageVideoView.Visibility = ViewStates.Gone;
                        MainUpperNavBar.Visibility = ViewStates.Visible;
                        MainPageVideoView.SetZOrderOnTop(false);
                        _drawer?.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                        ViewModelLocator.NavMgr.ResetOneTimeOverride();
                    }
                }));



            Bindings.Add(
                this.SetBinding(() => ViewModel.MediaElementSource).WhenSourceChanges(() =>
                {
                    if (string.IsNullOrEmpty(ViewModel.MediaElementSource))
                        return;

                    var mediaController = new MediaController(this);          
                    mediaController.SetAnchorView(MainPageVideoView);
                    MainPageVideoView.SetMediaController(mediaController);
                    MainPageVideoView.SetVideoURI(Uri.Parse(ViewModel.MediaElementSource));
                    MainPageVideoView.RequestFocus();
                }));

            AutoCompleteTextView searchAutoCompleteTextView = (AutoCompleteTextView)MainPageSearchView.FindViewById(Resource.Id.search_src_text);
            searchAutoCompleteTextView.Threshold = 1;

            MainPageSearchView.SuggestionsAdapter = _searchSuggestionAdapter;
            MainPageSearchView.QueryTextChange += MainPageSearchViewOnQueryTextChange;
            MainPageSearchView.QueryTextSubmit += MainPageSearchViewOnQueryTextSubmit;
            MainPageSearchView.SuggestionClick += MainPageSearchViewOnSuggestionClick;
            MainPageCloseVideoButton.Click += MainPageCloseVideoButtonOnClick;
            MainPageCopyVideoLinkButton.Click += MainPageCopyVideoLinkButtonOnClick;
            MainPageVideoView.Prepared += MainPageVideoViewOnPrepared;
            MainPageSearchView.Visibility = ViewStates.Visible;
            ((EditText)MainPageSearchView.FindViewById(Resource.Id.search_src_text)).SetTextColor(Color.White);


            MainPageHamburgerButton.Click +=  MainPageHamburgerButtonOnClick;
            ShareFloatingActionButton.SetOnClickListener(new OnClickListener(view =>
            {
                SimpleIoc.Default.GetInstance<IShareProvider>()
                    .Share(ResourceLocator.ShareManager.GenerateMessage());
            }));
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            BuildDrawer();     
            _drawer.OnDrawerItemClickListener = new HamburgerItemClickListener(OnHamburgerItemClick);

            MainPageCloseVideoButton.SetZ(0);
            MainPageCopyVideoLinkButton.SetZ(0);
            ShareFloatingActionButton.Hide();
        }

        private void ShareManagerOnTimerStateChanged(object sender, bool e)
        {
            if (e)
            {
                ShareFloatingActionButton.Show();
            }
            else
            {
                ShareFloatingActionButton.Hide();
            }
        }

        private void MainPageCopyVideoLinkButtonOnClick(object o, EventArgs eventArgs)
        {
            ViewModel.CopyMediaElementUrlCommand.Execute(null);
        }

        private void MainPageCloseVideoButtonOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.MediaElementSource = null;
            ViewModel.MediaElementVisibility = false;
        }

        private void MainPageVideoViewOnPrepared(object sender, EventArgs eventArgs)
        {
            MainPageVideoView.Start();
        }

        private void OnUpperFlyoutStatusChanged(AnimeStatus animeStatus)
        {
            if(_upperFilterMenu == null)
                return;
            ViewModelLocator.AnimeList.CurrentStatus = (int)animeStatus;
            ViewModelLocator.AnimeList.RefreshList();
            _upperFilterMenu.Dismiss(true);
            _upperFilterMenu = null;
        }

        private void OnUpperStatusSeasonSelected(int i1)
        {
            if (_upperFilterMenu == null)
                return;
            ViewModelLocator.AnimeList.SeasonalUrlsSelectedIndex = i1;
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
            if(!_allowHamburgerNavigation)
                return;

            OnHamburgerItemClick((PageIndex)arg3.Identifier);
            _drawer.SetSelection(arg3, false);
        }

        private void OnHamburgerItemClick(PageIndex page)
        {
            if(!_allowHamburgerNavigation)
                return;

            SetActiveButton(Utilities.GetButtonForPage(page));
            ViewModelLocator.GeneralMain.Navigate(page, GetAppropriateArgsForPage(page));
            _drawer.CloseDrawer();
        }



        private void MainPageHamburgerButtonOnClick(object sender, EventArgs eventArgs)
        {
            HamburgerOpened?.Invoke(this,EventArgs.Empty);
            _drawer.OpenDrawer();
        }

        private void SetRightTheme()
        {
            if (Settings.SelectedTheme == 1)
            {
                switch (AndroidColourThemeHelper.CurrentTheme)
                {
                    case AndroidColorThemes.Orange:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Orange);
                        break;
                    case AndroidColorThemes.Purple:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Purple);
                        break;
                    case AndroidColorThemes.Blue:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Blue);
                        break;
                    case AndroidColorThemes.Lime:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Lime);
                        break;
                    case AndroidColorThemes.Pink:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Pink);
                        break;
                    case AndroidColorThemes.Cyan:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Cyan);
                        break;
                    case AndroidColorThemes.SkyBlue:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_SkyBlue);
                        break;
                    case AndroidColorThemes.Red:
                        SetTheme(Resource.Style.Theme_MALClient_Dark_Red);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (Settings.DarkThemeAmoled)
                {
                    Theme.ApplyStyle(Resource.Style.BlackTheme, true);
                    IsAmoledApplied = true;
                }
                else
                {
                    IsAmoledApplied = false;
                }          
            }
            else
            {
                switch (AndroidColourThemeHelper.CurrentTheme)
                {
                    case AndroidColorThemes.Orange:
                        SetTheme(Resource.Style.Theme_MALClient_Light_Orange);
                        break;                                  
                    case AndroidColorThemes.Purple:             
                        SetTheme(Resource.Style.Theme_MALClient_Light_Purple);
                        break;
                    case AndroidColorThemes.Blue:
                        SetTheme(Resource.Style.Theme_MALClient_Light_Blue);
                        break;
                    case AndroidColorThemes.Lime:
                        SetTheme(Resource.Style.Theme_MALClient_Light_Lime);
                        break;
                    case AndroidColorThemes.Pink:
                        SetTheme(Resource.Style.Theme_MALClient_Light_Pink);
                        break;
                    case AndroidColorThemes.Cyan:
                        SetTheme(Resource.Style.Theme_MALClient_Light_Cyan);
                        break;
                    case AndroidColorThemes.SkyBlue:
                        SetTheme(Resource.Style.Theme_MALClient_Light_SkyBlue);
                        break;
                    case AndroidColorThemes.Red:
                        SetTheme(Resource.Style.Theme_MALClient_Light_Red);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }




        public override void OnConfigurationChanged(Configuration newConfig)
        {
            //if (newConfig.Orientation == global::Android.Content.Res.Orientation.Landscape)
            //{

            //}
            //else
            //{
            //    MainPageStatusContainer.Orientation = Orientation.Horizontal;

            //    MainPageCurrentStatus.SetMaxLines(2);
            //    UpdateCurrentStatusWidth();
            //    var margin = DimensionsHelper.DpToPx(5);


            //    var param = MainPageCurrentStatus.LayoutParameters as LinearLayout.LayoutParams;
            //    param.SetMargins(margin, margin, margin, margin);
            //    MainPageCurrentStatus.LayoutParameters = param;

            //    param = MainPageCurrentSatusSubtitle.LayoutParameters as LinearLayout.LayoutParams;
            //    param.SetMargins(margin, margin, margin, margin);
            //    MainPageCurrentSatusSubtitle.LayoutParameters = param;

            //    var cparam = MainPageStatusContainer.LayoutParameters;
            //    cparam.Height = -1;
            //    MainPageStatusContainer.LayoutParameters = cparam;

            //}
            
            base.OnConfigurationChanged(newConfig);
        }

        #region Views

        private ImageButton _mainPageHamburgerButton;
        private TextView _mainPageCurrentStatus;
        private TextView _mainPageCurrentSatusSubtitle;
        private LinearLayout _mainPageStatusContainer;
        private SearchView _mainPageSearchView;
        private ImageButton _mainPageRefreshButton;
        private LinearLayout _mainUpperNavBar;
        private FrameLayout _mainContentFrame;
        private AdView _mainPageAdView;
        private FloatingActionButton _shareFloatingActionButton;
        private VideoView _mainPageVideoView;
        private ImageButton _mainPageCopyVideoLinkButton;
        private ImageButton _mainPageCloseVideoButton;
        private RelativeLayout _mainPageVideoViewContainer;
        private LinearLayout _mainPageRoot;

        public ImageButton MainPageHamburgerButton => _mainPageHamburgerButton ?? (_mainPageHamburgerButton = FindViewById<ImageButton>(Resource.Id.MainPageHamburgerButton));
        public TextView MainPageCurrentStatus => _mainPageCurrentStatus ?? (_mainPageCurrentStatus = FindViewById<TextView>(Resource.Id.MainPageCurrentStatus));
        public TextView MainPageCurrentSatusSubtitle => _mainPageCurrentSatusSubtitle ?? (_mainPageCurrentSatusSubtitle = FindViewById<TextView>(Resource.Id.MainPageCurrentSatusSubtitle));
        public LinearLayout MainPageStatusContainer => _mainPageStatusContainer ?? (_mainPageStatusContainer = FindViewById<LinearLayout>(Resource.Id.MainPageStatusContainer));
        public SearchView MainPageSearchView => _mainPageSearchView ?? (_mainPageSearchView = FindViewById<SearchView>(Resource.Id.MainPageSearchView));
        public ImageButton MainPageRefreshButton => _mainPageRefreshButton ?? (_mainPageRefreshButton = FindViewById<ImageButton>(Resource.Id.MainPageRefreshButton));
        public LinearLayout MainUpperNavBar => _mainUpperNavBar ?? (_mainUpperNavBar = FindViewById<LinearLayout>(Resource.Id.MainUpperNavBar));
        public FrameLayout MainContentFrame => _mainContentFrame ?? (_mainContentFrame = FindViewById<FrameLayout>(Resource.Id.MainContentFrame));
        public AdView MainPageAdView => _mainPageAdView ?? (_mainPageAdView = FindViewById<AdView>(Resource.Id.MainPageAdView));
        public FloatingActionButton ShareFloatingActionButton => _shareFloatingActionButton ?? (_shareFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.ShareFloatingActionButton));
        public VideoView MainPageVideoView => _mainPageVideoView ?? (_mainPageVideoView = FindViewById<VideoView>(Resource.Id.MainPageVideoView));
        public ImageButton MainPageCopyVideoLinkButton => _mainPageCopyVideoLinkButton ?? (_mainPageCopyVideoLinkButton = FindViewById<ImageButton>(Resource.Id.MainPageCopyVideoLinkButton));
        public ImageButton MainPageCloseVideoButton => _mainPageCloseVideoButton ?? (_mainPageCloseVideoButton = FindViewById<ImageButton>(Resource.Id.MainPageCloseVideoButton));
        public RelativeLayout MainPageVideoViewContainer => _mainPageVideoViewContainer ?? (_mainPageVideoViewContainer = FindViewById<RelativeLayout>(Resource.Id.MainPageVideoViewContainer));
        public LinearLayout MainPageRoot => _mainPageRoot ?? (_mainPageRoot = FindViewById<LinearLayout>(Resource.Id.MainPageRoot));

        #endregion

    }
}