using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Search;
using MALClient.XShared.Comm.Search;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public class SearchEverywhereViewModel : ViewModelBase
    {
        private bool _queryHandler;
        private bool _loading;
        private bool _isEmptyNoticeVisible;
        private ICommand _navigateCharacterDetailsCommand;
        private bool _isFirstVisitGridVisible = true;
        private string _prevQuery;

        public ObservableCollection<ISearchEverywhereItem> SearchResults { get; } =
            new ObservableCollection<ISearchEverywhereItem>();

        public bool Loading
        {
            get { return _loading; }
            private set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public bool IsEmptyNoticeVisible
        {
            get { return _isEmptyNoticeVisible; }
            private set
            {
                _isEmptyNoticeVisible = value;
                RaisePropertyChanged(() => IsEmptyNoticeVisible);
            }
        }

        public bool IsFirstVisitGridVisible
        {
            get { return _isFirstVisitGridVisible; }
            private set
            {
                _isFirstVisitGridVisible = value;
                RaisePropertyChanged(() => IsFirstVisitGridVisible);
            }
        }

        public void Init(SearchPageNavArgsBase args)
        {
            if (!Loading && !SearchResults.Any())
            {
                IsFirstVisitGridVisible = true;
                IsEmptyNoticeVisible = false;
            }
            else
            {
                IsEmptyNoticeVisible = false;
                IsFirstVisitGridVisible = false;
            }

            if (args is SearchPageNavigationArgs a)
            {
                OnOnSearchQuerySubmitted(a.Query);
            }

            if (!_queryHandler)
                ViewModelLocator.GeneralMain.OnSearchDelayedQuerySubmitted += OnOnSearchQuerySubmitted;
            _queryHandler = true;

            OnOnSearchQuerySubmitted(ViewModelLocator.GeneralMain.CurrentSearchQuery);
        }

        private async void OnOnSearchQuerySubmitted(string query)
        {
            if (Loading || (query?.Equals(_prevQuery, StringComparison.CurrentCultureIgnoreCase) ?? true) ||
                string.IsNullOrEmpty(query))
                return;
            IsFirstVisitGridVisible = false;
            if (query.Length <= 2)
            {
                SearchResults.Clear();
                IsEmptyNoticeVisible = false;
                IsFirstVisitGridVisible = true;
                return;
            }

            _prevQuery = query;

            SearchResults.Clear();
            Loading = true;

            try
            {
                var searchResultsQuery = await new EverywhereSearchQuery().GetResult(query);

                foreach (var category in searchResultsQuery.Categories)
                {
                    SearchResults.Add(new SearchCategoryItem
                    {
                        Name = char.ToUpper(category.Type[0]) + category.Type.Substring(1)
                    });
                    foreach (var item in category.Items.OrderByDescending(item => item.EsScore))
                    {
                        ISearchEverywhereItem listItem;
                        switch (category.Type)
                        {
                            case "anime":
                                listItem = new SearchEverywhereAnimeItem(item);
                                break;
                            case "manga":
                                listItem = new SearchEverywhereMangaItem(item);
                                break;
                            case "character":
                                listItem = new SearchEverywhereCharacterItem(item);
                                break;
                            case "person":
                                listItem = new SearchEverywherePersonItem(item);
                                break;
                            case "user":
                                listItem = new SearchEverywhereUserItem(item);
                                break;
                            default:
                                continue;
                        }

                        SearchResults.Add(listItem);
                    }
                }

                IsEmptyNoticeVisible = !SearchResults.Any();
            }
            catch (Exception)
            {
                IsEmptyNoticeVisible = true;
            }

            Loading = false;
        }

        public void OnNavigatedFrom()
        {
            if (!ViewModelLocator.Mobile)
            {
                SearchResults?.Clear();
                _prevQuery = "";
            }

            ViewModelLocator.GeneralMain.OnSearchQuerySubmitted -= OnOnSearchQuerySubmitted;
            _queryHandler = false;
        }

        public void NavigateAnimeDetails(SearchEverywhereAnimeItem item)
        {
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(item.Item.Id, item.Item.Name, null, null,
                        new SearchPageNavigationArgs
                        {
                            Query = ViewModelLocator.SearchPage.PrevQuery,
                            Anime = false,
                            DisplayMode = ViewModelLocator.SearchPage.PrevArgs.DisplayMode,
                            Everywhere = true
                        })
                    {
                        Source = PageIndex.PageSearchEverywhere,
                        AnimeMode = true
                    });
        }

        public void NavigateMangaDetails(SearchEverywhereMangaItem item)
        {
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(item.Item.Id, item.Item.Name, null, null,
                        new SearchPageNavigationArgs
                        {
                            Query = ViewModelLocator.SearchPage.PrevQuery,
                            Anime = false,
                            DisplayMode = ViewModelLocator.SearchPage.PrevArgs.DisplayMode,
                            Everywhere = true
                        })
                    {
                        Source = PageIndex.PageSearchEverywhere,
                        AnimeMode = true
                    });
        }

        public void NavigateCharacterDetails(SearchEverywhereCharacterItem item)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageSearchEverywhere,
                new SearchPageNavigationArgs {Everywhere = true, Query = _prevQuery });
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,
                new CharacterDetailsNavigationArgs {Id = item.Item.Id});
        }

        public void NavigateUserDetails(SearchEverywhereUserItem item)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageSearchEverywhere,
                new SearchPageNavigationArgs { Everywhere = true, Query = _prevQuery});
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                new ProfilePageNavigationArgs
                {
                    AllowBackNavReset = false,
                    TargetUser = item.Item.Name
                });
        }

        public void NavigatePersonDetails(SearchEverywherePersonItem item)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageSearchEverywhere,
                new SearchPageNavigationArgs {Everywhere = true, Query = _prevQuery });
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageStaffDetails,
                new StaffDetailsNaviagtionArgs {Id = item.Item.Id});
        }
    }
}
