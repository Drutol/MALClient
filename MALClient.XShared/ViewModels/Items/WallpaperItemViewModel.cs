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
        private ICommand _copyLinkCommand;
        private ICommand _openRedditCommand;
        private ICommand _saveCommand;
        private ICommand _saveAsCommand;
        private ICommand _copyAndWaifuCommand;
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

        public ICommand CopyLinkCommand => _copyLinkCommand ?? (_copyLinkCommand = new RelayCommand(() => ResourceLocator.ClipboardProvider.SetText(Data.FileUrl)));
        public ICommand OpenRedditCommand => _openRedditCommand ?? (_openRedditCommand = new RelayCommand(() => ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(Data.RedditUrl))));
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(() => ResourceLocator.ImageDownloaderService.DownloadImageDefault(Data.FileUrl,string.Join(" ",Data.Title.Split(' ').Take(3)),false)));
        public ICommand SaveAsCommand => _saveAsCommand ?? (_saveAsCommand = new RelayCommand(() => ResourceLocator.ImageDownloaderService.DownloadImage(Data.FileUrl, Data.Title, false)));
        public ICommand CopyAndWaifuCommand => _copyAndWaifuCommand ?? (_copyAndWaifuCommand = new RelayCommand(() =>
                                               {
                                                   ResourceLocator.ClipboardProvider.SetText(Data.FileUrl);
                                                   ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("http://waifu2x.booru.pics/"));
                                               }));

    }
}
