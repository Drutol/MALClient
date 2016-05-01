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
using MALClient.Pages;

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
                ViewModelLocator.Main.PopulateSearchFilters(_filters);
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
            var response = "";
            _filters.Clear();
            _allAnimeSearchItems = new List<AnimeSearchItem>();
            if (_animeSearch)
            {
                await
                    Task.Run(
                        async () =>
                            response = await new AnimeSearchQuery(Utils.CleanAnimeTitle(query)).GetRequestResponse());
                try
                {
                    var parsedData = XDocument.Parse(response);
                    foreach (var item in parsedData.Element("anime").Elements("entry"))
                    {
                        var type = item.Element("type").Value;
                        var data = new AnimeGeneralDetailsData();
                        data.ParseXElement(item,true);
                        _allAnimeSearchItems.Add(new AnimeSearchItem(data));
                        if (!_filters.Contains(type))
                            _filters.Add(type);
                    }
                }
                catch (Exception) //if MAL returns nothing it returns unparsable xml ... 
                {
                    EmptyNoticeVisibility = Visibility.Visible;
                }
            }
            else // manga search
            {
                await
                    Task.Run(
                        async () =>
                            response = await new MangaSearchQuery(Utils.CleanAnimeTitle(query)).GetRequestResponse());
                try
                {
                    var parsedData = XDocument.Parse(response);
                    foreach (var item in parsedData.Element("manga").Elements("entry"))
                    {
                        var type = item.Element("type").Value;
                        var data = new AnimeGeneralDetailsData();
                        data.ParseXElement(item,false);
                        _allAnimeSearchItems.Add(new AnimeSearchItem(data, false));
                        if (!_filters.Contains(type))
                            _filters.Add(type);
                    }
                }
                catch (Exception) //if MAL returns nothing it returns unparsable xml ... 
                {
                    EmptyNoticeVisibility = Visibility.Visible;
                }
            }
            ViewModelLocator.Main.PopulateSearchFilters(_filters);
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