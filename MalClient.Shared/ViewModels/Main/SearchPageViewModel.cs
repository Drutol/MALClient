using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Utils;

namespace MALClient.ViewModels
{
    public class SearchPageNavigationArgs
    {
        public bool Anime { get; set; } = true;
        public string Query { get; set; }
    }

    public class SearchPageViewModel : ViewModelBase
    {
        private bool _animeSearch; // default to anime

        public string PrevQuery;

        public void Init(SearchPageNavigationArgs args)
        {
            if (_animeSearch != args.Anime)
                PrevQuery = null;

            _currrentFilter = null;
            _animeSearch = args.Anime;
            if (!string.IsNullOrWhiteSpace(args.Query))
            {
                ViewModelLocator.GeneralMain.PopulateSearchFilters(_filters);
                SubmitQuery(args.Query);
            }
            else
            {
                _filters.Clear();
                AnimeSearchItems.Clear();
                ResetQuery();
            }
        }

        public async void SubmitQuery(string query)
        {
            if (query == PrevQuery)
                return;
            PrevQuery = query;
            Loading = Visibility.Visible;
            EmptyNoticeVisibility = Visibility.Collapsed;
            AnimeSearchItems.Clear();
            var data = new List<AnimeGeneralDetailsData>();
            _filters.Clear();
            _allAnimeSearchItems = new List<AnimeSearchItem>();
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
                        _allAnimeSearchItems.Add(new AnimeSearchItem(item));
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
                    var parsedData = XDocument.Parse(response);
                    foreach (var item in parsedData.Element("manga").Elements("entry"))
                    {
                        var type = item.Element("type").Value;
                        var mangaData = new AnimeGeneralDetailsData();
                        mangaData.ParseXElement(item, false);
                        _allAnimeSearchItems.Add(new AnimeSearchItem(mangaData, false));
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
            Loading = Visibility.Collapsed;
        }

        private void PopulateItems()
        {
            AnimeSearchItems.Clear();
            foreach (
                var item in
                    _allAnimeSearchItems.Where(
                        item =>
                            string.IsNullOrWhiteSpace(_currrentFilter) ||
                            string.Equals(_currrentFilter, item.Type, StringComparison.CurrentCultureIgnoreCase)))
                AnimeSearchItems.Add(item);
            EmptyNoticeVisibility = AnimeSearchItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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

        private List<AnimeSearchItem> _allAnimeSearchItems;

        public ObservableCollection<AnimeSearchItem> AnimeSearchItems { get; } =
            new ObservableCollection<AnimeSearchItem>();

        public AnimeSearchItem CurrentlySelectedItem
        {
            get { return null; } //One way to VM
            set { value?.NavigateDetails(); }
        }

        private Visibility _loading = Visibility.Collapsed;

        public Visibility Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private Visibility _emptyNoticeVisibility = Visibility.Collapsed;

        public Visibility EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyNoticeVisibility);
            }
        }

        private readonly HashSet<string> _filters = new HashSet<string>();
        private string _currrentFilter;

        #endregion
    }
}