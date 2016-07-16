using System.Runtime.Serialization;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm.MagicalRawQueries;
using MalClient.Shared.Utils.Managers;

namespace MalClient.Shared.Models.Favourites
{
    [DataContract]
    public abstract class FavouriteBase : ViewModelBase
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Notes { get; set; } //show name, role etc. filled diffrently depending on context
        [DataMember]
        public string ImgUrl { get; set; }
        [DataMember]
        public string Id { get; set; }

        protected abstract FavouriteType Type { get; }

        //View sepcific + Logic
        private bool? _isFavourite;

        public bool IsFavourite
        {
            get { return (bool)(_isFavourite ?? (_isFavourite = FavouritesManager.IsFavourite(Type, Id))); }
            set
            {
                _isFavourite = value;
                RaisePropertyChanged(() => IsFavourite);
                RaisePropertyChanged(() => FavouriteSymbolIcon);
            }
        }

        private bool _isFavouriteButtonEnabled = true;

        public bool IsFavouriteButtonEnabled
        {
            get { return _isFavouriteButtonEnabled; }
            set
            {
                _isFavouriteButtonEnabled = value;
                RaisePropertyChanged(() => IsFavouriteButtonEnabled);
            }
        }

        public Symbol FavouriteSymbolIcon => IsFavourite ? Symbol.UnFavorite : Symbol.Favorite;

        private ICommand _toggleFavouriteCommand;

        public ICommand ToggleFavouriteCommand => _toggleFavouriteCommand ?? (_toggleFavouriteCommand = new RelayCommand(async () =>
        {
            IsFavouriteButtonEnabled = false;
            IsFavourite = !IsFavourite;
            if (IsFavourite)
                await FavouritesManager.AddFavourite(Type, Id);
            else
                await FavouritesManager.RemoveFavourite(Type, Id);
            IsFavouriteButtonEnabled = true;
        }));
    }
}
