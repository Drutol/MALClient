using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
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
    public sealed partial class AnimeItem : UserControl
    {
        //prop field pairs
        private int _myStatus;
        private int _myScore;
        private int _myEpisodes;
        private string _title;
        private float _globalScore;

        public int MyStatus
        {
            get { return _myStatus; }
            private set
            {
                _myStatus = value;
                BtnStatus.Content = Utils.StatusToString(value);
            }
        }
        public int MyScore
        {
            get { return _myScore; }
            private set
            {
                _myScore = value;
                BtnScore.Content = value > 0 ? $"{value}/10" : "Unranked";
            }
        }
        public int MyEpisodes
        {
            get { return _myEpisodes; }
            private set
            {
                _myEpisodes = value;
                TxtWatchedEps.Text = $"{value}/{(_allEpisodes == 0 ? "?" : _allEpisodes.ToString())}";
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
                _globalScore = value;
                TxtGlobalSocore.Text = value.ToString();
            }
        }

        //fields
        public readonly int Id;
        private readonly string _imgUrl;
        private int _allEpisodes;
        public int Index;
        //state fields
        private bool _expandState = false;
        private bool _seasonalState = false;
        private bool _imgLoaded = false;

        //props
        private int SeasonalMembers { get; set; } //TODO : Use this
        



        public AnimeItem(bool auth,string name,string img,int id,int myStatus,int myEps,int allEps,int myScore) //We are loading an item that IS on the list , it it's seasonal there's static "enhancing" method down below
        {
            //Base init
            this.InitializeComponent();
            //Assign fields
            Id = id;
            _imgUrl = img;
            _allEpisodes = allEps;
            //Assign properties
            MyStatus = myStatus;
            MyScore = myScore;
            Title = name;
            MyEpisodes = myEps;
            BtnShowMore.Visibility = Visibility.Collapsed;
           
            //We are not seasonal so it's already on list            
            BtnAddToList.Visibility = Visibility.Collapsed;

            if (auth) return; //Some other source lookup
            IncrementEps.Visibility = Visibility.Collapsed;
            DecrementEps.Visibility = Visibility.Collapsed;
            BtnStatus.IsEnabled = false;
            BtnScore.IsEnabled = false;
        }

        public AnimeItem(SeasonalAnimeData data, Dictionary<int, XElement> dl, Dictionary<int, AnimeItem> loaded)
            //We are loading an item that is NOT on the list and is seasonal
        {
            //Base init
            this.InitializeComponent();
            _seasonalState = true;
            //Assign Fields
            Id = data.Id;
            _imgUrl = data.ImgUrl;
            _myEpisodes = 0; // We don't want to set TextBlock
            //Assign properties
            MyStatus = (int) AnimeStatus.AllOrAiring;
            Title = data.Title;
            MyScore = 0;
            GlobalScore = data.Score;
            SeasonalMembers = data.Members;

            //Custom controls setup
            TxtWatchedEps.Text = $"{data.Episodes} Episodes";
            SymbolAiring.Visibility = Visibility.Visible;

            //Additional data from seasonal
            TxtSynopsis.Text = data.Synopsis;
            Index = data.Index;
            SymbolGlobalScore.Visibility = Visibility.Visible;
            
            //We are not on the list so we cannot really do this
            SetAuthStatus(false);

            Task.Run(async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    AnimeItem reference;
                    XElement re;
                    if (loaded.TryGetValue(Id, out reference))
                    {
                        _allEpisodes = reference._allEpisodes;
                        MyStatus = reference.MyStatus;
                        MyScore = reference.MyScore;
                        MyEpisodes = reference.MyEpisodes;

                        SetAuthStatus(true);

                        var dataCache = Utils.GetMainPageInstance().RetrieveLoadedAnime();
                        dataCache.LoadedAnime.Remove(reference);
                        dataCache.LoadedAnime.Add(this);

                    }
                    else if (dl.TryGetValue(Id, out re))
                    {
                        //Assign properties
                        _allEpisodes = Convert.ToInt32(re.Element("series_episodes").Value);
                        MyStatus = Convert.ToInt32(re.Element("my_status").Value);
                        MyScore = Convert.ToInt32(re.Element("my_score").Value);
                        MyEpisodes = Convert.ToInt32(re.Element("my_watched_episodes").Value);
                        

                        SetAuthStatus(true);
                        //We are not seasonal so it's already on list            

                        Utils.GetMainPageInstance().RetrieveLoadedAnime().AnimeItemLoaded(this);
                    }


                });
            });
        }

        #region Utils/Helpers
        /// <summary>
        /// When item is loaded it's image is set , why would we want to load it when it isn't even visible?
        /// </summary>
        public void ItemLoaded()
        {
            if (!_imgLoaded)
            {
                Img.Source = new BitmapImage(new Uri(_imgUrl));
                _imgLoaded = true;
            }
        }
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

        private void HideSynopsis(object sender, RoutedEventArgs e)
        {
            SynopsisHide.Begin();
            _expandState = false;
        }

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

        private void SetAuthStatus(bool auth)
        {
            if (auth)
            {
                BtnAddToList.Visibility = Visibility.Collapsed;
                IncrementEps.Visibility = Visibility.Visible;
                DecrementEps.Visibility = Visibility.Visible;
                BtnStatus.IsEnabled = true;
                BtnScore.IsEnabled = true;
            }
            else
            {
                BtnAddToList.Visibility = Visibility.Visible;
                IncrementEps.Visibility = Visibility.Collapsed;
                DecrementEps.Visibility = Visibility.Collapsed;
                BtnStatus.IsEnabled = false;
                BtnScore.IsEnabled = false;
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
                            new AnimeDetailsPageNavigationArgs(Id, Title, null,
                                Utils.GetMainPageInstance().GetCurrentListOrderParams()));

                }
            }
        }
        #endregion

        #region AnimeUpdate

        #region Watched
            private async void IncrementWatchedEp(object sender, RoutedEventArgs e)
            {
                SpinnerLoading.Visibility = Visibility.Visible;
                MyEpisodes++;
                string response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    MyEpisodes--; // Shouldn't occur really , but hey shouldn't and MAL api goes along very well.

                SpinnerLoading.Visibility = Visibility.Collapsed;
            }

            private async void DecrementWatchedEp(object sender, RoutedEventArgs e)
            {
                SpinnerLoading.Visibility = Visibility.Visible;
                MyEpisodes--;
                string response = await new AnimeUpdateQuery(this).GetRequestResponse();
                if (response != "Updated")
                    TxtWatchedEps.Text = $"{MyEpisodes}/{_allEpisodes}";

                SpinnerLoading.Visibility = Visibility.Collapsed;
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

        //changed in some other place , implemented before these were properties
        public void ChangeWatched(int newWatched)
        {
            MyEpisodes = newWatched;
        }

        public void ChangeStatus(int newStatus)
        {
            MyStatus = newStatus;
        }

        public void ChangeScore(int newScore)
        {
            MyScore = newScore;
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
            TileUrlInput.Visibility = Visibility.Visible;
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

            Utils.GetMainPageInstance().AddAnimeEntry(Creditentials.UserName,this);
        }

        private void NavigateDetails(object sender, RoutedEventArgs e)
        {
            Utils.GetMainPageInstance() 
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Id, Title, null,
                        Utils.GetMainPageInstance().GetCurrentListOrderParams()));
        }

        //Statics

        internal static AnimeItem EnhanceWithSeasonalData(SeasonalAnimeData data)
        {
            data.AnimeItemRef.TxtSynopsis.Text = data.Synopsis;
            data.AnimeItemRef.SymbolAiring.Visibility = Visibility.Visible;
            data.AnimeItemRef.BtnAddToList.Visibility = Visibility.Collapsed;
            data.AnimeItemRef.Index = data.Index;
            return data.AnimeItemRef;
        }


    }
}
