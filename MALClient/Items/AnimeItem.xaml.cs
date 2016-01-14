using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.Foundation;
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
using MALClient.Comm;
using MALClient.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        public int Id;
        public int MyStatus;
        public int MyScore;
        public int WatchedEpisodes;
        public string title;
        private string _imgUrl;
        public readonly int AllEpisodes;
        private bool _expandState = false;
        private bool _seasonalState = false;
        public int Index { get; set; }


        private bool _imgLoaded = false;

        public AnimeItem(bool auth,string name,string img,int id,int myStatus,int watchedEps,int allEps,int myScore)
        {
            this.InitializeComponent();
            Id = id;
            
            Status.Content = Utils.StatusToString(myStatus);
            WatchedEps.Text = $"{watchedEps}/{allEps}";
            Title.Text = name;
            MyStatus = myStatus;
            MyScore = myScore;
            title = name;
            _imgUrl = img;
            WatchedEpisodes = watchedEps;
            AllEpisodes = allEps;
            BtnScore.Content = myScore > 0 ? $"{myScore}/10" : "Unranked";

            if (!auth)
            {
                IncrementEps.Visibility = Visibility.Collapsed;
                DecrementEps.Visibility = Visibility.Collapsed;
                Status.IsEnabled = false;
                BtnScore.IsEnabled = false;
            }
        }

        public AnimeItem(SeasonalAnimeData data)
        {
            this.InitializeComponent();

            Id = data.Id;
            Status.Content = "Airing";
            Title.Text = data.Title;
            WatchedEps.Text = $"{data.Episodes} Episodes";
            _imgUrl = data.ImgUrl;
            TxtSynopsis.Text = data.Synopsis;
            Index = data.Index;

            MyStatus = 7; //as for all filtering

            _seasonalState = true;

            IncrementEps.Visibility = Visibility.Collapsed;
            DecrementEps.Visibility = Visibility.Collapsed;
            Status.IsEnabled = false;
            BtnScore.IsEnabled = false;
            BtnAddToList.Visibility = Visibility.Visible;
        }

        public void ItemLoaded()
        {
            if (!_imgLoaded)
            {
                Img.Source = new BitmapImage(new Uri(_imgUrl));
                _imgLoaded = true;
            }
        }

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
            var til = new SecondaryTile($"{Id}", $"{title}", targetUrl, new Uri($"ms-appdata:///local/{Id}.png"), TileSize.Default);
            Utils.RegisterTile(Id.ToString());
            await til.RequestCreateAsync();
        }

        public void Setbackground(SolidColorBrush brush)
        {
            Root.Background = brush;
        }

        private Point _initialPoint;
        private bool _manipulating;
        private MessageDialog _msg; 
        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
            _manipulating = true;
        }

        private async void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && _manipulating)
            {
                Point currentpoint = e.Position;
                if (currentpoint.X - _initialPoint.X >= 70) // swipe right
                {
                    e.Complete();
                    e.Handled = true;
                    _manipulating = false;
                    if (!Creditentials.Authenticated)
                    {
                        _msg = new MessageDialog("Log in in order to see details.");
                        await _msg.ShowAsync();
                    }
                    else
                        Utils.GetMainPageInstance()
                            .Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(Id, title, null,
                                    Utils.GetMainPageInstance().GetCurrentListOrderParams()));

                }
            }
        }

        private async void IncrementWatchedEp(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            WatchedEpisodes++;
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
                WatchedEps.Text = $"{WatchedEpisodes}/{AllEpisodes}";
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void DecrementWatchedEp(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            WatchedEpisodes--;
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
                WatchedEps.Text = $"{WatchedEpisodes}/{AllEpisodes}";
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        public void ChangeWatched(int newWatched)
        {
            WatchedEpisodes = newWatched;
            WatchedEps.Text = $"{newWatched}/{AllEpisodes}";
        }

        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var item = sender as MenuFlyoutItem;
            MyStatus = MALClient.Utils.StatusToInt(item.Text);
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
            {
                Status.Content = MALClient.Utils.StatusToString(MyStatus);
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        public void ChangeStatus(int newStatus)
        {
            MyStatus = newStatus;
            BtnScore.Content = Utils.StatusToString(newStatus);
        }

        private async void ChangeScore(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            MyScore = int.Parse(btn.Text.Split('-').First());
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
            {
                BtnScore.Content = $"{MyScore}/10";
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        public void ChangeScore(int newScore)
        {
            MyScore = newScore;
            BtnScore.Content = $"{newScore}/10";
        }

        private void PinTile(object sender, RoutedEventArgs e)
        {
            PinTile(TxtTileUrl.Text);
            TileUrlInput.Visibility = Visibility.Collapsed;
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

        private void TxtTileUrl_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var txt = sender as TextBox;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true;
                PinTile(txt.Text);
                TileUrlInput.Visibility = Visibility.Collapsed;
            }
        }

        private void AddThisToMyList(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        
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
    }
}
