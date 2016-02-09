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
        public ObservableCollection<ListViewItem> LeftDetailsRow => _loadedItems1; 
        public ObservableCollection<ListViewItem> RightDetailsRow => _loadedItems2; 
        private ObservableCollection<ListViewItem> _loadedItems1 = new ObservableCollection<ListViewItem>();
        private ObservableCollection<ListViewItem> _loadedItems2 = new ObservableCollection<ListViewItem>();
        private int _allEpisodes;

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
            get { return _animeItemReference.MyEpisodes; }
            set
            {
                _animeItemReference.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
            }
        }

        public string MyStatusBind => Utils.StatusToString(MyStatus);
        private int MyStatus
        {
            get { return _animeItemReference.MyStatus; }
            set
            {
                _animeItemReference.MyStatus = value;
                RaisePropertyChanged(() => MyStatusBind);
            }
        }

        public string MyScoreBind => MyScore == 0 ? "Unranked" : $"{MyScore}/10";
        private int MyScore
        {
            get { return _animeItemReference.MyScore; }
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

        public void Init(AnimeDetailsPageNavigationArgs param)
        {
            Id = param.Id;
            Title = param.Title;
            _animeItemReference = param.AnimeItem;
            if (_animeItemReference == null || _animeItemReference is AnimeSearchItem || !(_animeItemReference as AnimeItem).Auth)
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

            if (_animeItemReference is AnimeItem && (_animeItemReference as AnimeItem).Auth)
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
        }

        private async void OpenMalPage()
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/anime/{Id}"));
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
            var response = await new AnimeUpdateQuery(Id, eps, MyStatus, MyScore).GetRequestResponse();
            if (response == "Updated")
            {
                MyEpisodes = eps;
                View.GetWatchedEpsFlyout().Hide();
                WatchedEpsInputNoticeVisibility = false;
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
            _animeItemReference = animeItem.AnimeItem;
            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;
            GlobalScore = GlobalScore; //trigger setter of anime item
            if (Status == "Currently Airing")
                (_animeItemReference as AnimeItem).Airing = true;
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

            ViewModelLocator.AnimeList.RemoveAnimeEntry((_animeItemReference as AnimeItem)._parentAbstraction);

            (_animeItemReference as AnimeItem).SetAuthStatus(false, true);
            AddAnimeVisibility = true;
            MyDetailsVisibility = false;
        }

        #endregion

        #region FetchAndPopulate

        private void PopulateData()
        {
            if (_animeItemReference is AnimeItem)
            {
                var day = Status == "Currently Airing" ? (int)DateTime.Parse(StartDate).DayOfWeek + 1 : -1;
                DataCache.RegisterVolatileData(Id, new VolatileDataCache
                {
                    DayOfAiring = day,
                    GlobalScore = GlobalScore
                });
                ((AnimeItem)_animeItemReference).Airing = day != -1;
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
                        BuildTextBlock(label,FontWeights.SemiBold,0),
                        BuildTextBlock(val1,FontWeights.SemiLight,1),
                    },
                },
                Background = new SolidColorBrush(alternate ? Color.FromArgb(170, 230, 230, 230) : Colors.Transparent),
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
            Synopsis =
                Regex.Replace(animeElement.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "")
                    .Trim()
                    .Replace("[i]", "")
                    .Replace("[/i]", "")
                    .Replace("#039;", "'")
                    .Replace("quot;", "\"")
                    .Replace("&mdash;", "—");
            StartDate = animeElement.Element("start_date").Value;
            EndDate = animeElement.Element("end_date").Value;
            _imgUrl = animeElement.Element("image").Value;
            if (_animeItemReference == null)
                AllEpisodes = Convert.ToInt32(animeElement.Element("episodes").Value);
            PopulateData();
        }

        private async void FetchData(string id, string title)
        {
            var data = "";
            await Task.Run(async () => data = await new AnimeSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse());
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Element("anime").Elements("entry");
            ExtractData(elements.First(element => element.Element("id").Value == id));
        }

        #endregion
    }
}
