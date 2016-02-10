using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
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
    public interface IAnimeItemInteractions
    {
        Flyout WatchedFlyout { get; }
    }

    public class AnimeItemViewModel : ViewModelBase , IAnimeData
    {
        private readonly string _imgUrl;
        
        private int _allEpisodes;
        //state fields
        private bool _expandState;
        
        private float _globalScore;
        public readonly AnimeItemAbstraction _parentAbstraction;
        private readonly bool _seasonalState;
        //prop field pairs

        public IAnimeItemInteractions View;

        #region Constructors
        private AnimeItemViewModel(string img, int id, AnimeItemAbstraction parent)
        {           
            _parentAbstraction = parent;
            _imgUrl = img;
            Id = id;
            Image = new BitmapImage(new Uri(_imgUrl));
        }

        public AnimeItemViewModel(bool auth, string name, string img, int id, int myStatus, int myEps, int allEps, int myScore,
            AnimeItemAbstraction parent, bool setEpsAuth = false) : this(img, id, parent)
            //We are loading an item that IS on the list
        {
            //Assign fields
            Id = id;
            _allEpisodes = allEps;
            Auth = auth;
            //Assign properties
            MyStatus = myStatus;
            MyScore = myScore;
            Title = name;
            MyEpisodes = myEps;
            ShowMoreVisibility = false;

            //We are not seasonal so it's already on list            
            AddToListVisibility = false;

            SetAuthStatus(auth, setEpsAuth);
            AdjustIncrementButtonsVisibility();
            //There may be additional data available
            GlobalScore = _parentAbstraction.GlobalScore;
            if (_parentAbstraction.AirDay != -1)
                Airing = true;
        }

        public AnimeItemViewModel(SeasonalAnimeData data,
            AnimeItemAbstraction parent) : this(data.ImgUrl, data.Id, parent)
            //We are loading an item that is NOT on the list and is seasonal
        {
            _seasonalState = true;
            //Assign Fields
            _parentAbstraction.MyEpisodes = 0; // We don't want to set TextBlock
            //Assign properties
            MyStatus = (int)AnimeStatus.AllOrAiring;
            Title = data.Title;
            MyScore = 0;
            GlobalScore = data.Score;
            int.TryParse(data.Episodes, out _allEpisodes);
            if (data.Genres != null)
                Genres = data.Genres;
            //SeasonalMembers = data.Members;

            //Custom controls setup
            
            Airing = true;
            //Additional data from seasonal
            Synopsis = data.Synopsis;
            //We are not on the list so we cannot really do this
            SetAuthStatus(false, true);
            AdjustIncrementButtonsVisibility();
        }
        #endregion

        #region PropertyPairs
        public string AirDayBind => Utils.DayToString((DayOfWeek)(_parentAbstraction.AirDay - 1));
        private bool _airing;
        public bool Airing
        {
            get { return _airing; }
            set
            {
                if (_parentAbstraction.TryRetrieveVolatileData())
                {
                    RaisePropertyChanged(() => AirDayBind);
                    TitleMargin = new Thickness(5, 3, 50, 0);
                }
                _airing = value;
                RaisePropertyChanged(() => Airing);
            }
        }

        private Thickness _titleMargin = new Thickness(5,3,5,3);
        public Thickness TitleMargin
        {
            get { return _titleMargin; }
            set
            {
                _titleMargin = value;
                RaisePropertyChanged(() => TitleMargin);
            }
        }

        private List<string> _genres = new List<string>();
        private List<string> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value;
                RaisePropertyChanged(() => Genres);
            }
        }

        public bool Auth { get; private set; }

        public string MyStatusBind => Utils.StatusToString(MyStatus);
        public int MyStatus
        {
            get { return _parentAbstraction.MyStatus; }
            set
            {
                _parentAbstraction.MyStatus = value;
                RaisePropertyChanged(() => MyStatusBind);
            }
        }

        public string MyScoreBind => MyScore == 0 ? "Unranked" : $"{MyScore}/10";
        public int MyScore
        {
            get { return _parentAbstraction.MyScore; }
            set
            {
                _parentAbstraction.MyScore = value;
                RaisePropertyChanged(() => MyScoreBind);
            }
        }

        public string MyEpisodesBind => Auth ? $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}" : $"{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())} Episodes";
        public int MyEpisodes
        {
            get { return _parentAbstraction.MyEpisodes; }
            set
            {
                _parentAbstraction.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string GlobalScoreBind => GlobalScore == 0 ? "" : GlobalScore.ToString();
        public float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                if (value == 0)
                    return;
                _globalScore = value;
                RaisePropertyChanged(() => GlobalScoreBind);
            }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        private bool _updateButtonsEnableState;
        public bool UpdateButtonsEnableState
        {
            get { return _updateButtonsEnableState; }
            set
            {
                _updateButtonsEnableState = value;
                RaisePropertyChanged(() => UpdateButtonsEnableState);
            }
        }

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

        private bool _addToListVisibility;
        public bool AddToListVisibility
        {
            get { return _addToListVisibility; }
            set
            {
                _addToListVisibility = value;
                RaisePropertyChanged(() => AddToListVisibility);
            }
        }

        private bool _incrementEpsVisibility;
        public bool IncrementEpsVisibility
        {
            get { return _incrementEpsVisibility; }
            set
            {
                _incrementEpsVisibility = value;
                RaisePropertyChanged(() => IncrementEpsVisibility);
            }
        }

        private bool _decrementEpsVisibility;
        public bool DecrementEpsVisibility
        {
            get { return _decrementEpsVisibility; }
            set
            {
                _decrementEpsVisibility = value;
                RaisePropertyChanged(() => DecrementEpsVisibility);
            }
        }

        private bool _showMoreVisiblity;
        public bool ShowMoreVisibility
        {
            get { return _showMoreVisiblity; }
            set
            {
                _showMoreVisiblity = value;
                RaisePropertyChanged(() => ShowMoreVisibility);
            }
        }

        private bool _updateButtonsVisibility;
        public bool UpdateButtonsVisibility
        {
            get { return _updateButtonsVisibility; }
            set
            {
                _updateButtonsVisibility = value;
                RaisePropertyChanged(() => UpdateButtonsVisibility);
            }
        }

        private bool _tileUrlInputVisibility;
        public bool TileUrlInputVisibility
        {
            get { return _tileUrlInputVisibility; }
            set
            {
                _tileUrlInputVisibility = value;
                RaisePropertyChanged(() => TileUrlInputVisibility);
            }
        }

        private string _tileUrlInput;
        public string TileUrlInput
        {
            get { return _tileUrlInput; }
            set
            {
                _tileUrlInput = value;
                RaisePropertyChanged(() => TileUrlInput);
            }
        }

        private string _watchedEpsInput;
        public string WatchedEpsInput
        {
            get { return _watchedEpsInput; }
            set
            {
                _watchedEpsInput = value;
                RaisePropertyChanged(() => WatchedEpsInput);
            }
        }

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

        private Brush _rootBrush;
        public Brush RootBrush
        {
            get { return _rootBrush; }
            set
            {
                _rootBrush = value;
                RaisePropertyChanged(() => RootBrush);
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

        private ICommand _incrementWatchedCommand;
        public ICommand IncrementWatchedCommand
        {
            get
            {
                return _incrementWatchedCommand ?? (_incrementWatchedCommand = new RelayCommand(IncrementWatchedEp));
            }
        }

        private ICommand _decrementWatchedCommand;
        public ICommand DecrementWatchedCommand
        {
            get
            {
                return _decrementWatchedCommand ?? (_decrementWatchedCommand = new RelayCommand(DecrementWatchedEp));
            }
        }

        private ICommand _addAnimeCommand;
        public ICommand AddAnimeCommand
        {
            get
            {
                return _addAnimeCommand ?? (_addAnimeCommand = new RelayCommand(AddThisToMyList));
            }
        }

        private ICommand _pinTileCommand;
        public ICommand PinTileCommand
        {
            get
            {
                return _pinTileCommand ?? (_pinTileCommand = new RelayCommand(PinTile));
            }
        }

        private ICommand _navigateDetailsCommand;
        public ICommand NavigateDetailsCommand
        {
            get
            {
                return _addAnimeCommand ?? (_addAnimeCommand = new RelayCommand(NavigateDetails));
            }
        }

        #endregion
        //fields
        public int Id { get; set; }
        //props
        //private int SeasonalMembers { get; set; } //TODO : Use this
        public int AllEpisodes => _allEpisodes;

        public async void NavigateDetails()
        {
            await Utils.GetMainPageInstance()
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Id, Title, null, this,
                        Utils.GetMainPageInstance().GetCurrentListOrderParams())
                    { Source = PageIndex.PageAnimeList });
        }

        public void UpdateWithSeasonData(SeasonalAnimeData data)
        {
            GlobalScore = data.Score;
            if (data.Genres != null)
                Genres = data.Genres;
            Airing = true;

        }

        private async void AddThisToMyList()
        {
            var response = await new AnimeAddQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Created"))
                return;

            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;


            SetAuthStatus(true);
            AdjustIncrementButtonsVisibility();
            AddToListVisibility = false;
            ViewModelLocator.AnimeList.AddAnimeEntry(_parentAbstraction);
        }

        #region Utils/Helpers

        public async void PinTile(string targetUrl)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile thumb = await folder.CreateFileAsync($"{Id}.png", CreationCollisionOption.ReplaceExisting);

            var http = new HttpClient();
            byte[] response = await http.GetByteArrayAsync(_imgUrl); //get bytes

            Stream fs = await thumb.OpenStreamForWriteAsync(); //get stream

            using (var writer = new DataWriter(fs.AsOutputStream()))
            {
                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();
            }

            if (!targetUrl.Contains("http"))
                targetUrl = "http://" + targetUrl;
            var til = new SecondaryTile($"{Id}", $"{Title}", targetUrl, new Uri($"ms-appdata:///local/{Id}.png"),
                TileSize.Default);
            Utils.RegisterTile(Id.ToString());
            await til.RequestCreateAsync();
        }

        //Pinned with custom link.
        public void PinTile()
        {
            PinTile(TileUrlInput);
            TileUrlInputVisibility = false;
        }


        /// <summary>
        ///     Used by alternate row colors
        /// </summary>
        /// <param name="brush"></param>
        public void Setbackground(SolidColorBrush brush)
        {
            RootBrush = brush;
        }



        public void SetAuthStatus(bool auth, bool eps = false)
        {
            Auth = auth;
            if (auth)
            {
                AddToListVisibility = false;
                UpdateButtonsVisibility = true;
                UpdateButtonsEnableState = true;
            }
            else
            {
                AddToListVisibility = _seasonalState && Creditentials.Authenticated;
                UpdateButtonsEnableState = false;

                if (eps)
                {
                    RaisePropertyChanged(() => MyEpisodesBind);
                    UpdateButtonsVisibility = false;
                }
            }
            AdjustIncrementButtonsVisibility();
        }

        private void AdjustIncrementButtonsVisibility()
        {
            if (!Auth || !Creditentials.Authenticated)
            {
                IncrementEpsVisibility = false;
                DecrementEpsVisibility = false;
                return;
            }

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
            {
                IncrementEpsVisibility = false;
                DecrementEpsVisibility = true;
            }
            else if (MyEpisodes == 0)
            {
                IncrementEpsVisibility = true;
                DecrementEpsVisibility = false;
            }
            else
            {
                IncrementEpsVisibility = true;
                DecrementEpsVisibility = true;
            }
        }

        #endregion

        #region AnimeUpdate

        #region Watched

        private async void IncrementWatchedEp()
        {
            LoadingUpdate = true;

            if (MyStatus == (int)AnimeStatus.PlanToWatch || MyStatus == (int)AnimeStatus.Dropped ||
                MyStatus == (int)AnimeStatus.OnHold)
                PromptForStatusChange((int)AnimeStatus.Watching);

            MyEpisodes++;
            var response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyEpisodes--; // Shouldn't occur really , but hey shouldn't and MAL api goes along very well.

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
                PromptForStatusChange((int)AnimeStatus.Completed);


            AdjustIncrementButtonsVisibility();

            LoadingUpdate = false;
        }

        private async void DecrementWatchedEp()
        {
            LoadingUpdate = true;
            MyEpisodes--;
            var response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyEpisodes++;

            AdjustIncrementButtonsVisibility();

            LoadingUpdate = false;
        }

        public async void ChangeWatchedEps()
        {
            int watched;
            if (!int.TryParse(WatchedEpsInput, out watched))
            {
                WatchedEpsInputNoticeVisibility = true;
                return;
            }
            if (watched >= 0 && (_allEpisodes == 0 || watched <= _allEpisodes))
            {
                View.WatchedFlyout.Hide();
                LoadingUpdate = true;
                WatchedEpsInputNoticeVisibility = false;
                var prevWatched = MyEpisodes;
                MyEpisodes = watched;
                var response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    MyEpisodes = prevWatched;

                if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
                    PromptForStatusChange((int)AnimeStatus.Completed);

                AdjustIncrementButtonsVisibility();


                LoadingUpdate = false;
                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = true;
            }
        }



        #endregion

        private async void ChangeStatus(object status)
        {
            LoadingUpdate = true;
            var myPrevStatus = MyStatus;
            MyStatus = Utils.StatusToInt(status as string);
            var response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyStatus = myPrevStatus;

            if (MyStatus == (int)AnimeStatus.Completed && _allEpisodes != 0)
                PromptForWatchedEpsChange(_allEpisodes);

            LoadingUpdate = false;
        }

        private async void ChangeScore(object score)
        {
            LoadingUpdate = true;
            var myPrevScore = MyScore;
            MyScore = Convert.ToInt32(score);
            var response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyScore = myPrevScore;

            LoadingUpdate = false;
        }

        #endregion

        #region Prompts

        private async void PromptForStatusChange(int to)
        {
            if (MyStatus == to)
                return;
            var msg = new MessageDialog($"From : {Utils.StatusToString(MyStatus)}\nTo : {Utils.StatusToString(to)}",
                "Would you like to change current status?");
            var confirmation = false;
            msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();
            if (confirmation)
            {
                var myPrevStatus = MyStatus;
                MyStatus = to;
                var response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    MyStatus = myPrevStatus;
            }
        }

        private async void PromptForWatchedEpsChange(int to)
        {
            if (MyEpisodes == to)
                return;
            var msg = new MessageDialog($"From : {MyEpisodes}\nTo : {to}",
                "Would you like to change watched episodes value?");
            var confirmation = false;
            msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();
            if (confirmation)
            {
                var myPrevEps = MyEpisodes;
                MyEpisodes = to;
                var response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    MyStatus = myPrevEps;

                AdjustIncrementButtonsVisibility();
            }
        }

        #endregion
    }


}
