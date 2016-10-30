using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.ViewModels.Main
{
    //this thing begs for refactor

    public class SearchPageViewModel : ViewModelBase
    {
        private bool _animeSearch; // default to anime
        private bool _queryHandler;
        private readonly HashSet<string> _filters = new HashSet<string>();
        private string _currrentFilter;
        private bool _isFirstVisitGridVisible = true;
        private bool _directQueryInputVisibility;
        public SearchPageNavigationArgs PrevArgs;
        public string PrevQuery;

        public void Init(SearchPageNavigationArgs args)
        {
            PrevArgs = args;
            if (args.ByGenre || args.ByStudio)
            {
                PrevQuery = null;
                EmptyNoticeVisibility = false;
                IsFirstVisitGridVisible = false;
                GenreSelectionGridVisibility = true;
                DirectQueryInputVisibility = false;

                if (args.ByGenre)
                {
                    AvailableSelectionChoices = Enum.GetValues(typeof(AnimeGenres)).Cast<Enum>().OrderBy(val => val.ToString()).ToList();
                }
                else
                {
                    AvailableSelectionChoices = Enum.GetValues(typeof(AnimeStudios)).Cast<Enum>().OrderBy(val => val.ToString()).ToList();
                }

                return;
            }
            else
            {
                GenreSelectionGridVisibility = false;
            }

            if (_animeSearch != args.Anime)
                PrevQuery = null;
            if(!_queryHandler)
                ViewModelLocator.GeneralMain.OnSearchQuerySubmitted += SubmitQuery;
            _queryHandler = true;
            _currrentFilter = null;
            _animeSearch = args.Anime;
            EmptyNoticeVisibility = false;
            IsFirstVisitGridVisible = true;
            if (args.DisplayMode == SearchPageDisplayModes.Off)
            {
                ViewModelLocator.NavMgr.ResetOffBackNav();
                DirectQueryInputVisibility = true;
            }
            else
            {
                DirectQueryInputVisibility = false;
            }

            if (!string.IsNullOrWhiteSpace(args.Query))
            {
                ViewModelLocator.GeneralMain.PopulateSearchFilters(_filters);
                SubmitQuery(args.Query);
            }
            else
            {
                _filters.Clear();
                AnimeSearchItemViewModels.Clear();
                IsFirstVisitGridVisible = true;
                ResetQuery();
            }
        }

        public void OnNavigatedFrom()
        {
            ViewModelLocator.GeneralMain.OnSearchQuerySubmitted -= SubmitQuery;
            _queryHandler = false;
        }

        public async void SubmitQuery(string query)
        {
            if (string.IsNullOrEmpty(query) || query == PrevQuery || query.Length < 2)
            {
                IsFirstVisitGridVisible = false;
                EmptyNoticeVisibility = false;
                return;
            }
            IsFirstVisitGridVisible = false;
            PrevQuery = query;
            Loading = true;
            EmptyNoticeVisibility = false;
            AnimeSearchItemViewModels.Clear();
            var data = new List<AnimeGeneralDetailsData>();
            _filters.Clear();
            _allAnimeSearchItemViewModels = new List<AnimeSearchItemViewModel>();
            if (_animeSearch)
            {
                await
                    Task.Run(
                        async () =>
                            data = await new AnimeSearchQuery(Utilities.CleanAnimeTitle(query)).GetSearchResults());
                try
                {
                    foreach (var item in data)
                    {
                        var type = item.Type;
                        _allAnimeSearchItemViewModels.Add(new AnimeSearchItemViewModel(item));
                        if (!_filters.Contains(type))
                            _filters.Add(type);
                    }
                }
                catch (Exception) //if MAL returns nothing it returns unparsable xml ... 
                {
                    //will display empty notice
                }
            }
            else // manga search
            {
                var response = "";
                await
                    Task.Run(
                        async () =>
                            response = await new MangaSearchQuery(Utilities.CleanAnimeTitle(query)).GetRequestResponse());
                try
                {
                    response = WebUtility.HtmlDecode(response);
                    var parsedData = XDocument.Parse(response.Replace("&", "")); //due to unparasable stuff returned by mal);
                    foreach (var item in parsedData.Element("manga").Elements("entry"))
                    {
                        var type = item.Element("type").Value;
                        var mangaData = new AnimeGeneralDetailsData();
                        mangaData.ParseXElement(item, false);
                        _allAnimeSearchItemViewModels.Add(new AnimeSearchItemViewModel(mangaData, false));
                        if (!_filters.Contains(type))
                            _filters.Add(type);
                    }
                }
                catch (Exception) //if MAL returns nothing it returns unparsable xml ... 
                {
                    //will display empty notice
                }
            }
            ViewModelLocator.GeneralMain.PopulateSearchFilters(_filters);
            PopulateItems();
            Loading = false;
        }

        private void PopulateItems()
        {
            AnimeSearchItemViewModels.Clear();
            foreach (
                var item in
                    _allAnimeSearchItemViewModels.Where(
                        item =>
                            string.IsNullOrWhiteSpace(_currrentFilter) ||
                            string.Equals(_currrentFilter, item.Type, StringComparison.CurrentCultureIgnoreCase)))
                AnimeSearchItemViewModels.Add(item);
            EmptyNoticeVisibility = AnimeSearchItemViewModels.Count == 0;
        }

        private void ResetQuery()
        {
            PrevQuery = null;
        }

        public void SubmitFilter(string filter)
        {
            _currrentFilter = filter == "None" ? "" : filter;
            PopulateItems();
        }

        #region Properties

        private List<AnimeSearchItemViewModel> _allAnimeSearchItemViewModels;

        public ObservableCollection<AnimeSearchItemViewModel> AnimeSearchItemViewModels { get; } =
            new ObservableCollection<AnimeSearchItemViewModel>();

        public AnimeSearchItemViewModel CurrentlySelectedItem
        {
            get { return null; } //One way to VM
            set
            {
                value?.NavigateDetails();
            }
        }

        private bool _loading = false;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private bool _emptyNoticeVisibility = false;
        private bool _genreSelectionGridVisibility;
        private List<Enum> _availableSelectionChoices;

        public bool EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyNoticeVisibility);
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

        public bool DirectQueryInputVisibility
        {
            get { return _directQueryInputVisibility; }
            set
            {
                _directQueryInputVisibility = value; 
                RaisePropertyChanged(() => DirectQueryInputVisibility);
            }
        }

        public bool GenreSelectionGridVisibility
        {
            get { return _genreSelectionGridVisibility; }
            set
            {
                _genreSelectionGridVisibility = value;
                RaisePropertyChanged(() => GenreSelectionGridVisibility);
            }
        }

        public List<Enum> AvailableSelectionChoices
        {
            get { return _availableSelectionChoices; }
            set
            {
                _availableSelectionChoices = value;
                RaisePropertyChanged(() => AvailableSelectionChoices);
            }
        }

        #endregion
    }
}