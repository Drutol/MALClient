using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public interface IDetailsViewInteraction
    {
        Flyout GetWatchedEpsFlyout();
    }

    public class AnimeDetailsPageViewModel : ViewModelBase
    {
        private IAnimeData _animeItemReference;
        private float _globalScore;
        private string _imgUrl;
        private bool _loadedDetails;
        private bool _loadedReviews;
        public ObservableCollection<ListViewItem> LeftDetailsRow => _loadedItems1; 
        public ObservableCollection<ListViewItem> RightDetailsRow => _loadedItems2; 
        public ObservableCollection<ListViewItem> LeftGenres => _genres1;
        public ObservableCollection<ListViewItem> RightGenres => _genres2; 
        public ObservableCollection<ListViewItem> Episodes => _episodes; 
        public ObservableCollection<ListViewItem> OPs => _ops; 
        public ObservableCollection<ListViewItem> EDs => _eds;
        public ObservableCollection<AnimeReviewData> Reviews { get; set; } = new ObservableCollection<AnimeReviewData>(); 
        private readonly ObservableCollection<ListViewItem> _loadedItems1 = new ObservableCollection<ListViewItem>();
        private readonly ObservableCollection<ListViewItem> _loadedItems2 = new ObservableCollection<ListViewItem>();
        private readonly ObservableCollection<ListViewItem> _genres1 = new ObservableCollection<ListViewItem>();
        private readonly ObservableCollection<ListViewItem> _genres2 = new ObservableCollection<ListViewItem>();
        private readonly ObservableCollection<ListViewItem> _episodes = new ObservableCollection<ListViewItem>();     
        private readonly ObservableCollection<ListViewItem> _ops = new ObservableCollection<ListViewItem>();
        private readonly ObservableCollection<ListViewItem> _eds = new ObservableCollection<ListViewItem>();
        private int _allEpisodes;

        private string AnnId { get; set; }
        private int Id { get; set; }
        private string Title { get; set; }

        private int AllEpisodes
        {
            get
            {
                return _animeItemReference?.AllEpisodes ?? _allEpisodes;
            }
            set { _allEpisodes = value; }
        }
        private string Type { get; set; }
        private string Status { get; set; }
        private string _synopsis;
        public string Synopsis
        {
            get { return _synopsis; }
            set
            {
                _synopsis = value;
                RaisePropertyChanged(() => Synopsis);
            }
        }

        private List<string> _synonyms = new List<string>();

        private string StartDate { get; set; }
        private string EndDate { get; set; }

        private float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                if (_animeItemReference != null)
                    _animeItemReference.GlobalScore = value;
                _globalScore = value;
            }
        }

        public IDetailsViewInteraction View { get; set; }

        public string MyEpisodesBind => $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";
        private int MyEpisodes
        {
            get { return _animeItemReference?.MyEpisodes ?? 0; }
            set
            {
                _animeItemReference.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
            }
        }

        public string MyStatusBind => Utils.StatusToString(MyStatus);
        private int MyStatus
        {
            get { return _animeItemReference?.MyStatus ?? (int)AnimeStatus.AllOrAiring; }
            set
            {
                _animeItemReference.MyStatus = value;
                RaisePropertyChanged(() => MyStatusBind);
            }
        }

        public string MyScoreBind => MyScore == 0 ? "Unranked" : $"{MyScore}/10";
        private int MyScore
        {
            get { return _animeItemReference?.MyScore ?? 0; }
            set
            {
                _animeItemReference.MyScore = value;
                RaisePropertyChanged(() => MyScoreBind);
            }
        }

        private bool _loadingUpdate;
        public bool LoadingUpdate
        {
            get { return _loadingUpdate; }
            set
            {
                _loadingUpdate = value;
                RaisePropertyChanged(() => LoadingUpdate);
            }
        }

        private Visibility _loadingGlobal;
        public Visibility LoadingGlobal
        {
            get { return _loadingGlobal; }
            set
            {
                _loadingGlobal = value;
                RaisePropertyChanged(() => LoadingGlobal);
            }
        }

        private Visibility _loadingDetails;
        public Visibility LoadingDetails
        {
            get { return _loadingDetails; }
            set
            {
                _loadingDetails = value;
                RaisePropertyChanged(() => LoadingDetails);
            }
        }

        private Visibility _loadingReviews;
        public Visibility LoadingReviews
        {
            get { return _loadingReviews; }
            set
            {
                _loadingReviews = value;
                RaisePropertyChanged(() => LoadingReviews);
            }
        }

        private Visibility _detailedDataVisibility;
        public Visibility DetailedDataVisibility
        {
            get { return _detailedDataVisibility; }
            set
            {
                _detailedDataVisibility = value;
                RaisePropertyChanged(() => DetailedDataVisibility);
            }
        }

        private string _watchedEpsInput;
        public string WatchedEpsInput {
            get { return _watchedEpsInput; }
            set
            {
                _watchedEpsInput = value;
                RaisePropertyChanged(() => WatchedEpsInput);
            } }

        private bool _watchedEpsInputNoticeVisibility;
        public bool WatchedEpsInputNoticeVisibility
        {
            get { return _watchedEpsInputNoticeVisibility; }
            set
            {
                _watchedEpsInputNoticeVisibility = value;
                RaisePropertyChanged(() => WatchedEpsInputNoticeVisibility);
            }
        }

        private bool _myDetailsVisibility;
        public bool MyDetailsVisibility
        {
            get { return _myDetailsVisibility; }
            set
            {
                _myDetailsVisibility = value;
                RaisePropertyChanged(() => MyDetailsVisibility);
            }
        }

        private bool _addAnimeVisibility;
        public bool AddAnimeVisibility
        {
            get { return _addAnimeVisibility; }
            set
            {
                _addAnimeVisibility = value;
                RaisePropertyChanged(() => AddAnimeVisibility);
            }
        }

        private ICommand _changeStatusCommand;
        public ICommand ChangeStatusCommand
        {
            get
            {
                return _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand<Object>(ChangeStatus));
            }
        }

        private ICommand _changeScoreCommand;
        public ICommand ChangeScoreCommand
        {
            get
            {
                return _changeScoreCommand ?? (_changeScoreCommand = new RelayCommand<Object>(ChangeScore));
            }
        }

        private ICommand _changeWatchedCommand;
        public ICommand ChangeWatchedCommand
        {
            get
            {
                return _changeWatchedCommand ?? (_changeWatchedCommand = new RelayCommand(ChangeWatchedEps));
            }
        }

        private ICommand _addAnimeCommand;
        public ICommand AddAnimeCommand
        {
            get
            {
                return _addAnimeCommand ?? (_addAnimeCommand = new RelayCommand(AddAnime));
            }
        }

        private ICommand _removeAnimeCommand;
        public ICommand RemoveAnimeCommand
        {
            get
            {
                return _removeAnimeCommand ?? (_removeAnimeCommand = new RelayCommand(RemoveAnime));
            }
        }

        private ICommand _openInMalCommand;
        public ICommand OpenInMalCommand
        {
            get
            {
                return _openInMalCommand ?? (_openInMalCommand = new RelayCommand(OpenMalPage));
            }
        }

        private ICommand _openInAnnCommand;
        public ICommand OpenInAnnCommand
        {
            get
            {
                return _openInAnnCommand ?? (_openInAnnCommand = new RelayCommand(OpenAnnPage));
            }
        }

        private BitmapImage _detailImage;
        public BitmapImage DetailImage
        {
            get { return _detailImage; }
            set
            {
                _detailImage = value;
                RaisePropertyChanged(() => DetailImage);
            }
        }

        private Visibility _noEpisodesDataVisibility;
        public Visibility NoEpisodesDataVisibility
        {
            get { return _noEpisodesDataVisibility; }
            set
            {
                _noEpisodesDataVisibility = value;
                RaisePropertyChanged(() => NoEpisodesDataVisibility);
            }
        }

        private Visibility _noEDsDataVisibility;
        public Visibility NoEDsDataVisibility
        {
            get { return _noEDsDataVisibility; }
            set
            {
                _noEDsDataVisibility = value;
                RaisePropertyChanged(() => NoEDsDataVisibility);
            }
        }

        private Visibility _noOPsDataVisibility;
        public Visibility NoOPsDataVisibility
        {
            get { return _noOPsDataVisibility; }
            set
            {
                _noOPsDataVisibility = value;
                RaisePropertyChanged(() => NoOPsDataVisibility);
            }
        }

        private Visibility _noGenresDataVisibility;
        public Visibility NoGenresDataVisibility
        {
            get { return _noGenresDataVisibility; }
            set
            {
                _noGenresDataVisibility = value;
                RaisePropertyChanged(() => NoGenresDataVisibility);
            }
        }

        private int _detailsPivotSelectedIndex;
        public int DetailsPivotSelectedIndex
        {
            get { return _detailsPivotSelectedIndex; }
            set
            {
                _detailsPivotSelectedIndex = value;
                RaisePropertyChanged(() => DetailsPivotSelectedIndex);
            }
        }

        private Visibility _noReviewsDataNoticeVisibility = Visibility.Collapsed;
        public Visibility NoReviewsDataNoticeVisibility
        {
            get { return _noReviewsDataNoticeVisibility; }
            set
            {
                _noReviewsDataNoticeVisibility = value;
                RaisePropertyChanged(() => NoReviewsDataNoticeVisibility);
            }
        }

        public void Init(AnimeDetailsPageNavigationArgs param)
        {
            LoadingGlobal = Visibility.Visible;
            Id = param.Id;
            Title = param.Title;
            _animeItemReference = param.AnimeItem;
            if (_animeItemReference == null || _animeItemReference is AnimeSearchItem || !(_animeItemReference as AnimeItemViewModel).Auth)
            //if we are from search or from unauthenticated item let's look for proper abstraction
            {
                if (!ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(param.Id, ref _animeItemReference))
                // else we don't have this item
                {
                    //we may only prepare for its creation
                    AddAnimeVisibility = true;
                    MyDetailsVisibility = false;
                }
            } // else we already have it

            if (_animeItemReference is AnimeItemViewModel && (_animeItemReference as AnimeItemViewModel).Auth)
            {
                //we have item on the list , so there's valid data here
                MyDetailsVisibility = true;
                AddAnimeVisibility = false;
                RaisePropertyChanged(() => MyEpisodesBind);
                RaisePropertyChanged(() => MyStatusBind);
                RaisePropertyChanged(() => MyScoreBind);
            }

            switch (param.Source)
            {
                case PageIndex.PageSearch:
                    ExtractData(param.AnimeElement);
                    Utils.RegisterBackNav(param.Source, true);
                    break;
                case PageIndex.PageAnimeList:
                    FetchData(param.Id.ToString(), param.Title);
                    Utils.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
                case PageIndex.PageRecomendations:
                    ExtractData(param.AnimeElement);
                    Utils.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
            }
            _loadedDetails = _loadedReviews = false;
            DetailsPivotSelectedIndex = 0;
            Reviews.Clear();
        }

        private async void OpenMalPage()
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/anime/{Id}"));
        }

        private async void OpenAnnPage()
        {
            await Launcher.LaunchUriAsync(new Uri($"http://www.animenewsnetwork.com/encyclopedia/anime.php?id={AnnId}"));
        }

        #region ChangeStuff

        private async void ChangeStatus(object status)
        {
            LoadingUpdate = true;
            var prevStatus = MyStatus;
            MyStatus = Utils.StatusToInt(status as string);
            var response = await new AnimeUpdateQuery(Id, MyEpisodes, MyStatus, MyScore).GetRequestResponse();
            if (response != "Updated")
                MyStatus = prevStatus;

            LoadingUpdate = false;
        }

        private async void ChangeScore(object score)
        {
            LoadingUpdate = true;
            var prevScore = MyScore;
            MyScore = Convert.ToInt32(score as string);
            var response = await new AnimeUpdateQuery(Id, MyEpisodes, MyStatus, MyScore).GetRequestResponse();
            if (response != "Updated")
                MyScore = prevScore;
            LoadingUpdate = false;
        }

        public async void ChangeWatchedEps()
        {
            LoadingUpdate = true;
            int eps;
            if (!int.TryParse(WatchedEpsInput, out eps))
            {
                WatchedEpsInputNoticeVisibility = true;
                LoadingUpdate = false;
                return;
            }
            if (eps >= 0 && (AllEpisodes == 0 || eps <= AllEpisodes))
            {
                View.GetWatchedEpsFlyout().Hide();
                WatchedEpsInputNoticeVisibility = false;
                var prevWatched = MyEpisodes;
                MyEpisodes = eps;
                var response = await new AnimeUpdateQuery(Id,MyEpisodes,MyStatus,MyScore).GetRequestResponse();
                if (response != "Updated")
                    MyEpisodes = prevWatched;

                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = true;
            }
            LoadingUpdate = false;
        }



        #endregion

        #region Add/Remove

        private async void AddAnime()
        {
            var response = await new AnimeAddQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Created"))
                return;
            AddAnimeVisibility = false;
            var animeItem = new AnimeItemAbstraction(true, Title, _imgUrl, Id, 6, 0, AllEpisodes, 0);
            _animeItemReference = animeItem.ViewModel;
            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;
            GlobalScore = GlobalScore; //trigger setter of anime item
            if (Status == "Currently Airing")
                (_animeItemReference as AnimeItemViewModel).Airing = true;
            ViewModelLocator.AnimeList.AddAnimeEntry(animeItem);
            MyDetailsVisibility = true;
        }

        private async void RemoveAnime()
        {
            var uSure = false;
            var msg = new MessageDialog("Are you sure about deleting this entry from your list?");
            msg.Commands.Add(new UICommand("I'm sure", command => uSure = true));
            msg.Commands.Add(new UICommand("Cancel", command => uSure = false));
            await msg.ShowAsync();
            if (!uSure)
                return;

            var response = await new AnimeRemoveQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Deleted"))
                return;

            ViewModelLocator.AnimeList.RemoveAnimeEntry((_animeItemReference as AnimeItemViewModel)._parentAbstraction);

            (_animeItemReference as AnimeItemViewModel).SetAuthStatus(false, true);
            AddAnimeVisibility = true;
            MyDetailsVisibility = false;
        }

        #endregion

        #region FetchAndPopulate

        private void PopulateData()
        {
            if (_animeItemReference is AnimeItemViewModel)
            {
                var day = Status == "Currently Airing" ? (int)DateTime.Parse(StartDate).DayOfWeek + 1 : -1;
                DataCache.RegisterVolatileData(Id, new VolatileDataCache
                {
                    DayOfAiring = day,
                    GlobalScore = GlobalScore
                });
                ((AnimeItemViewModel)_animeItemReference).Airing = day != -1;
                DataCache.SaveVolatileData();
            }
            _loadedItems1.Clear();
            _loadedItems2.Clear();
            _loadedItems1.Add(BuildListViewItem("Episodes", AllEpisodes == 0 ? "?" : AllEpisodes.ToString(), true));
            _loadedItems1.Add(BuildListViewItem("Score", GlobalScore.ToString()));
            _loadedItems1.Add(BuildListViewItem("Start", StartDate, true));
            _loadedItems2.Add(BuildListViewItem("Type", Type, true, 0.3f, 0.7f));
            _loadedItems2.Add(BuildListViewItem("Status", Status, false, 0.3f, 0.7f));
            _loadedItems2.Add(BuildListViewItem("End", EndDate, true, 0.3f, 0.7f));

            Synopsis = Synopsis;
            Utils.GetMainPageInstance().CurrentStatus = Title;

            DetailImage  = new BitmapImage(new Uri(_imgUrl));
            LoadingGlobal = Visibility.Collapsed;
        }

        private ListViewItem BuildListViewItem(string label, string val1, bool alternate = false, float left = 0.4f, float right = 0.6f)
        {
            return new ListViewItem
            {
                Content = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = new GridLength(left,GridUnitType.Star)},
                        new ColumnDefinition {Width = new GridLength(right,GridUnitType.Star)}
                    },
                    Children =
                    {
                        BuildTextBlock(label,val1 == "" ? FontWeights.SemiLight :FontWeights.SemiBold,0),
                        BuildTextBlock(val1,FontWeights.SemiLight,1),
                    },
                },
                Background = new SolidColorBrush(alternate ? Color.FromArgb(170, 230, 230, 230) : Color.FromArgb(255, 245, 245, 245)),
                Height = 20
            };
        }

        private TextBlock BuildTextBlock(string value, FontWeight weight, int column)
        {
            var txt = new TextBlock
            {
                Text = value,
                FontWeight = weight,
                FontSize = 13,
                Height = 20,
                TextAlignment = !weight.Equals(FontWeights.SemiBold) ? TextAlignment.Center : TextAlignment.Left
            };
            txt.SetValue(Grid.ColumnProperty, column);
            return txt;
        }


        private void ExtractData(XElement animeElement)
        {
            GlobalScore = float.Parse(animeElement.Element("score").Value);
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;
            Synopsis = Utils.DecodeXmlSynopsis(animeElement.Element("synopsis").Value);               
            StartDate = animeElement.Element("start_date").Value;
            EndDate = animeElement.Element("end_date").Value;
            _imgUrl = animeElement.Element("image").Value;
            _synonyms = animeElement.Element("synonyms").Value.Split(',').ToList();
            _synonyms.Add(animeElement.Element("english").Value);
            _synonyms.Add(Title);
            _synonyms = _synonyms.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (_animeItemReference == null)
                AllEpisodes = Convert.ToInt32(animeElement.Element("episodes").Value);
            PopulateData();
        }

        private async Task FetchData(string id, string title)
        {
            LoadingGlobal = Visibility.Visible;
            var data = "";
            await Task.Run(async () => data = await new AnimeSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse());
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Element("anime").Elements("entry");
            ExtractData(elements.First(element => element.Element("id").Value == id));
        }

        public async void RefreshData()
        {
            await FetchData(Id.ToString(),Title);
            if(_loadedDetails)
                LoadDetails(true);
            if(_loadedReviews)
                LoadReviews(true);
        }

        public async void LoadDetails(bool force = false)
        {
            if (_loadedDetails && !force)
                return;
            _loadedDetails = true;
            LoadingDetails = Visibility.Visible;
            try
            {
                var data = await new AnimeGeneralDetailsQuery(_synonyms.Count == 1 ? Title : string.Join("&title=~",_synonyms),Id,Title).GetGeneralDetailsData(force);
                _genres1.Clear();
                _genres2.Clear();
                _episodes.Clear();
                _ops.Clear();
                _eds.Clear();
                AnnId = data.AnnId;
                int i = 1;
                bool alternate1 = true, alternate2 = true;
                foreach (var genre in data.Genres)
                {
                    if (i % 2 == 0)
                    {
                        _genres1.Add(BuildListViewItem(Utils.FirstCharToUpper(genre),"", alternate1, 1f, 0f));
                        alternate1 = !alternate1;
                    }
                    else
                    {
                        _genres2.Add(BuildListViewItem(Utils.FirstCharToUpper(genre), "", alternate2, 1f, 0f));
                        alternate2 = !alternate2;
                    }
                    i++;
                }
                i = 1;
                alternate1 = false;
                foreach (var episode in data.Episodes.Take(40))
                {
                        _episodes.Add(BuildListViewItem($"{i++}.", episode, alternate1, 0.1f, 0.9f));
                        alternate1 = !alternate1;
                }
                if (data.Episodes.Count > 40)
                {
                    _episodes.Add(BuildListViewItem("?.", $"{data.Episodes.Count - 40} More episodes...", alternate1, 0.1f, 0.9f));
                }
                i = 1;
                alternate1 = true;
                foreach (var op in data.OPs)
                {
                    _ops.Add(BuildListViewItem(op,"", alternate1, 1f, 0f));
                    alternate1 = !alternate1;
                    i++;
                }
                i = 1;
                alternate1 = true;
                foreach (var ed in data.EDs)
                {
                    _eds.Add(BuildListViewItem( ed,"", alternate1, 1f, 0f));
                    alternate1 = !alternate1;
                    i++;
                }
                NoEpisodesDataVisibility = _episodes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                NoGenresDataVisibility = _genres1.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                NoEDsDataVisibility = _eds.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                NoOPsDataVisibility = _ops.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                DetailedDataVisibility = Visibility.Visible;
            }
            catch (Exception)
            {
                DetailedDataVisibility = Visibility.Collapsed;
            }
            
            LoadingDetails = Visibility.Collapsed;
        }

        public async void LoadReviews(bool force = false)
        {
            if(_loadedReviews && !force)
                return;
            LoadingReviews = Visibility.Visible;
            _loadedReviews = true;
            Reviews.Clear();
            List<AnimeReviewData> revs = new List<AnimeReviewData>();
            await Task.Run( async () => revs = await new AnimeReviewsQuery(Id).GetAnimeReviews(force));
            
            foreach (var rev in revs)
            {
                Reviews.Add(rev);
            }
            NoReviewsDataNoticeVisibility = Reviews.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LoadingReviews = Visibility.Collapsed;
        }
        #endregion


    }
}
