using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm;
using MALClient.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl , IAnimeData
    {
        //prop field pairs
        private string _title;
        private float _globalScore;
        private bool _airing = false;
        private List<string> _genres = new List<string>(); 

        public int MyStatus
        {
            get { return _parentAbstraction.MyStatus; }
            set
            {
                _parentAbstraction.MyStatus = value;
                BtnStatus.Content = Utils.StatusToString(value);
            }
        }
        public int MyScore
        {
            get { return _parentAbstraction.MyScore; }
            set
            {
                _parentAbstraction.MyScore = value;
                BtnScore.Content = value > 0 ? $"{value}/10" : "Unranked";
            }
        }
        public int MyEpisodes
        {
            get { return _parentAbstraction.MyEpisodes; }
            set
            {
                _parentAbstraction.MyEpisodes = value;
                BtnWatchedEps.Content = $"{value}/{(_allEpisodes == 0 ? "?" : _allEpisodes.ToString())}";
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                TxtTitle.Text = value;
            }
        }
        public float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                if(value == 0)
                    return;
                _globalScore = value;
                TxtGlobalSocore.Text = value.ToString();
                SymbolGlobalScore.Visibility = Visibility.Visible;
            }
        }

        public bool Airing
        {
            get { return _airing; }
            set
            {
                if (_parentAbstraction.TryRetrieveVolatileData())
                {
                    TxtAiringDay.Text = Utils.DayToString((DayOfWeek)(_parentAbstraction.AirDay - 1));
                    TxtTitle.Margin = new Thickness(5, 3, 50, 0);
                }
                SymbolAiring.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                _airing = value;
            }
        }

        private List<string> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value;
                foreach (var genre in value)
                    TxtGenres.Text += genre + "    ";
            }
        } 

        //fields
        public int Id { get; set; }
        private readonly string _imgUrl;
        private int _allEpisodes;
        internal AnimeItemAbstraction _parentAbstraction;
        //state fields
        private bool _expandState = false;
        private bool _seasonalState = false;
        private bool _auth;
        //props
        //private int SeasonalMembers { get; set; } //TODO : Use this
        public int AllEpisodes => _allEpisodes;
        public bool Auth => _auth;


        private AnimeItem(string img,int id,AnimeItemAbstraction parent)
        {
            this.InitializeComponent();
            _parentAbstraction = parent;
            _imgUrl = img;
            Id = id;
            Img.Source = new BitmapImage(new Uri(_imgUrl));
        }

        public AnimeItem(bool auth,string name,string img,int id,int myStatus,int myEps,int allEps,int myScore, AnimeItemAbstraction parent) : this(img,id,parent) //We are loading an item that IS on the list
        {
            //Assign fields
            Id = id;
            _allEpisodes = allEps;
            _auth = auth;
            //Assign properties
            MyStatus = myStatus;
            MyScore = myScore;
            Title = name;
            MyEpisodes = myEps;
            BtnShowMore.Visibility = Visibility.Collapsed;
           
            //We are not seasonal so it's already on list            
            BtnAddToList.Visibility = Visibility.Collapsed;

            SetAuthStatus(auth);          
            AdjustIncrementButtonsVisibility();
            //There may be additional data available
            GlobalScore = _parentAbstraction.GlobalScore;
            if (_parentAbstraction.AirDay != -1)
                Airing = true;

        }

        public AnimeItem(SeasonalAnimeData data, Dictionary<int, AnimeItemAbstraction> loaded,AnimeItemAbstraction parent) :this(data.ImgUrl,data.Id,parent)//We are loading an item that is NOT on the list and is seasonal
        {
            _seasonalState = true;
            //Assign Fields
            _parentAbstraction.MyEpisodes = 0; // We don't want to set TextBlock
            //Assign properties
            MyStatus = (int) AnimeStatus.AllOrAiring;
            Title = data.Title;
            MyScore = 0;
            GlobalScore = data.Score;
            if(data.Genres != null)
            Genres = data.Genres;
            //SeasonalMembers = data.Members;

            //Custom controls setup
            BtnWatchedEps.Content = $"{data.Episodes} Episodes";
            Airing = true;
            //Additional data from seasonal
            TxtSynopsis.Text = data.Synopsis;          
            SymbolGlobalScore.Visibility = Visibility.Visible;
            
            //We are not on the list so we cannot really do this
            SetAuthStatus(false,true);
            AdjustIncrementButtonsVisibility();

            if(loaded == null)
                return;
            Task.Run(async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    AnimeItemAbstraction reference;
                    if (loaded.TryGetValue(Id, out reference))
                    {
                        _allEpisodes = reference.AllEpisodes;
                        MyStatus = reference.MyStatus;
                        MyScore = reference.MyScore;
                        MyEpisodes = reference.MyEpisodes;

                        SetAuthStatus(true);
                        AdjustIncrementButtonsVisibility();
                        var dataCache = Utils.GetMainPageInstance().RetrieveLoadedAnime();
                        dataCache.LoadedAnime.Remove(reference);
                        dataCache.LoadedAnime.Add(_parentAbstraction);

                    }
                });
            });
        }

        #region Utils/Helpers

        /// <summary>
        /// Creates tile with series cover as background , leading to certain URI.
        /// </summary>
        /// <param name="targetUrl"></param>
        public async void PinTile(string targetUrl)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var thumb = await folder.CreateFileAsync($"{Id}.png", CreationCollisionOption.ReplaceExisting);

            HttpClient http = new HttpClient();
            byte[] response = await http.GetByteArrayAsync(_imgUrl); //get bytes

            var fs = await thumb.OpenStreamForWriteAsync(); //get stream

            using (DataWriter writer = new DataWriter(fs.AsOutputStream()))
            {
                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();
            }

            if (!targetUrl.Contains("http"))
                targetUrl = "http://" + targetUrl;
            var til = new SecondaryTile($"{Id}", $"{Title}", targetUrl, new Uri($"ms-appdata:///local/{Id}.png"), TileSize.Default);
            Utils.RegisterTile(Id.ToString());
            await til.RequestCreateAsync();
        }
        //Pinned with custom link.
        private void PinTile(object sender, RoutedEventArgs e)
        {
            PinTile(TxtTileUrl.Text);
            TileUrlInput.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Used by alternate row colors
        /// </summary>
        /// <param name="brush"></param>
        public void Setbackground(SolidColorBrush brush)
        {
            Root.Background = brush;
        }

        //Synopsis stuff
        private void ShowMore(object sender, RoutedEventArgs e)
        {
            if (!_expandState)
            {
                SynopsisShow.Begin();
                _expandState = true;
            }
            else
            {
                SynopsisHide.Begin();
                _expandState = false;
            }
        }

        public void SetAuthStatus(bool auth,bool eps = false)
        {
            _auth = auth;
            if (auth)
            {
                BtnAddToList.Visibility = Visibility.Collapsed;
                BtnStatus.Visibility = Visibility.Visible;
                BtnScore.Visibility = Visibility.Visible;
                BtnStatus.IsEnabled = true;
                BtnScore.IsEnabled = true;
                BtnWatchedEps.IsEnabled = true;
            }
            else
            {
                BtnAddToList.Visibility = _seasonalState ? Visibility.Visible : Visibility.Collapsed;
                BtnStatus.IsEnabled = false;
                BtnScore.IsEnabled = false;
                BtnWatchedEps.IsEnabled = false;

                if(eps)
                {
                    BtnWatchedEps.Content = $"{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())} Episodes";
                    BtnStatus.Visibility = Visibility.Collapsed;
                    BtnScore.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void AdjustIncrementButtonsVisibility()
        {
            if (!_auth)
            {
                IncrementEps.Visibility = Visibility.Collapsed;
                DecrementEps.Visibility = Visibility.Collapsed;
                return;
            }

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
            {
                IncrementEps.Visibility = Visibility.Collapsed;
                DecrementEps.Visibility = Visibility.Visible;
            }
            else if (MyEpisodes == 0)
            {
                IncrementEps.Visibility = Visibility.Visible;
                DecrementEps.Visibility = Visibility.Collapsed;
            }
            else
            {
                IncrementEps.Visibility = Visibility.Visible;
                DecrementEps.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Swipe
        private Point _initialPoint;
        private bool _manipulating;
        private MessageDialog _msg;
        /// <summary>
        /// When manipulation starts , saves initial point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
            _manipulating = true;
        }

        /// <summary>
        /// Calculates distance , navigates to anime details if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && _manipulating)
            {
                Point currentpoint = e.Position;
                if (currentpoint.X - _initialPoint.X >= 70) // swipe right
                {
                    e.Complete();
                    e.Handled = true;
                    _manipulating = false;
                    Utils.GetMainPageInstance() //If we are not authenticated msg box will appear.
                        .Navigate(PageIndex.PageAnimeDetails,
                            new AnimeDetailsPageNavigationArgs(Id, Title, null,this,
                                Utils.GetMainPageInstance().GetCurrentListOrderParams(_seasonalState)));

                }
            }
        }
        #endregion

        #region AnimeUpdate

        #region Watched
        private async void IncrementWatchedEp(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;

            if(MyStatus == (int)AnimeStatus.PlanToWatch || MyStatus == (int)AnimeStatus.Dropped || MyStatus == (int)AnimeStatus.OnHold)
                PromptForStatusChange((int)AnimeStatus.Watching);

            MyEpisodes++;
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyEpisodes--; // Shouldn't occur really , but hey shouldn't and MAL api goes along very well.

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
                PromptForStatusChange((int)AnimeStatus.Completed);

            
            AdjustIncrementButtonsVisibility();

            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void DecrementWatchedEp(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            MyEpisodes--;
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyEpisodes++;

            AdjustIncrementButtonsVisibility();

            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeWatchedEps(object sender, RoutedEventArgs e)
        {
            int watched;
            if (!int.TryParse(TxtBoxWatchedEps.Text, out watched))
            {
                TxtWatchedInvalidInputNotice.Visibility = Visibility.Visible;
                return;
            }
            if (watched >= 0 && (_allEpisodes == 0 || watched <= _allEpisodes))
            {
                WatchedEpsFlyout.Hide();
                SpinnerLoading.Visibility = Visibility.Visible;
                TxtWatchedInvalidInputNotice.Visibility = Visibility.Collapsed;
                int prevWatched = MyEpisodes;
                MyEpisodes = watched;
                string response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    MyEpisodes = prevWatched;

                if(MyEpisodes == _allEpisodes && _allEpisodes != 0)
                    PromptForStatusChange((int)AnimeStatus.Completed);

                AdjustIncrementButtonsVisibility();


                SpinnerLoading.Visibility = Visibility.Collapsed;
                TxtBoxWatchedEps.Text = "";
            }
        }

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Enter)
                ChangeWatchedEps(null,null);
        }
        #endregion

        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var item = sender as MenuFlyoutItem;
            var myPrevStatus = MyStatus;
            MyStatus = Utils.StatusToInt(item.Text);
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")
                MyStatus = myPrevStatus;
            
            if(MyStatus == (int)AnimeStatus.Completed && _allEpisodes != 0)
                PromptForWatchedEpsChange(_allEpisodes);

            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeScore(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            int myPrevScore = MyScore;
            MyScore = int.Parse(btn.Text.Split('-').First());
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response != "Updated")            
                MyScore = myPrevScore;
            
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region CustomTilePin

        private void TxtTileUrl_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            var txt = sender as TextBox;
            txt.IsEnabled = false; //reset input
            txt.IsEnabled = true;
            PinTile(txt.Text);
            CloseTileUrlInput(null, null);
        }

        public void OpenTileUrlInput()
        {
            TxtTileUrl.Text = "";        
            Utils.GetMainPageInstance().AnimeListScrollTo(this);
            TileUrlInput.Visibility = Visibility.Visible;
            TxtTileUrl.Focus(FocusState.Keyboard);
        }

        private void CloseTileUrlInput(object sender, RoutedEventArgs e)
        {
            TileUrlInput.Visibility = Visibility.Collapsed;
        }
        #endregion

        private async void AddThisToMyList(object sender, RoutedEventArgs e)
        {
            string response = await new AnimeAddQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Created"))
                return;

            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;


            
            SetAuthStatus(true);
            AdjustIncrementButtonsVisibility();
            BtnAddToList.Visibility = Visibility.Collapsed;
            Utils.GetMainPageInstance().AddAnimeEntry(Creditentials.UserName,_parentAbstraction);
        }

        private void NavigateDetails(object sender, RoutedEventArgs e)
        {
            Utils.GetMainPageInstance() 
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Id, Title, null,this,
                        Utils.GetMainPageInstance().GetCurrentListOrderParams(_seasonalState)));
        }

        #region Prompts

        private async void PromptForStatusChange(int to)
        {
            if(MyStatus == to)
                return;
            var msg = new MessageDialog($"From : {Utils.StatusToString(MyStatus)}\nTo : {Utils.StatusToString(to)}","Would you like to change current status?");
            bool confirmation = false;
            msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();
            if (confirmation)
            {
                int myPrevStatus = MyStatus;
                MyStatus = to;
                string response = await new AnimeUpdateQuery(this).GetRequestResponse();              
                if (response != "Updated")
                    MyStatus = myPrevStatus;
            }
        }

        private async void PromptForWatchedEpsChange(int to)
        {
            if (MyEpisodes == to)
                return;
            var msg = new MessageDialog($"From : {MyEpisodes}\nTo : {to}", "Would you like to change watched episodes value?");
            bool confirmation = false;
            msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();
            if (confirmation)
            {
                int myPrevEps = MyEpisodes;
                MyEpisodes = to;
                string response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    MyStatus = myPrevEps;

                AdjustIncrementButtonsVisibility();
            }
        }
        #endregion


    }
}
