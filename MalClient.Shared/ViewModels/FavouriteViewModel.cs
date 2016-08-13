using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Models.Favourites;
using MalClient.Shared.Utils.Managers;

namespace MalClient.Shared.ViewModels
{
    public class FavouriteViewModel : ViewModelBase
    {
        public FavouriteBase Data { get; set; }

        public FavouriteViewModel(FavouriteBase data)
        {
            Data = data;
        }

        //View sepcific + Logic
        private bool? _isFavourite;

        public bool IsFavourite
        {
            get { return (bool)(_isFavourite ?? (_isFavourite = FavouritesManager.IsFavourite(Data.Type, Data.Id))); }
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
                await FavouritesManager.AddFavourite(Data.Type, Data.Id);
            else
                await FavouritesManager.RemoveFavourite(Data.Type, Data.Id);
            IsFavouriteButtonEnabled = true;
        }));
    }
}
