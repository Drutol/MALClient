using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models.Misc;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Items;

namespace MALClient.XShared.ViewModels.Main
{
    public class WallpapersViewModel : ViewModelBase
    {
        private WallpaperPageNavigationArgs _prevArgs;
        private ObservableCollection<WallpaperItemViewModel> _wallpapers;
        private bool _loadingWallpapersVisibility;
        private bool _noWallpapersNoticeVisibility;
        private int _currentPage;
        private ICommand _goForwardCommand;
        private ICommand _goBackwardsCommand;

        public ObservableCollection<WallpaperItemViewModel> Wallpapers
        {
            get { return _wallpapers; }
            set
            {
                _wallpapers = value;
                RaisePropertyChanged(() => Wallpapers);
            }
        }

        public bool LoadingWallpapersVisibility
        {
            get { return _loadingWallpapersVisibility; }
            set
            {
                _loadingWallpapersVisibility = value;
                RaisePropertyChanged(() => LoadingWallpapersVisibility);
            }
        }

        public bool NoWallpapersNoticeVisibility
        {
            get { return _noWallpapersNoticeVisibility; }
            set
            {
                _noWallpapersNoticeVisibility = value;
                RaisePropertyChanged(() => NoWallpapersNoticeVisibility);
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(() => IsGoBackwardsButtonEnabled);
            }
        }

        public ICommand GoForwardCommand => _goForwardCommand ?? (_goForwardCommand = new RelayCommand(() =>
                                            {
                                                CurrentPage++;
                                                LoadWallpapers();
                                            }));

        public ICommand GoBackwardsCommand => _goBackwardsCommand ?? (_goBackwardsCommand = new RelayCommand(() =>
                                              {
                                                  CurrentPage--;
                                                  LoadWallpapers();
                                              }));


        public bool IsGoBackwardsButtonEnabled => CurrentPage != 0;

        public void Init(WallpaperPageNavigationArgs args)
        {
            if(_prevArgs != null && string.Equals(_prevArgs.Query,args.Query,StringComparison.CurrentCultureIgnoreCase))
                return;

            _prevArgs = args;

            CurrentPage = 0;
            LoadWallpapers();
        }

        private async void LoadWallpapers()
        {
            LoadingWallpapersVisibility = true;
            Wallpapers?.Clear();
            var wallpapers = await AnimeWallpapersQuery.GetAllWallpapers(CurrentPage);
            if (wallpapers != null)
                Wallpapers = new ObservableCollection<WallpaperItemViewModel>(wallpapers.Select(data => new WallpaperItemViewModel(data)));
            LoadingWallpapersVisibility = false;
            NoWallpapersNoticeVisibility = Wallpapers?.Any() ?? true;
        }
    }
}
