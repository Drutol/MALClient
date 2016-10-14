using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models.Misc;

namespace MALClient.XShared.ViewModels.Items
{
    public class WallpaperItemViewModel : ViewModelBase
    {
        private bool _isBlurred;
        private ICommand _revealCommand;
        private string _resolution;
        public AnimeWallpaperData Data { get; set; }

        public WallpaperItemViewModel(AnimeWallpaperData data)
        {
            Data = data;
            IsBlurred = data.Nsfw;
        }

        public bool IsBlurred
        {
            get { return _isBlurred; }
            set
            {
                if(_isBlurred == value)
                    return;

                _isBlurred = value;
                RaisePropertyChanged(() => IsBlurred);
            }
        }

        public string Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = value;
                RaisePropertyChanged(() => Resolution);
            }
        }

        public ICommand RevealCommand => _revealCommand ?? (_revealCommand = new RelayCommand(() => IsBlurred = false));

    }
}
