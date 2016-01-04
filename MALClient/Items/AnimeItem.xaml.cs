using System;
using System.IO;
using System.Net.Http;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private bool _imgLoaded = false;
        internal object score;

        public AnimeItem(bool auth,string name,string img,int id,int status,int watchedEps,int allEps,int score)
        {
            this.InitializeComponent();
            Id = id;
            
            Status.Content = MALClient.Utils.StatusToString(status);
            WatchedEps.Text = $"{watchedEps}/{allEps}";
            Ttile.Text = name;
            this.status = status;
            this.score = score;
            title = name;
            _imgUrl = img;
            WatchedEpisodes = watchedEps;
            AllEpisodes = allEps;

            if (!auth)
            {
                IncrementEps.Visibility = Visibility.Collapsed;
                DecrementEps.Visibility = Visibility.Collapsed;
                Status.IsEnabled = false;
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
            var til = new SecondaryTile($"{Id}", $"{title}", targetUrl, new Uri($"ms-appdata:///local/{Id}.png"), TileSize.Default);
            MALClient.Utils.RegisterTile(Id.ToString());
            await til.RequestCreateAsync();
        }

        public void Setbackground(SolidColorBrush brush)
        {
            Root.Background = brush;
        }

        private async void IncrementWatchedEp(object sender, RoutedEventArgs e)
        {
            WatchedEpisodes++;
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
                WatchedEps.Text = $"{WatchedEpisodes}/{AllEpisodes}";
        }

        private async void DecrementWatchedEp(object sender, RoutedEventArgs e)
        {
            WatchedEpisodes--;
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
                WatchedEps.Text = $"{WatchedEpisodes}/{AllEpisodes}";
        }

        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            status = MALClient.Utils.StatusToInt(item.Text);
            string response = await new AnimeUpdateQuery(this).GetRequestResponse();
            if (response == "Updated")
            {
                Status.Content = MALClient.Utils.StatusToString(status);
            }
        }
    }
}
