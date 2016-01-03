using System;
using System.IO;
using System.Net.Http;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        public int Id;
        public int status;
        public string title;
        private string ImgUrl;

        public AnimeItem(string name,string img,int id,int status)
        {
            this.InitializeComponent();
            Id = id;
            Img.Source = new BitmapImage(new Uri(img));
            Status.Text = Utils.Utils.StatusToString(status);
            Ttile.Text = name;
            this.status = status;
            title = name;
            ImgUrl = img;
        }

        public async void PinTile(string targetUrl)
        {

            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var thumb = await folder.CreateFileAsync($"{Id}.png", CreationCollisionOption.ReplaceExisting);

            HttpClient http = new HttpClient();
            byte[] response = await http.GetByteArrayAsync(ImgUrl); //get bytes

            var fs = await thumb.OpenStreamForWriteAsync(); //get stream

            using (DataWriter writer = new DataWriter(fs.AsOutputStream()))
            {
                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();
            }
            var til = new SecondaryTile($"{Id}", $"{title}", targetUrl, new Uri($"ms-appdata:///local/{Id}.png"), TileSize.Default);
            Utils.Utils.RegisterTile(Id.ToString());
            await til.RequestCreateAsync();
        }

        public void Setbackground(SolidColorBrush brush)
        {
            Root.Background = brush;
        }
    }
}
