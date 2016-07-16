using System.Runtime.Serialization;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm.MagicalRawQueries;
using MALClient.Utils;

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