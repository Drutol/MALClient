using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.Utils.Managers;

namespace MALClient.XShared.ViewModels
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
