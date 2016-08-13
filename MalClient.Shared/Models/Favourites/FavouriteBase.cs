using System.Runtime.Serialization;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm.MagicalRawQueries;
using MalClient.Shared.Utils.Managers;

namespace MalClient.Shared.Models.Favourites
{
    public abstract class FavouriteBase
    {
        public string Name { get; set; }
        public string Notes { get; set; } //show name, role etc. filled diffrently depending on context
        public string ImgUrl { get; set; }
        public string Id { get; set; }

        public abstract FavouriteType Type { get; }
    }
}
