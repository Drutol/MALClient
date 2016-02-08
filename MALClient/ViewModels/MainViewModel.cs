using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;
using MALClient.UserControls;

namespace MALClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Dictionary<string, AnimeUserCache> _allAnimeItemsCache = new Dictionary<string, AnimeUserCache>();
        private List<RecomendationData> _recomendationDataCache = new List<RecomendationData>();
        private List<AnimeItemAbstraction> _seasonalAnimeCache = new List<AnimeItemAbstraction>();
        private Tuple<DateTime, ProfileData> _profileDataCache;
        private bool? _searchStateBeforeNavigatingToSearch;        
        private bool _wasOnDetailsFromSearch;
        private bool _onSearchPage;

        private bool _menuPaneState;

        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            private set
            {
                _menuPaneState = value;
                RaisePropertyChanged(() => MenuPaneState);
                if(value)
                    new ViewModelLocator().Hamburger.PaneOpened();
            }
        }

        public MainViewModel()
        { 

        }

        internal async Task Navigate(PageIndex index, object args = null)
        {
            var wasOnSearchPage = _onSearchPage;
            _onSearchPage = false;
            MenuPaneState = false;

            if (!Creditentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }

            var vl = new ViewModelLocator();
            vl.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList);

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggle.IsChecked = _searchStateBeforeNavigatingToSearch;
                if (SearchToggle.IsChecked.Value)
                    ShowSearchStuff();
                else
                {
                    HideSearchStuff();
                }
            }
            switch (index)
            {
                case PageIndex.PageAnimeList:
                    ShowSearchStuff();
                    if (wasOnSearchPage || _wasOnDetailsFromSearch)
                    {
                        _currSearchQuery = "";
                        SearchInput.Text = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    await Task.Run(async () =>
                    {
                        await
                            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                                () => { MainContent.Navigate(typeof(AnimeListPage), args); });
                    });
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).AnimeElement != null;
                    //from search , details are passed instead of being downloaded once more
                    MainContent.Navigate(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    MainContent.Navigate(typeof(SettingsPage));
                    break;
                case PageIndex.PageSearch:
                    NavigateSearch(args != null);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    MainContent.Navigate(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    await
                        Task.Run(
                            async () =>
                            {
                                await
                                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                        CoreDispatcherPriority.High,
                                        () => { MainContent.Navigate(typeof(ProfilePage), RetrieveProfileData()); });
                            });
                    break;
                case PageIndex.PageAbout:
                    HideSearchStuff();
                    SetStatus("About");
                    MainContent.Navigate(typeof(AboutPage));
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    SetStatus("Recommendations");
                    MainContent.Navigate(typeof(RecomendationsPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }
}
