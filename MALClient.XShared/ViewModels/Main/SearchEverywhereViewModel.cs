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

        public ObservableCollection<ISearchEverywhereItem> SearchResults { get; } = new ObservableCollection<ISearchEverywhereItem>();

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

        public ICommand NavigateCharacterDetailsCommand
            => _navigateCharacterDetailsCommand ?? (_navigateCharacterDetailsCommand = new RelayCommand<FavouriteViewModel>(
                        entry =>
                        {
                            if (ViewModelLocator.Mobile)
                                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageCharacterSearch, null);
                            else if (ViewModelLocator.GeneralMain.OffContentVisibility)
                            {
                                if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageStaffDetails)
                                    ViewModelLocator.StaffDetails.RegisterSelfBackNav(int.Parse(entry.Data.Id));
                                else if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageCharacterDetails)
                                    ViewModelLocator.CharacterDetails.RegisterSelfBackNav(int.Parse(entry.Data.Id));
                            }
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,
                                new CharacterDetailsNavigationArgs { Id = int.Parse(entry.Data.Id) });
                        }));

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

            if (!_queryHandler)
                ViewModelLocator.GeneralMain.OnSearchDelayedQuerySubmitted += OnOnSearchQuerySubmitted;
            _queryHandler = true;

            OnOnSearchQuerySubmitted(ViewModelLocator.GeneralMain.CurrentSearchQuery);
        }

        private async void OnOnSearchQuerySubmitted(string query)
        {
            if (Loading || (query?.Equals(_prevQuery, StringComparison.CurrentCultureIgnoreCase) ?? true) || string.IsNullOrEmpty(query))
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
                        Name = category.Type
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
    }
}
