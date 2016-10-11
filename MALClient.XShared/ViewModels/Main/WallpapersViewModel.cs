using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Models.Misc;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels.Main
{
    public class WallpapersViewModel : ViewModelBase
    {
        private WallpaperPageNavigationArgs _prevArgs;
        private ObservableCollection<AnimeWallpaperData> _wallpapers;
        private bool _loadingWallpapersVisibility;
        private bool _noWallpapersNoticeVisibility;

        public ObservableCollection<AnimeWallpaperData> Wallpapers
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

        public async void Init(WallpaperPageNavigationArgs args)
        {
            if(_prevArgs != null && string.Equals(_prevArgs.Query,args.Query,StringComparison.CurrentCultureIgnoreCase))
                return;

            _prevArgs = args;

            LoadingWallpapersVisibility = true;


            var wallpapers = await AnimeWallpapersQuery.GetAllWallpapers();
            if(wallpapers != null)
                Wallpapers = new ObservableCollection<AnimeWallpaperData>(wallpapers.Take(20));

            LoadingWallpapersVisibility = false;
            NoWallpapersNoticeVisibility = Wallpapers?.Any() ?? true;
        }
    }
}
