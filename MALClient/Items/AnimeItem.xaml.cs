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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        public int Id;
        public int status;
        public string title;
        private string _imgUrl;
        public int WatchedEpisodes;
        public int AllEpisodes;
        public int Score;

        private bool _imgLoaded = false;

        public AnimeItem(bool auth,string name,string img,int id,int status,int watchedEps,int allEps,int score)
        {
            this.InitializeComponent();
            Id = id;
            
            Status.Content = MALClient.Utils.StatusToString(status);
            WatchedEps.Text = $"{watchedEps}/{allEps}";
            Ttile.Text = name;
            this.status = status;
            Score = score;
            title = name;
            _imgUrl = img;
            WatchedEpisodes = watchedEps;
            AllEpisodes = allEps;
            if(score > 0)
                BtnScore.Content = $"{score}/10";
            else
            {
                BtnScore.Content = "Unranked";
            }

            if (!auth)
            {
                IncrementEps.Visibility = Visibility.Collapsed;
                DecrementEps.Visibility = Visibility.Collapsed;
                Status.IsEnabled = false;
                BtnScore.IsEnabled = false;
            }
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

            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
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
            MALClient.Utils.RegisterTile(Id.ToString());
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
                        Utils.GetMainPageInstance().NavigateDetails(null,Id,title);

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

        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var item = sender as MenuFlyoutItem;
            status = MALClient.Utils.StatusToInt(item.Text);
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
            {
                Status.Content = MALClient.Utils.StatusToString(status);
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeScore(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            Score = int.Parse(btn.Text.Split('-').First());
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
            {
                BtnScore.Content = $"{Score}/10";
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
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
    }
}
