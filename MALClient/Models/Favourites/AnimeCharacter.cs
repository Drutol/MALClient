using System.Runtime.Serialization;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm.MagicalRawQueries;
using MALClient.Utils;
using WinRTXamlToolkit.IO.Serialization;

namespace MALClient.Models.Favourites
{
    [DataContract]
    public class AnimeCharacter : FavouriteBase
    {
        [DataMember]
        public string ShowId { get; set; }
        [DataMember]
        public bool FromAnime { get; set; }

        protected override FavouriteType Type { get; } = FavouriteType.Character;
    }
}