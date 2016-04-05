using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MALClient.Models.Favourites
{
    public class FavCharacter
    {
        public string Name { get; set; }
        public string OriginatingShowName { get; set; }
        public string ImgUrl { get; set; }
        public string Id { get; set; }
        public string ShowId { get; set; }
        public bool FromAnime { get; set; }
        public BitmapImage ImgBitmap { get; private set; }
        public void LoadBitmap()
        {
            if(ImgBitmap != null)
                return; 
            ImgBitmap = new BitmapImage(new Uri(ImgUrl));
        }
    }
}
