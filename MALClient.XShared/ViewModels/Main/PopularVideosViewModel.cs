using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.MalSpecific;

namespace MALClient.XShared.ViewModels.Main
{
    public class PopularVideosViewModel : ViewModelBase
    {
        private ObservableCollection<AnimeVideoData> _videos;
        private bool _loading;

        public ObservableCollection<AnimeVideoData> Videos
        {
            get { return _videos; }
            set
            {
                _videos = value;
                RaisePropertyChanged(() => Videos);
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public double ItemWidth => AnimeItemViewModel.MaxWidth;

        public async void Init()
        {
            if(Videos?.Any() ?? false)
                return;

            Loading = true;

            var videos = await new PopularVideosQuery().GetVideos();

            Videos = new ObservableCollection<AnimeVideoData>(videos);

            Loading = false;
        }
    }
}
