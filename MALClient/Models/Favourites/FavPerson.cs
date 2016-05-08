using System;
using Windows.UI.Xaml.Media.Imaging;

namespace MALClient.Models.Favourites
{
    public class FavPerson
    {
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Id { get; set; }
        public BitmapImage ImgBitmap { get; private set; }

        public void LoadBitmap()
        {
            if (ImgBitmap != null)
                return;
            ImgBitmap = new BitmapImage(new Uri(ImgUrl));
        }
    }
}