using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Items;
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
        #region Properties
        public ObservableCollection<AnimeSearchItem> AnimeSearchItems { get; } = 
            new ObservableCollection<AnimeSearchItem>();

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

        #endregion

        public string PrevQuery;
        private bool _animeSearch; // default to anime

        public void Init(SearchPageNavigationArgs args)
        {
            _animeSearch = args.Anime;
            if(args.Query != null)
                SubmitQuery(args.Query);
            else
            {
                AnimeSearchItems.Clear();
                ResetQuery();
            }

            NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }

        public async void SubmitQuery(string query)
        {
            if(query == PrevQuery)
                return;
            PrevQuery = query;
            Loading = Visibility.Visible;
            EmptyNoticeVisibility = Visibility.Collapsed;
            AnimeSearchItems.Clear();
            var response = "";
            if (_animeSearch)
            {
                await
                    Task.Run(
                        async () =>
                            response = await new AnimeSearchQuery(Utils.CleanAnimeTitle(query)).GetRequestResponse());
                try
                {
                    XDocument parsedData = XDocument.Parse(response);
                    foreach (XElement item in parsedData.Element("anime").Elements("entry"))
                    {
                        AnimeSearchItems.Add(new AnimeSearchItem(item));
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
                    XDocument parsedData = XDocument.Parse(response);
                    foreach (XElement item in parsedData.Element("manga").Elements("entry"))
                        AnimeSearchItems.Add(new AnimeSearchItem(item,false));
                }
                catch (Exception) //if MAL returns nothing it returns unparsable xml ... 
                {
                    EmptyNoticeVisibility = Visibility.Visible;
                }
            }
            Loading = Visibility.Collapsed;
        }

        private void ResetQuery()
        {
            PrevQuery = null;
        }
    }
}
